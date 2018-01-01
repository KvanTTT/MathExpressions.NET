using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionsNET
{
	public class CountNumber
	{
		public int Number;
		public int Count;
		public bool Calculated;

		public override string ToString()
			=> $"Number = {Number}; Count = {Count}; Calculated = {Calculated}";
	}

	public struct OpCodeArg
	{
		public OpCode OpCode;
		public object Arg;

		public OpCodeArg(OpCode opCode)
		{
			OpCode = opCode;
			Arg = null;
		}

		public OpCodeArg(OpCode opCode, object arg)
		{
			OpCode = opCode;
			Arg = arg;
		}

		public override string ToString()
		{
			return string.Format("{0,-10}{1}", OpCode, Arg);
		}
	}

	public struct VariableLifetimeCycle
	{
		public int BeginInd;
		public int EndInd;

		public VariableLifetimeCycle(int beginInd, int endInd)
		{
			BeginInd = beginInd;
			EndInd = endInd;
		}
	}

	public partial class MathFunc
	{
		private const string NamespaceName = "MathFuncLib";
		private const string ClassName = "MathFunc";

		public double DerivativeDelta = 0.000001;
		public bool Static = true;

		public MathFuncAssemblyCecil MathFuncAssembly
		{
			get;
			private set;
		}

		private List<OpCodeArg> IlInstructions;
		private int LocalVarNumber;
		private Dictionary<FuncNode, CountNumber> FuncNodes;

		public ICollection<Instruction> Instructions
		{
			get;
			private set;
		}

		public void Compile(MathFuncAssemblyCecil mathFuncAssembly, string funcName)
		{
			MathFuncAssembly = mathFuncAssembly;

			var globalBody = new MethodDefinition(funcName,
				MethodAttributes.Public | MethodAttributes.HideBySig | (Static ? MethodAttributes.Static : 0), mathFuncAssembly.DoubleType);
			
			var globalIlProcessor = globalBody.Body.GetILProcessor();

			AddFuncArgs(mathFuncAssembly.Assembly, globalIlProcessor);

			DefineLocals();
			IlInstructions = new List<OpCodeArg>();
			EmitNode(Root);
			IlInstructions.Add(new OpCodeArg(OpCodes.Ret));
			OptimizeInstructions();
			OptimizeLocalVariables(ref LocalVarNumber);

			for (int i = 0; i < LocalVarNumber; i++)
				globalBody.Body.Variables.Add(new VariableDefinition(mathFuncAssembly.DoubleType));

			foreach (var instr in IlInstructions)
				EmitInstruction(globalIlProcessor, instr, Static);

			Instructions = globalBody.Body.Instructions;

			mathFuncAssembly.Class.Methods.Add(globalBody);
		}

		private void AddFuncArgs(AssemblyDefinition assembly, ILProcessor ilProc)
		{
			int paramNumber = 0;

			var method = ilProc.Body.Method;
			method.Parameters.Add(new ParameterDefinition(Variable.Name, ParameterAttributes.None, MathFuncAssembly.DoubleType));
			Variable.ArgNumber = paramNumber++;

			foreach (var param in Parameters)
			{
				method.Parameters.Add(new ParameterDefinition(param.Value.Name, ParameterAttributes.None, MathFuncAssembly.DoubleType));
				param.Value.ArgNumber = paramNumber++;
			}

			var unknownFuncType = assembly.MainModule.Import(typeof(Func<double, double>));
			foreach (var func in UnknownFuncs)
			{
				method.Parameters.Add(new ParameterDefinition(func.Value.Name, ParameterAttributes.None, unknownFuncType));
				func.Value.ArgNumber = paramNumber++;
			}

			SetParamsAndUnknownFuncsArgNumbers(Root);
		}

		protected void SetParamsAndUnknownFuncsArgNumbers(MathFuncNode node)
		{
			foreach (var child in node.Children)
				SetParamsAndUnknownFuncsArgNumbers(child);

			if (node.Type == MathNodeType.Function && !((FuncNode)node).IsKnown)
				node.ArgNumber = UnknownFuncs[node.Name].ArgNumber;
			else if (node.Type == MathNodeType.Constant)
				node.ArgNumber = Parameters[node.Name].ArgNumber;
			else if (node.Type == MathNodeType.Variable)
				node.ArgNumber = Variable.ArgNumber;
		}

		#region Init Locals

		private void DefineLocals()
		{
			LocalVarNumber = 0;
			FuncNodes = new Dictionary<FuncNode, CountNumber>();
			DefineLocals(Root);

			InvertLocalVariablesNumbers(Root);
		}

		private void DefineLocals(MathFuncNode node)
		{
			var funcNode = node as FuncNode;
			if (funcNode == null)
				return;

			CountNumber value;
			var hash = funcNode.GetHashCode();
			if (FuncNodes.TryGetValue(funcNode, out value))
			{
				funcNode.Number = value.Number;
				FuncNodes[funcNode].Count += 1;
			}
			else
			{
				FuncNodes.Add(funcNode, new CountNumber() { Number = LocalVarNumber, Count = 1, Calculated = false });
				funcNode.Number = LocalVarNumber;
				LocalVarNumber += funcNode.Children.Count;
			}

			foreach (var child in node.Children)
				DefineLocals(child);
		}

		private void InvertLocalVariablesNumbers(MathFuncNode node)
		{
			var funcNode = node as FuncNode;
			if (funcNode != null)
			{
				funcNode.Number = LocalVarNumber - funcNode.Number - funcNode.Children.Count;
				FuncNodes[funcNode].Number = funcNode.Number;

				foreach (var child in funcNode.Children)
					InvertLocalVariablesNumbers(child);
			}
		}

		#endregion

		#region Emit AST Nodes

		private void EmitNode(MathFuncNode node, bool negExpAbs = false)
		{
			switch (node.Type)
			{
				case MathNodeType.Calculated:
					IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8,
						negExpAbs ? Math.Abs(((CalculatedNode)node).Value) : ((CalculatedNode)node).Value));
					break;
				case MathNodeType.Value:
					IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8,
						negExpAbs ? Math.Abs(((ValueNode)node).Value.ToDouble()) : ((ValueNode)node).Value.ToDouble()));
					break;

				case MathNodeType.Constant:
				case MathNodeType.Variable:
					IlInstructions.Add(new OpCodeArg(OpCodes.Ldarg, node.ArgNumber));
					break;

				case MathNodeType.Function:
					var funcNode = node as FuncNode;
					var func = FuncNodes[funcNode];
					if (!func.Calculated)
					{
						EmitFunc(funcNode, negExpAbs);
						func.Calculated = true;
						// if (FuncNodes[funcNode].Count > 1) TODO: this optimization disallowed due to derivatives.
						{
							IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number));
							IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						}
					}
					else
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
					break;
			}
		}

		private bool EmitFunc(FuncNode funcNode, bool negExpAbs = false)
		{
			switch (funcNode.FunctionType)
			{
				case KnownFuncType.Add:
					return EmitAddFunc(funcNode);
				case KnownFuncType.Sub:
					return EmitSubFunc(funcNode);
				case KnownFuncType.Mult:
					return EmitMultFunc(funcNode);
				case KnownFuncType.Div:
					return EmitDivFunc(funcNode);
				case KnownFuncType.Neg:
					return EmitNegFunc(funcNode, negExpAbs);
				case KnownFuncType.Pow:
					return EmitExpFunc(funcNode, negExpAbs);
				case KnownFuncType.Diff:
					return EmitDiffFunc(funcNode);
			}

			if (!EmitKnownFunc(funcNode)) // Unknown function (from input args).
				EmitUnknownFunc(funcNode);

			return true;
		}

		private bool EmitAddFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem = null;

			firstItem = funcNode.Children.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && FuncNodes[func].Count == 1 && func.LessThenZero());
			});

			if (firstItem == null)
				firstItem = funcNode.Children[0];

			EmitNode(firstItem);

			for (int i = 0; i < funcNode.Children.Count; i++)
			{
				if (funcNode.Children[i] == firstItem)
					continue;

				var func = funcNode.Children[i] as FuncNode;
				if (func != null && FuncNodes[func].Count == 1 && func.LessThenZero())
				{
					EmitNode(func.Children[0], true);
					IlInstructions.Add(new OpCodeArg(OpCodes.Sub));
				}
				else
				{
					EmitNode(funcNode.Children[i]);
					IlInstructions.Add(new OpCodeArg(OpCodes.Add));
				}
			}

			return true;
		}

		private bool EmitSubFunc(FuncNode funcNode)
		{
			EmitNode(funcNode.Children[0]);
			for (int i = 1; i < funcNode.Children.Count; i++)
			{
				EmitNode(funcNode.Children[i]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Sub));
			}

			return true;
		}

		private bool EmitMultFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem = null;

			firstItem = funcNode.Children.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && FuncNodes[func].Count == 1 && func.FunctionType == KnownFuncType.Pow && func.Children[1].LessThenZero());
			});

			if (firstItem == null)
				firstItem = funcNode.Children[0];

			EmitNode(firstItem);

			for (int i = 0; i < funcNode.Children.Count; i++)
			{
				if (funcNode.Children[i] == firstItem)
					continue;

				var func = funcNode.Children[i] as FuncNode;

				if (func != null && FuncNodes[func].Count == 1 && func.FunctionType == KnownFuncType.Pow && func.Children[1].LessThenZero())
				{
					EmitNode(funcNode.Children[i], true);
					IlInstructions.Add(new OpCodeArg(OpCodes.Div));
				}
				else
				{
					EmitNode(funcNode.Children[i]);
					if (IlInstructions[IlInstructions.Count - 1].OpCode == OpCodes.Ldc_R8 && (double)IlInstructions[IlInstructions.Count - 1].Arg == 1.0)
						IlInstructions.RemoveAt(IlInstructions.Count - 1);
					else
						IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
				}
			}

			return true;
		}

		private bool EmitDivFunc(FuncNode funcNode)
		{
			EmitNode(funcNode.Children[0]);
			for (int i = 1; i < funcNode.Children.Count; i++)
			{
				EmitNode(funcNode.Children[i]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Div));
			}

			return true;
		}

		private bool EmitNegFunc(FuncNode funcNode, bool negExpAbs)
		{
			EmitNode(funcNode.Children[0]);

			if (!negExpAbs)
				IlInstructions.Add(new OpCodeArg(OpCodes.Neg));

			return true;
		}

		private bool EmitExpFunc(FuncNode funcNode, bool negExpAbs)
		{
			if ((funcNode.Children[1].Type == MathNodeType.Value && ((ValueNode)funcNode.Children[1]).Value.IsInteger) ||
				(funcNode.Children[1].Type == MathNodeType.Calculated && ((CalculatedNode)funcNode.Children[1]).Value % 1 == 0))
			{
				int powerValue = (int)funcNode.Children[1].DoubleValue;
				int power = Math.Abs(powerValue);
				if (negExpAbs)
					powerValue = power;

				if (powerValue < 0)
					IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));

				EmitNode(funcNode.Children[0]);

				if (power == 1)
				{
				}
				else
					if (power <= 3)
					{
						IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						for (int i = 1; i < power; i++)
						{
							IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
							IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
						}
					}
					else if (power == 4)
					{
						IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
						IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number));
						IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
					}
					else
					{
						// result: funcNode.Number
						// x: funcNode.Number + 1

						//int result = x;
						IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number + 1));
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number + 1));

						power--;
						do
						{
							if ((power & 1) == 1)
							{
								IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number + 1));
								IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
							}

							if (power <= 1)
								break;

							//x = x * x;
							IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number + 1));
							IlInstructions.Add(new OpCodeArg(OpCodes.Ldloc, funcNode.Number + 1));
							IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
							IlInstructions.Add(new OpCodeArg(OpCodes.Stloc, funcNode.Number + 1));

							power = power >> 1;
						}
						while (power != 0);
					}

				if (powerValue < 0)
					IlInstructions.Add(new OpCodeArg(OpCodes.Div));
			}
			else
			{
				var child1 = funcNode.Children[1];
				if ((child1.Type == MathNodeType.Value && ((ValueNode)child1).Value.Abs() == new Rational<long>(1, 2, false)) ||
					(child1.Type == MathNodeType.Calculated && Math.Abs(((CalculatedNode)child1).Value) == 0.5))
				{
					if (!negExpAbs && ((child1.Type == MathNodeType.Value && ((ValueNode)child1).Value < 0) ||
							(child1.Type == MathNodeType.Calculated && ((CalculatedNode)child1).Value < 0)))
					{
						IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
						EmitNode(funcNode.Children[0]);
						IlInstructions.Add(new OpCodeArg(OpCodes.Call, MathFuncAssembly.TypesReferences[KnownFuncType.Sqrt]));
						IlInstructions.Add(new OpCodeArg(OpCodes.Div));
					}
					else
					{
						EmitNode(funcNode.Children[0]);
						IlInstructions.Add(new OpCodeArg(OpCodes.Call, MathFuncAssembly.TypesReferences[KnownFuncType.Sqrt]));
					}
				}
				else
				{
					EmitNode(funcNode.Children[0]);
					if (negExpAbs)
						EmitNode(funcNode.Children[1].Abs());
					else
						EmitNode(funcNode.Children[1]);

					IlInstructions.Add(new OpCodeArg(OpCodes.Call, MathFuncAssembly.TypesReferences[(KnownFuncType)funcNode.FunctionType]));
				}
			}

			return true;
		}

		private bool EmitDiffFunc(FuncNode funcNode)
		{
			var diffFunc = funcNode.Children[0];
			var arg = diffFunc.Children[0];
			bool isUnknownFunc = diffFunc.Type == MathNodeType.Function && !((FuncNode)diffFunc).IsKnown;

			var diff = FuncNodes[(FuncNode)diffFunc];

			// f(x + dx)
			if (isUnknownFunc)
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldarg, diffFunc.ArgNumber));
			EmitNode(arg);
			IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, DerivativeDelta));
			IlInstructions.Add(new OpCodeArg(OpCodes.Add));
			if (isUnknownFunc)
				IlInstructions.Add(new OpCodeArg(OpCodes.Callvirt, MathFuncAssembly.InvokeFuncRef));
			else
				EmitNode(diffFunc);

			// f(x)
			EmitNode(diffFunc);

			IlInstructions.Add(new OpCodeArg(OpCodes.Sub));
			IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0 / DerivativeDelta));
			IlInstructions.Add(new OpCodeArg(OpCodes.Mul));

			return true;
		}

		private bool EmitKnownFunc(FuncNode funcNode)
		{
			MethodReference value;
			if (funcNode.FunctionType != null && MathFuncAssembly.TypesReferences.TryGetValue((KnownFuncType)funcNode.FunctionType, out value))
			{
				foreach (var child in funcNode.Children)
					EmitNode(child);

				IlInstructions.Add(new OpCodeArg(OpCodes.Call, value));

				return true;
			}
			else if (funcNode.FunctionType == KnownFuncType.Cot)
			{
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
				EmitNode(funcNode.Children[0]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Tan]));
				IlInstructions.Add(new OpCodeArg(OpCodes.Div));

				return true;
			}
			else if (funcNode.FunctionType == KnownFuncType.Arccot)
			{
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
				EmitNode(funcNode.Children[0]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Div));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Arctan]));

				return true;
			}
			else if (funcNode.FunctionType == KnownFuncType.Arcsinh)
			{
				EmitNode(funcNode.Children[0]);
				EmitNode(funcNode.Children[0]);
				EmitNode(funcNode.Children[0]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
				IlInstructions.Add(new OpCodeArg(OpCodes.Add));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Sqrt]));
				IlInstructions.Add(new OpCodeArg(OpCodes.Add));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Log]));

				return true;
			}
			else if (funcNode.FunctionType == KnownFuncType.Arcosh)
			{
				EmitNode(funcNode.Children[0]);
				EmitNode(funcNode.Children[0]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
				IlInstructions.Add(new OpCodeArg(OpCodes.Add));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Sqrt]));
				EmitNode(funcNode.Children[0]);
				IlInstructions.Add(new OpCodeArg(OpCodes.Ldc_R8, 1.0));
				IlInstructions.Add(new OpCodeArg(OpCodes.Sub));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Sqrt]));
				IlInstructions.Add(new OpCodeArg(OpCodes.Mul));
				IlInstructions.Add(new OpCodeArg(OpCodes.Add));
				IlInstructions.Add(new OpCodeArg(OpCodes.Call, KnownFunc.TypesMethods[KnownFuncType.Log]));

				return true;
			}
			else
				return false;
		}

		private bool EmitUnknownFunc(FuncNode funcNode)
		{
			IlInstructions.Add(new OpCodeArg(OpCodes.Ldarg, funcNode.ArgNumber));

			foreach (var child in funcNode.Children)
				EmitNode(child);

			IlInstructions.Add(new OpCodeArg(OpCodes.Callvirt, MathFuncAssembly.InvokeFuncRef));

			return true;
		}

		#endregion

		#region Optimization

		private void OptimizeLocalVariables(ref int localVarNumber)
		{
			List<List<VariableLifetimeCycle>> localVariablesLifeCycles = new List<List<VariableLifetimeCycle>>(localVarNumber);
			for (int i = 0; i < localVarNumber; i++)
				localVariablesLifeCycles.Add(new List<VariableLifetimeCycle>());

			for (int i = 0; i < IlInstructions.Count; i++)
				if (IlInstructions[i].OpCode == OpCodes.Stloc)
					localVariablesLifeCycles[(int)IlInstructions[i].Arg].Add(new VariableLifetimeCycle(i, i));
				else if (IlInstructions[i].OpCode == OpCodes.Ldloc)
				{
					var cycles = localVariablesLifeCycles[(int)IlInstructions[i].Arg];
					cycles[cycles.Count - 1] = new VariableLifetimeCycle(cycles[cycles.Count - 1].BeginInd, i);
				}

			for (int i = 1; i < localVariablesLifeCycles.Count; i++)
				for (int k = 0; k < i; k++)
					for (int j = 0; j < localVariablesLifeCycles[i].Count; j++)
						if (!IsIntersect(localVariablesLifeCycles[i][j], localVariablesLifeCycles[k]))
						{
							IlInstructions[localVariablesLifeCycles[i][j].BeginInd] = new OpCodeArg(OpCodes.Stloc, k);
							for (int l = localVariablesLifeCycles[i][j].BeginInd + 1; l <= localVariablesLifeCycles[i][j].EndInd; l++)
								if (IlInstructions[l].OpCode == OpCodes.Ldloc && (int)IlInstructions[l].Arg == i)
									IlInstructions[l] = new OpCodeArg(OpCodes.Ldloc, k);

							localVariablesLifeCycles[k].Add(localVariablesLifeCycles[i][j]);
							localVariablesLifeCycles[i].RemoveAt(j);
						}

			for (int i = localVariablesLifeCycles.Count - 1; i >= 0; i--)
				if (localVariablesLifeCycles[i].Count == 0)
					localVariablesLifeCycles.RemoveAt(i);
				else
					break;

			localVarNumber = localVariablesLifeCycles.Count;
		}
		
		private bool IsIntersect(VariableLifetimeCycle cycle1, List<VariableLifetimeCycle> cycles)
		{
			foreach (var cycle in cycles)
				if (IsIntersect(cycle1, cycle))
					return true;
			return false;
		}

		private bool IsIntersect(VariableLifetimeCycle cycle1, VariableLifetimeCycle cycle2)
		{
			if (cycle1.BeginInd >= cycle2.BeginInd && cycle1.BeginInd <= cycle2.EndInd)
				return true;

			if (cycle1.EndInd >= cycle2.BeginInd && cycle1.EndInd <= cycle2.EndInd)
				return true;

			// inside.
			if (cycle2.BeginInd >= cycle1.BeginInd && cycle2.BeginInd <= cycle1.EndInd)
				return true;

			return false;
		}

		private void OptimizeInstructions()
		{
			int i = 0;
			while (i < IlInstructions.Count)
			{
				if (i < IlInstructions.Count - 1)
				{
					var firstOpCode = IlInstructions[i].OpCode;

					if ((firstOpCode == OpCodes.Ldarg || firstOpCode == OpCodes.Ldc_R8) && IlInstructions[i + 1].OpCode == OpCodes.Stloc)
					{
						var locNumber = (int)IlInstructions[i + 1].Arg;

						for (int j = i + 2; j < IlInstructions.Count; j++)
						{
							if ((IlInstructions[j].OpCode == OpCodes.Stloc) && (int)IlInstructions[j].Arg == locNumber)
								break;

							if ((IlInstructions[j].OpCode == OpCodes.Ldloc) && (int)IlInstructions[j].Arg == locNumber)
								IlInstructions[j] = new OpCodeArg(firstOpCode, IlInstructions[i].Arg);
						}

						IlInstructions.RemoveRange(i, 2);
						continue;
					}

					if (firstOpCode == OpCodes.Ldloc && IlInstructions[i + 1].OpCode == OpCodes.Stloc)
					{
						if ((int)IlInstructions[i].Arg == (int)IlInstructions[i + 1].Arg)
						{
							IlInstructions.RemoveRange(i, 2);
							continue;
						}
						else
						{
							var locNumber1 = (int)IlInstructions[i].Arg;
							var locNumber2 = (int)IlInstructions[i + 1].Arg;

							bool arg2IsUsed = false;
							for (int j = i + 2; j < IlInstructions.Count; j++)
							{
								if ((IlInstructions[j].OpCode == OpCodes.Stloc) && (int)IlInstructions[j].Arg == locNumber1)
									break;

								if ((IlInstructions[j].OpCode == OpCodes.Stloc) && (int)IlInstructions[j].Arg == locNumber2)
									break;

								if ((IlInstructions[j].OpCode == OpCodes.Ldloc) && (int)IlInstructions[j].Arg == locNumber2)
								{
									IlInstructions[j] = new OpCodeArg(OpCodes.Ldloc, locNumber1);
									arg2IsUsed = true;
								}
							}

							if (arg2IsUsed)
							{
								IlInstructions.RemoveRange(i, 2);
								continue;
							}
						}
					}

					if (firstOpCode == OpCodes.Stloc && IlInstructions[i + 1].OpCode == OpCodes.Ldloc &&
						(int)IlInstructions[i].Arg == (int)IlInstructions[i + 1].Arg)
					{
						var locNumber = (int)IlInstructions[i].Arg;

						bool remove = true;
						for (int j = i + 2; j < IlInstructions.Count; j++)
							if ((IlInstructions[j].OpCode == OpCodes.Ldloc) && (int)IlInstructions[j].Arg == locNumber)
							{
								remove = false;
								break;
							}

						if (remove)
						{
							IlInstructions.RemoveRange(i, 2);
							continue;
						}
					}
				}

				i++;
			}
		}

		#endregion

		#region Emitting to Mono.Cecil
		
		private static void EmitInstruction(ILProcessor ilProcessor, OpCodeArg instr, bool staticFunc)
		{
			if (instr.Arg == null)
				ilProcessor.Emit(instr.OpCode);
			else if (instr.Arg is int)
			{
				if (instr.OpCode == OpCodes.Ldarg)
					EmitArgLoad(ilProcessor, (int)instr.Arg + (staticFunc ? 0 : 1));
				else if (instr.OpCode == OpCodes.Ldloc)
					EmitLocalLoad(ilProcessor, (int)instr.Arg);
				else if (instr.OpCode == OpCodes.Stloc)
					EmitLocalSave(ilProcessor, (int)instr.Arg);
				else
					ilProcessor.Emit(instr.OpCode, (int)instr.Arg);
			}
			else if (instr.Arg is double)
				ilProcessor.Emit(instr.OpCode, (double)instr.Arg);
			else if (instr.Arg is float)
				ilProcessor.Emit(instr.OpCode, (float)instr.Arg);
			else if (instr.Arg is string)
				ilProcessor.Emit(instr.OpCode, (string)instr.Arg);
			else if (instr.Arg is byte)
				ilProcessor.Emit(instr.OpCode, (byte)instr.Arg);
			else if (instr.Arg is sbyte)
				ilProcessor.Emit(instr.OpCode, (sbyte)instr.Arg);
			else if (instr.Arg is CallSite)
				ilProcessor.Emit(instr.OpCode, (CallSite)instr.Arg);
			else if (instr.Arg is FieldReference)
				ilProcessor.Emit(instr.OpCode, (FieldReference)instr.Arg);
			else if (instr.Arg is Instruction)
				ilProcessor.Emit(instr.OpCode, (Instruction)instr.Arg);
			else if (instr.Arg is Instruction[])
				ilProcessor.Emit(instr.OpCode, (Instruction[])instr.Arg);
			else if (instr.Arg is MethodReference)
				ilProcessor.Emit(instr.OpCode, (MethodReference)instr.Arg);
			else if (instr.Arg is ParameterDefinition)
				ilProcessor.Emit(instr.OpCode, (ParameterDefinition)instr.Arg);
			else if (instr.Arg is TypeReference)
				ilProcessor.Emit(instr.OpCode, (TypeReference)instr.Arg);
			else if (instr.Arg is VariableDefinition)
				ilProcessor.Emit(instr.OpCode, (VariableDefinition)instr.Arg);
		}

		public static void EmitArgLoad(ILProcessor ilProcessor, int argNumber)
		{
			switch (argNumber)
			{
				case 0:
					ilProcessor.Emit(OpCodes.Ldarg_0);
					break;
				case 1:
					ilProcessor.Emit(OpCodes.Ldarg_1);
					break;
				case 2:
					ilProcessor.Emit(OpCodes.Ldarg_2);
					break;
				case 3:
					ilProcessor.Emit(OpCodes.Ldarg_3);
					break;
				default:
					if (argNumber < 256)
						ilProcessor.Emit(OpCodes.Ldarg_S, (byte)argNumber);
					else
						ilProcessor.Emit(OpCodes.Ldarg, (ushort)argNumber);
					break;
			}
		}

		public static void EmitLocalLoad(ILProcessor ilProcessor, int localNumber)
		{
			switch (localNumber)
			{
				case 0:
					ilProcessor.Emit(OpCodes.Ldloc_0);
					break;
				case 1:
					ilProcessor.Emit(OpCodes.Ldloc_1);
					break;
				case 2:
					ilProcessor.Emit(OpCodes.Ldloc_2);
					break;
				case 3:
					ilProcessor.Emit(OpCodes.Ldloc_3);
					break;
				default:
					if (localNumber < 256)
						ilProcessor.Emit(OpCodes.Ldloc_S, (byte)localNumber);
					else
						ilProcessor.Emit(OpCodes.Ldloc, (ushort)localNumber);
					break;
			}
		}

		public static void EmitLocalSave(ILProcessor ilProcessor, int localNumber)
		{
			switch (localNumber)
			{
				case 0:
					ilProcessor.Emit(OpCodes.Stloc_0);
					break;
				case 1:
					ilProcessor.Emit(OpCodes.Stloc_1);
					break;
				case 2:
					ilProcessor.Emit(OpCodes.Stloc_2);
					break;
				case 3:
					ilProcessor.Emit(OpCodes.Stloc_3);
					break;
				default:
					if (localNumber < 256)
						ilProcessor.Emit(OpCodes.Stloc_S, (byte)localNumber);
					else
						ilProcessor.Emit(OpCodes.Stloc, (ushort)localNumber);
					break;
			}
		}

		#endregion
	}
}
