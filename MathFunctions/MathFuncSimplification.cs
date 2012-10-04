using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public partial class MathFunc
	{
		public MathFunc Simplify()
		{
			return new MathFunc(Simplify(Root));
		}

		#region Helpers

		private MathFuncNode Simplify(MathFuncNode node)
		{
			var funcNode = node as FuncNode;
			if (funcNode != null)
			{
				for (int i = 0; i < funcNode.Childs.Count; i++)
					funcNode.Childs[i] = Simplify(funcNode.Childs[i]);

				switch (funcNode.FunctionType)
				{
					case KnownMathFunctionType.Neg:
						if (funcNode.Childs[0].Type == MathNodeType.Value)
							return new ValueNode(-funcNode.Childs[0].Value);
						else
							if (funcNode.Childs.Any(child => child.IsValue))
								return Calculate(funcNode.FunctionType, funcNode.Childs.Select(child => (ValueNode)child).ToList()) ?? funcNode;
						break;

					case KnownMathFunctionType.Add:
						BuildMultichildTree(funcNode);
						ReduceSummands(funcNode);
						return AddValues(funcNode);

					case KnownMathFunctionType.Mult:
						BuildMultichildTree(funcNode);
						var addResult = BreakOnAddNodes(funcNode);
						var powerNode = ReducePowers(funcNode);
						var multResult = MultValues(powerNode);

						bool isNeg = multResult.Type == MathNodeType.Function && ((FuncNode)multResult).FunctionType == KnownMathFunctionType.Neg;
						MathFuncNode negNode = isNeg ? multResult.Childs[0] : multResult;
						return addResult != null && addResult.NodeCount <= (isNeg ? negNode.Childs.Count + 1 : negNode.Childs.Count) ?
							addResult :
							multResult;

					case KnownMathFunctionType.Exp:
						if (funcNode.Childs[0].Type == MathNodeType.Function)
						{
							if ((funcNode.Childs[0] as FuncNode).FunctionType == KnownMathFunctionType.Exp)
							{
								funcNode.Childs[1] = Simplify(
									new FuncNode(KnownMathFunctionType.Mult, funcNode.Childs[0].Childs[1], funcNode.Childs[1]));
								funcNode.Childs[0] = funcNode.Childs[0].Childs[0];
								return ExpValue(funcNode);
							}
							else if ((funcNode.Childs[0] as FuncNode).FunctionType == KnownMathFunctionType.Mult)
							{
								var expResult = ExpValue(funcNode);
								var multNode = PowerIntoMult(funcNode.Childs[0].Childs, funcNode.Childs[1]);
								return (multNode.NodeCount <= expResult.NodeCount) ? multNode : expResult;
							}
						}

						return ExpValue(funcNode);

					default:
						if (funcNode.Childs.Any(child => child.IsValue))
							return Calculate(funcNode.FunctionType, funcNode.Childs.Select(child => (ValueNode)child).ToList()) ?? funcNode;
						break;
				}

				return funcNode;
			}

			return node;
		}

		#region Addition

		private MathFuncNode AddValues(FuncNode funcNode)
		{
			var values = funcNode.Childs
				.Where(child => child.Type == MathNodeType.Value)
				.Select(valueChild => valueChild.Value);

			Rational<long> result = 0;
			foreach (var value in values)
				result += value;

			var notValuesNodes = funcNode.Childs
				.Where(child => child.Type != MathNodeType.Value).ToList();

			if (result == 0.0m)
				return
					notValuesNodes.Count == 0 ? new ValueNode(0) :
					notValuesNodes.Count == 1 ? notValuesNodes.First() :
					new FuncNode(KnownMathFunctionType.Add, notValuesNodes.ToList());
			else
			{
				if (notValuesNodes.Count == 0)
					return new ValueNode(result);
				else
				{
					notValuesNodes.Add(new ValueNode(result));
					return new FuncNode(KnownMathFunctionType.Add, notValuesNodes);
				}
			}
		}

		private void BuildMultichildTree(FuncNode beginNode)
		{
			var newChilds = new List<MathFuncNode>();
			BuildMultichildTree(beginNode, beginNode, newChilds);
			beginNode.Childs = newChilds;
		}

		private void BuildMultichildTree(FuncNode beginNode, FuncNode funcNode, List<MathFuncNode> newChilds)
		{
			foreach (var child in funcNode.Childs)
			{
				var childFuncNode = child as FuncNode;
				if (childFuncNode != null && childFuncNode.FunctionType == beginNode.FunctionType)
					BuildMultichildTree(beginNode, childFuncNode, newChilds);
				else
					newChilds.Add(child);
			}
		}

		private MathFuncNode ReduceAddition(MathFuncNode node1, MathFuncNode node2)
		{
			var valueNodes1 = node1.Type == MathNodeType.Function &&
				(node1 as FuncNode).FunctionType == KnownMathFunctionType.Mult ?
				node1.Childs.Where(child => child.IsValue).ToList() :
				node1.IsValue ?
				new List<MathFuncNode>() { node1 } :
				new List<MathFuncNode>() { new ValueNode(1) };
			var notValueNodes1 = node1.Type == MathNodeType.Function &&
				(node1 as FuncNode).FunctionType == KnownMathFunctionType.Mult ?
				node1.Childs.Where(child => !child.IsValue).ToList() :
				node1.IsValue ?
				new List<MathFuncNode>() { } :
				new List<MathFuncNode>() { node1 };

			var valueNodes2 = node2.Type == MathNodeType.Function &&
				(node2 as FuncNode).FunctionType == KnownMathFunctionType.Mult ?
				node2.Childs.Where(child => child.IsValue).ToList() :
				node2.IsValue ?
				new List<MathFuncNode>() { node2 } :
				new List<MathFuncNode>() { new ValueNode(1) };
			var notValueNodes2 = node2.Type == MathNodeType.Function &&
				(node2 as FuncNode).FunctionType == KnownMathFunctionType.Mult ?
				node2.Childs.Where(child => !child.IsValue).ToList() :
				node2.IsValue ?
				new List<MathFuncNode>() { } :
				new List<MathFuncNode>() { node2 };

			var mult1 = new FuncNode(KnownMathFunctionType.Mult, notValueNodes1.ToList());
			var mult2 = new FuncNode(KnownMathFunctionType.Mult, notValueNodes2.ToList());
			mult1.Sort();
			mult2.Sort();

			if (mult1.Equals(mult2))
			{
				var generalNodes = new List<MathFuncNode>();
				generalNodes.AddRange(valueNodes1);
				generalNodes.AddRange(valueNodes2);
				var resultNodes = new List<MathFuncNode>();
				resultNodes.Add(AddValues(new FuncNode(KnownMathFunctionType.Add, generalNodes)));
				resultNodes.AddRange(notValueNodes1);
				return Simplify(new FuncNode(KnownMathFunctionType.Mult, resultNodes));
			}
			else
				return null;
		}

		private void ReduceSummands(FuncNode funcNode)
		{
			int i = 0;
			while (i < funcNode.Childs.Count)
			{
				int j = i + 1;
				while (j < funcNode.Childs.Count)
				{
					var newNode = ReduceAddition(funcNode.Childs[i], funcNode.Childs[j]);
					if (newNode != null)
					{
						funcNode.Childs[i] = newNode;
						funcNode.Childs.RemoveAt(j);
					}
					else
						j++;
				}
				i++;
			}
		}

		#endregion

		#region Mult

		private MathFuncNode BreakOnAddNodes(FuncNode funcNode)
		{
			var addNodes = funcNode.Childs.Where(child =>
						child.Type == MathNodeType.Function &&
						((FuncNode)child).FunctionType == KnownMathFunctionType.Add).ToList();

			if (addNodes.Count != 0)
			{
				var funcNodesExceptAddNodes = funcNode.Childs.Except(addNodes);
				var ar = new int[addNodes.Count];
				var lengthAr = addNodes.Select(addNode => addNode.Childs.Count).ToArray();

				var newAddChilds = new List<MathFuncNode>();
				var newChilds = new List<MathFuncNode>();

				do
				{
					newAddChilds.Clear();
					newAddChilds.AddRange(funcNodesExceptAddNodes);
					for (int j = 0; j < ar.Length; j++)
						newAddChilds.Add(addNodes[j].Childs[ar[j]]);

					newChilds.Add(Simplify(new FuncNode(KnownMathFunctionType.Mult, newAddChilds)));
				}
				while (IncArray(ar, lengthAr));

				return Simplify(new FuncNode(KnownMathFunctionType.Add, newChilds));
			}
			else
				return null;
		}

		private bool IncArray(int[] ar, int[] lengthAr)
		{
			int c = 1;
			for (int i = 0; i < ar.Length; i++)
			{
				ar[i] += c;
				if (ar[i] == lengthAr[i])
				{
					ar[i] = 0;
					c = 1;
				}
				else
					c = 0;
			}

			return c == 0;
		}

		private MathFuncNode MultValues(FuncNode funcNode)
		{
			var values = funcNode.Childs
				.Where(child => child.Type == MathNodeType.Value)
				.Select(valueChild => valueChild.Value);

			values = values.Union(funcNode.Childs
				.Where(child => child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownMathFunctionType.Neg)
				.Select(valueChild => new Rational<long>(-1, 1)));

			Rational<long> result = 1;
			foreach (var value in values)
				result *= value;

			if (result == 0)
				return new ValueNode(0);

			var notValuesNodes = funcNode.Childs
				.Where(child => child.Type != MathNodeType.Value && !(child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownMathFunctionType.Neg))
				.Union(funcNode.Childs
				.Where(child => child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownMathFunctionType.Neg)
				.Select(negChild => negChild.Childs[0])).ToList();

			if (result == 1.0m)
			{
				return notValuesNodes.Count == 0 ? new ValueNode(1) :
					notValuesNodes.Count == 1 ? notValuesNodes.First() :
					new FuncNode(KnownMathFunctionType.Mult, notValuesNodes);
			}
			else if (result == -1.0m)
			{
				return notValuesNodes.Count == 0 ? (MathFuncNode)(new ValueNode(-1)) :
					notValuesNodes.Count == 1 ? new FuncNode(KnownMathFunctionType.Neg, notValuesNodes.First()) :
					new FuncNode(KnownMathFunctionType.Neg, new FuncNode(KnownMathFunctionType.Mult, notValuesNodes));
			}
			else
			{
				if (notValuesNodes.Count == 0)
					return new ValueNode(result);
				else
				{
					if (result < 0)
					{
						notValuesNodes.Add(new ValueNode(-result));
						return new FuncNode(KnownMathFunctionType.Neg, new FuncNode(KnownMathFunctionType.Mult, notValuesNodes));
					}
					else
					{
						notValuesNodes.Add(new ValueNode(result));
						return new FuncNode(KnownMathFunctionType.Mult, notValuesNodes);
					}
				}
			}
		}

		private FuncNode ReducePowers(FuncNode funcNode)
		{
			var newChilds = new List<MathFuncNode>();
			bool[] markedNodes = new bool[funcNode.Childs.Count];

			for (int i = 0; i < funcNode.Childs.Count; i++)
			{
				var basis = UnderPowerExpr(funcNode.Childs[i]);
				var nodesWithSameBasis = new List<MathFuncNode>();
				if (!markedNodes[i])
					nodesWithSameBasis.Add(funcNode.Childs[i]);

				for (int j = i + 1; j < funcNode.Childs.Count; j++)
					if (basis.Equals(UnderPowerExpr(funcNode.Childs[j])) && !markedNodes[j])
					{
						nodesWithSameBasis.Add(funcNode.Childs[j]);
						markedNodes[j] = true;
					}

				if (nodesWithSameBasis.Count > 1)
				{
					newChilds.Add(Simplify(new FuncNode(KnownMathFunctionType.Exp,
						basis,
						Simplify(new FuncNode(KnownMathFunctionType.Add,
							nodesWithSameBasis.Select(node => PowerExpr(node)).ToList()))
					)));
				}
				else if (!markedNodes[i])
					newChilds.Add(funcNode.Childs[i]);
			}

			if (funcNode.Childs.Count != newChilds.Count)
				return new FuncNode(KnownMathFunctionType.Mult, newChilds);
			else
				return funcNode;
		}
		#endregion

		#region Exp

		private MathFuncNode ExpValue(FuncNode funcNode)
		{
			var bValue = funcNode.Childs[1] as ValueNode;
			if (bValue != null)
			{
				if (bValue.Value == 0)
					return new ValueNode(1);
				else if (bValue.Value == 1)
					return funcNode.Childs[0];
				else if (funcNode.Childs[0].Type == MathNodeType.Value)
					return new ValueNode((decimal)Math.Pow(funcNode.Childs[0].Value.ToDouble(), bValue.Value.ToDouble()));
				else if (bValue.Value.IsInteger && funcNode.Childs[0].Type == MathNodeType.Function &&
					((FuncNode)funcNode.Childs[0]).FunctionType == KnownMathFunctionType.Neg) 
					if (bValue.Value.Numerator % 2 == 0)
						return new FuncNode(KnownMathFunctionType.Exp, funcNode.Childs[0].Childs[0], new ValueNode(bValue.Value));
					else
						return new FuncNode(KnownMathFunctionType.Neg, new FuncNode(KnownMathFunctionType.Exp, funcNode.Childs[0].Childs[0], new ValueNode(bValue.Value)));
			}

			var aValue = funcNode.Childs[0] as ValueNode;
			if (aValue != null)
			{
				if (aValue.Value == 0)
					return new ValueNode(0);
				else if (aValue.Value == 1)
					return new ValueNode(1);
			}

			return funcNode;
		}

		private MathFuncNode PowerExpr(MathFuncNode node)
		{
			return (node.Type == MathNodeType.Function &&
					   ((FuncNode)node).FunctionType == KnownMathFunctionType.Exp) ?
					   node.Childs[1] : new ValueNode(1);
		}

		private MathFuncNode PowerIntoMult(IEnumerable<MathFuncNode> multChilds, MathFuncNode expNode)
		{
			var newChilds = multChilds.Select(child => 
				Simplify(new FuncNode(KnownMathFunctionType.Exp, child, expNode)));
			return Simplify(new FuncNode(KnownMathFunctionType.Mult, newChilds.ToList()));
		}
		
		private MathFuncNode UnderPowerExpr(MathFuncNode node)
		{
			return (node.Type == MathNodeType.Function &&
					   ((FuncNode)node).FunctionType == KnownMathFunctionType.Exp) ?
					   node.Childs[0] : node;
		}
		#endregion

		#endregion
	}
}
