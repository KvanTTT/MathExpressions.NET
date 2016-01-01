using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressionsNET
{
	public partial class MathFunc
	{
		public MathFunc GetPrecompilied()
		{
			var result = Precompile(null, Root);
			result.Sort();
			return new MathFunc(result, Variable, Parameters.Select(p => p.Value));
		}

		public bool ContainsNaN()
		{
			return ContainsNaNHelper(Root);
		}

		#region Helpers

		private bool ContainsNaNHelper(MathFuncNode node)
		{
			for (int i = 0; i < node.Childs.Count; i++)
				if (ContainsNaNHelper(node.Childs[i]))
					return true;

			CalculatedNode calcNode = node as CalculatedNode;
			if (calcNode != null && double.IsNaN(calcNode.DoubleValue))
				return true;
			else
				return false;
		}

		private MathFuncNode Precompile(MathFuncNode parent, MathFuncNode node)
		{
			if (node.Type == MathNodeType.Value)
				return new CalculatedNode(((ValueNode)node).Value);
			else if (node.Type == MathNodeType.Function)
			{
				for (int i = 0; i < node.Childs.Count; i++)
					node.Childs[i] = Precompile(node, node.Childs[i]);

				FuncNode func = (FuncNode)node;
				switch (func.FunctionType)
				{
					case KnownFuncType.Add:
						node = PrecompileAddFunc(func);
						break;

					case KnownFuncType.Mult:
						node = PrecompileMultFunc(func);
						break;

					case KnownFuncType.Pow:
						node = PrecompileExpFunc(parent, func);
						break;
				}

				if (node.Childs.Count > 0 && node.Childs.All(child => child.Type == MathNodeType.Value || child.Type == MathNodeType.Calculated))
					return (MathFuncNode)CalculateValues(((FuncNode)node).FunctionType, node.Childs) ?? (MathFuncNode)node;
				else
					return node;
			}
			else
				return node;
		}

		private MathFuncNode PrecompileAddFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem;
			MathFuncNode result = null;

			var funcNode2 = FoldCalculatedSummands(funcNode);

			firstItem = funcNode2.Childs.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && func.LessThenZero());
			});

			if (firstItem == null)
				firstItem = funcNode2.Childs[0];

			result = firstItem;

			for (int i = 0; i < funcNode2.Childs.Count; i++)
			{
				if (funcNode2.Childs[i] == firstItem)
					continue;

				if (funcNode2.Childs[i].LessThenZero())
					result = new FuncNode(KnownFuncType.Sub, result, funcNode2.Childs[i].Abs());
				else
					result = new FuncNode(KnownFuncType.Add, result, funcNode2.Childs[i]);
			}

			return result;
		}

		private MathFuncNode PrecompileMultFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem;
			MathFuncNode result = null;

			var funcNode2 = MultCalculatedFactors(funcNode);
			//var funcNode2 = funcNode;

			firstItem = funcNode2.Childs.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && func.FunctionType == KnownFuncType.Pow && func.Childs[1].LessThenZero());
			});

			if (firstItem == null)
			{
				firstItem = funcNode2.Childs[0];
				result = PrecompileExpFunc(null, (FuncNode)funcNode2.Childs[0]);
			}
			else
				result = firstItem;

			for (int i = 0; i < funcNode2.Childs.Count; i++)
			{
				if (funcNode2.Childs[i] == firstItem)
					continue;

				FuncNode funcChildNode = funcNode2.Childs[i] as FuncNode;
				if (funcChildNode != null && funcChildNode.FunctionType == KnownFuncType.Pow && funcChildNode.Childs[1].LessThenZero())
				{
					if (!funcChildNode.Childs[1].IsValueOrCalculated || funcChildNode.Childs[1].DoubleValue != -1.0)
					{
						FuncNode second;
						if (!funcChildNode.Childs[1].IsValueOrCalculated)
							second = new FuncNode(KnownFuncType.Pow, funcChildNode.Childs[0], funcChildNode.Childs[1].Abs());
						else
							second = Math.Abs(funcChildNode.Childs[1].DoubleValue) != 0.5 ?
								new FuncNode(KnownFuncType.Pow, funcChildNode.Childs[0], funcChildNode.Childs[1].Abs()) :
								new FuncNode(KnownFuncType.Sqrt, funcChildNode.Childs[0]);
						result = new FuncNode(KnownFuncType.Div, result, second);
					}
					else
						result = new FuncNode(KnownFuncType.Div, result, funcChildNode.Childs[0]);
				}
				else
					result = new FuncNode(KnownFuncType.Mult, result, funcNode2.Childs[i]);
			}

			return result;
		}

		private FuncNode PrecompileExpFunc(MathFuncNode parent, FuncNode funcNode)
		{
			if ((parent == null ||
				((FuncNode)parent).FunctionType != KnownFuncType.Mult) &&
				funcNode.Childs[1].LessThenZero())
			{
				if (!funcNode.Childs[1].IsValueOrCalculated || funcNode.Childs[1].DoubleValue != -1.0)
				{
					FuncNode second;
					if (!funcNode.Childs[1].IsValueOrCalculated)
						second = new FuncNode(KnownFuncType.Pow, funcNode.Childs[0], funcNode.Childs[1].Abs());
					else
						second = Math.Abs(funcNode.Childs[1].DoubleValue) != 0.5 ?
							new FuncNode(KnownFuncType.Pow, funcNode.Childs[0], funcNode.Childs[1].Abs()) :
							new FuncNode(KnownFuncType.Sqrt, funcNode.Childs[0]);
					return new FuncNode(KnownFuncType.Div, new CalculatedNode(1.0), second);
				}
				else
					return new FuncNode(KnownFuncType.Div, new CalculatedNode(1.0), funcNode.Childs[0]);
			}
			if (funcNode.Childs[1].IsValueOrCalculated && funcNode.Childs[1].DoubleValue == 0.5)
				return new FuncNode(KnownFuncType.Sqrt, funcNode.Childs[0]);
			return funcNode;
		}

		private CalculatedNode CalculateValues(KnownFuncType? funcType, IList<MathFuncNode> args)
		{
			double result;
			switch (funcType)
			{
				case KnownFuncType.Add:
					result = args[0].DoubleValue;
					for (int i = 1; i < args.Count; i++)
						result += args[i].DoubleValue;
					return new CalculatedNode(result);

				case KnownFuncType.Sub:
					result = args[0].DoubleValue;
					for (int i = 1; i < args.Count; i++)
						result -= args[i].DoubleValue;
					return new CalculatedNode(result);

				case KnownFuncType.Mult:
					result = args[0].DoubleValue;
					for (int i = 1; i < args.Count; i++)
						result *= args[i].DoubleValue;
					return new CalculatedNode(result);

				case KnownFuncType.Div:
					result = args[0].DoubleValue;
					for (int i = 1; i < args.Count; i++)
						result /= args[i].DoubleValue;
					return new CalculatedNode(result);

				case KnownFuncType.Pow:
					if (args[1].DoubleValue == 0.5)
						return new CalculatedNode(Math.Sqrt(args[0].DoubleValue));
					else
						return new CalculatedNode(Math.Pow(args[0].DoubleValue, args[1].DoubleValue));

				case KnownFuncType.Neg:
					return new CalculatedNode(-args[0].DoubleValue);

				case KnownFuncType.Sgn:
					return new CalculatedNode((double)Math.Sign(args[0].DoubleValue));

				case KnownFuncType.Trunc:
					return new CalculatedNode(Math.Truncate(args[0].DoubleValue));

				case KnownFuncType.Round:
					return new CalculatedNode(Math.Round(args[0].DoubleValue));

				case KnownFuncType.Diff:
					return new CalculatedNode(0.0);

				case KnownFuncType.Sqrt:
					return new CalculatedNode(Math.Sqrt(args[0].DoubleValue));

				case KnownFuncType.Sin:
					return new CalculatedNode(Math.Sin(args[0].DoubleValue));

				case KnownFuncType.Cos:
					return new CalculatedNode(Math.Cos(args[0].DoubleValue));

				case KnownFuncType.Tan:
					return new CalculatedNode(Math.Tan(args[0].DoubleValue));

				case KnownFuncType.Cot:
					return new CalculatedNode(1 / Math.Tan(args[0].DoubleValue));

				case KnownFuncType.Arcsin:
					return new CalculatedNode(Math.Asin(args[0].DoubleValue));

				case KnownFuncType.Arccos:
					return new CalculatedNode(Math.Acos(args[0].DoubleValue));

				case KnownFuncType.Arctan:
					return new CalculatedNode(Math.Atan(args[0].DoubleValue));

				case KnownFuncType.Arccot:
					return new CalculatedNode(Math.PI / 2 - Math.Atan(args[0].DoubleValue));

				case KnownFuncType.Sinh:
					return new CalculatedNode(Math.Sinh(args[0].DoubleValue));

				case KnownFuncType.Cosh:
					return new CalculatedNode(Math.Cosh(args[0].DoubleValue));

				case KnownFuncType.Arcsinh:
					return new CalculatedNode(Math.Log(args[0].DoubleValue + Math.Sqrt(args[0].DoubleValue * args[0].DoubleValue + 1)));

				case KnownFuncType.Arcosh:
					return new CalculatedNode(Math.Log(args[0].DoubleValue + Math.Sqrt(args[0].DoubleValue * args[0].DoubleValue - 1)));

				case KnownFuncType.Ln:
					return new CalculatedNode(Math.Log(args[0].DoubleValue));

				case KnownFuncType.Log10:
					return new CalculatedNode(Math.Log10(args[0].DoubleValue));

				case KnownFuncType.Log:
					return new CalculatedNode(Math.Log(args[0].DoubleValue, args[1].DoubleValue));

				case KnownFuncType.Abs:
					return new CalculatedNode(Math.Abs(args[0].DoubleValue));

				default:
					return null;
			}
		}

		private FuncNode FoldCalculatedSummands(FuncNode sum)
		{
			var result = sum.Childs
				.Where(child => child.Type == MathNodeType.Calculated || child.Type == MathNodeType.Value)
				.Select(summand => summand.DoubleValue)
				.Aggregate(0.0, (t, factor) => t += factor);

			if (result != 0.0)
			{
				var newChilds = new List<MathFuncNode>() { new CalculatedNode(result) };
				newChilds.AddRange(sum.Childs.Where(c => c.Type != MathNodeType.Calculated && c.Type != MathNodeType.Value));

				return new FuncNode(KnownFuncType.Sub, newChilds);
			}
			else
				return sum;
		}

		private FuncNode MultCalculatedFactors(FuncNode mult)
		{
			var result = mult.Childs
				.Where(child => child.Type == MathNodeType.Calculated || child.Type == MathNodeType.Value)
				.Select(factor => factor.DoubleValue)
				.Aggregate(1.0, (t, factor) => t *= factor);

			if (result != 1.0)
			{
				var newChilds = new List<MathFuncNode>() { new CalculatedNode(result) };
				newChilds.AddRange(mult.Childs.Where(c => c.Type != MathNodeType.Calculated && c.Type != MathNodeType.Value));

				return new FuncNode(KnownFuncType.Mult, newChilds);
			}
			else
				return mult;
		}


		#endregion
	}
}
