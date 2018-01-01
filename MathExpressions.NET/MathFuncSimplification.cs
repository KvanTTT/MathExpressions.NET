using System.Collections.Generic;
using System.Linq;

namespace MathExpressionsNET
{
	public partial class MathFunc
	{
		public MathFunc Simplify()
		{
			var result = Simplify(Root);
			result.Sort();
			return new MathFunc(result, Variable, Parameters.Select(p => p.Value));
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
					case KnownFuncType.Neg:
						if (funcNode.Childs[0].Type == MathNodeType.Value)
							return new ValueNode(-((ValueNode)funcNode.Childs[0]).Value);
						else if (funcNode.Childs[0].Type == MathNodeType.Function && ((FuncNode)funcNode.Childs[0]).FunctionType == KnownFuncType.Neg)
							return funcNode.Childs[0].Childs[0];
						break;

					case KnownFuncType.Add:
						BuildMultichildTree(funcNode);
						ReduceSummands(funcNode);
						return FoldValues(funcNode);

					case KnownFuncType.Mult:
						BuildMultichildTree(funcNode);
						var addResult = BreakOnAddNodes(funcNode);
						var multResult = MultValues(funcNode);
						bool isNeg = multResult.Type == MathNodeType.Function && ((FuncNode)multResult).FunctionType == KnownFuncType.Neg;
						var powerNode = ReducePowers(isNeg ? multResult.Childs[0] : multResult);
						powerNode.Childs = powerNode.Childs.Except(powerNode.Childs.Where(child => child.Type == MathNodeType.Value &&
							((ValueNode)child).Value == 1)).ToList();

						return addResult != null && addResult.NodeCount <= (isNeg ? powerNode.NodeCount + 1 : powerNode.NodeCount) ?
							addResult :
								(isNeg ? (powerNode.Type == MathNodeType.Value ?
									(MathFuncNode)(new ValueNode(-((ValueNode)powerNode).Value)) : new FuncNode(KnownFuncType.Neg, powerNode)) : powerNode);

					case KnownFuncType.Pow:
						if (funcNode.Childs[0].Type == MathNodeType.Function)
						{
							if ((funcNode.Childs[0] as FuncNode).FunctionType == KnownFuncType.Pow)
							{
								funcNode.Childs[1] = Simplify(
									new FuncNode(KnownFuncType.Mult, funcNode.Childs[0].Childs[1], funcNode.Childs[1]));
								funcNode.Childs[0] = funcNode.Childs[0].Childs[0];
								return ExpValue(funcNode);
							}
							else if ((funcNode.Childs[0] as FuncNode).FunctionType == KnownFuncType.Mult)
							{
								var expResult = ExpValue(funcNode);
								var multNode = PowerIntoMult(funcNode.Childs[0].Childs, funcNode.Childs[1]);
								return (multNode.NodeCount <= expResult.NodeCount) ? multNode : expResult;
							}
						}

						return ExpValue(funcNode);

					/*case KnownFuncType.Diff:
						return Simplify(GetDerivative(funcNode.Childs[0]));*/

					default:
						if (funcNode.Childs.All(child => child.Type == MathNodeType.Value))
							return (MathFuncNode)SimplifyValues(funcNode.FunctionType, funcNode.Childs.Select(child => (ValueNode)child).ToList())
								?? (MathFuncNode)funcNode;
						break;
				}

				return funcNode;
			}

			return node;
		}

		#region Addition

		private MathFuncNode FoldValues(FuncNode funcNode)
		{
			var values = funcNode.Childs
				.Where(child => child.Type == MathNodeType.Value)
				.Select(valueChild => ((ValueNode)valueChild).Value);

			Rational<long> result = 0;
			foreach (var value in values)
				result += value;

			var notValuesNodes = funcNode.Childs
				.Where(child => child.Type != MathNodeType.Value).ToList();

			if (result == 0)
				return
					notValuesNodes.Count == 0 ? new ValueNode(0) :
					notValuesNodes.Count == 1 ? notValuesNodes.First() :
					new FuncNode(KnownFuncType.Add, notValuesNodes.ToList());
			else
			{
				if (notValuesNodes.Count == 0)
					return new ValueNode(result);
				else
				{
					notValuesNodes.Add(new ValueNode(result));
					return new FuncNode(KnownFuncType.Add, notValuesNodes);
				}
			}
		}

		private void BuildMultichildTree(FuncNode beginNode)
		{
			var newChilds = new List<MathFuncNode>();
			BuildMultichildTree(beginNode, beginNode, newChilds, false);
			beginNode.Childs = newChilds;
		}

		private void BuildMultichildTree(FuncNode beginNode, FuncNode funcNode, List<MathFuncNode> newChilds, bool neg)
		{
			foreach (var child in funcNode.Childs)
			{
				var childFuncNode = child as FuncNode;
				if (childFuncNode != null)
				{
					if (childFuncNode.FunctionType == beginNode.FunctionType)
					{
						BuildMultichildTree(beginNode, childFuncNode, newChilds, neg);
						continue;
					}
					else if (childFuncNode.FunctionType == KnownFuncType.Neg)
					{
						BuildMultichildTree(beginNode, childFuncNode, newChilds, !neg);
						continue;
					}
				}

				if (neg && (beginNode.FunctionType == KnownFuncType.Add ||
					(beginNode.FunctionType == KnownFuncType.Mult && child == funcNode.Childs.First())))
				{
					if (child.Type == MathNodeType.Value)
						newChilds.Add(new ValueNode(-((ValueNode)child).Value));
					else
						newChilds.Add(new FuncNode(KnownFuncType.Neg, child));
				}
				else
					newChilds.Add(child);
			}
		}

		private MathFuncNode ReduceAddition(MathFuncNode node1, MathFuncNode node2)
		{
			var node1neg = node1.Type == MathNodeType.Function && (node1 as FuncNode).FunctionType == KnownFuncType.Neg;
			var node2neg = node2.Type == MathNodeType.Function && (node2 as FuncNode).FunctionType == KnownFuncType.Neg;
			var node11 = node1neg ? node1.Childs[0] : node1;
			var node21 = node2neg ? node2.Childs[0] : node2;

			MathFuncNode valueNode1 = null;
			if (node11.Type == MathNodeType.Function &&
				(node11 as FuncNode).FunctionType == KnownFuncType.Mult)
					valueNode1 = node11.Childs.Where(child => child.IsValueOrCalculated).FirstOrDefault();
			if (valueNode1 == null)
				valueNode1 = node11.IsValueOrCalculated ? node11 : new ValueNode(new Rational<long>(1, 1));
			var value1 = ((ValueNode)valueNode1).Value;
			if (node1neg)
				value1 *= -1;

			MathFuncNode valueNode2 = null;
			if (node21.Type == MathNodeType.Function &&
				(node21 as FuncNode).FunctionType == KnownFuncType.Mult)
				valueNode2 = node21.Childs.Where(child => child.IsValueOrCalculated).FirstOrDefault();
			if (valueNode2 == null)
				valueNode2 = node21.IsValueOrCalculated ? node21 : new ValueNode(new Rational<long>(1, 1));
			var value2 = ((ValueNode)valueNode2).Value;
			if (node2neg)
				value2 *= -1;

			var notValueNodes1 = node11.Type == MathNodeType.Function &&
				(node11 as FuncNode).FunctionType == KnownFuncType.Mult ?
				node11.Childs.Where(child => !child.IsValueOrCalculated).ToList() :
				node11.IsValueOrCalculated ?
				new List<MathFuncNode>() { } :
				new List<MathFuncNode>() { node11 };
			var notValueNodes2 = node21.Type == MathNodeType.Function &&
				(node21 as FuncNode).FunctionType == KnownFuncType.Mult ?
				node21.Childs.Where(child => !child.IsValueOrCalculated).ToList() :
				node21.IsValueOrCalculated ?
				new List<MathFuncNode>() { } :
				new List<MathFuncNode>() { node21 };

			var mult1 = new FuncNode(KnownFuncType.Mult, notValueNodes1.ToList());
			var mult2 = new FuncNode(KnownFuncType.Mult, notValueNodes2.ToList());
			mult1.Sort();
			mult2.Sort();

			if (mult1.Equals(mult2))
			{
				var resultNodes = new List<MathFuncNode>();
				resultNodes.Add(new ValueNode(value1 + value2));
				resultNodes.AddRange(notValueNodes1);
				return Simplify(new FuncNode(KnownFuncType.Mult, resultNodes));
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
						((FuncNode)child).FunctionType == KnownFuncType.Add).ToList();

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

					newChilds.Add(Simplify(new FuncNode(KnownFuncType.Mult, newAddChilds)));
				}
				while (IncArray(ar, lengthAr));

				return Simplify(new FuncNode(KnownFuncType.Add, newChilds));
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

		private MathFuncNode MultValues(MathFuncNode funcNode)
		{
			var values = funcNode.Childs
				.Where(child => child.Type == MathNodeType.Value)
				.Select(valueChild => ((ValueNode)valueChild).Value);

			values = values.Concat(funcNode.Childs
				.Where(child => child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownFuncType.Neg)
				.Select(valueChild => new Rational<long>(-1, 1)));

			Rational<long> result = 1;
			foreach (var value in values)
				result *= value;

			if (result == 0)
				return new ValueNode(0);

			var notValuesNodes = funcNode.Childs
				.Where(child => child.Type != MathNodeType.Value && !(child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownFuncType.Neg))
				.Concat(funcNode.Childs
				.Where(child => child.Type == MathNodeType.Function && ((FuncNode)child).FunctionType == KnownFuncType.Neg)
				.Select(negChild => negChild.Childs[0])).ToList();

			if (result == 1)
			{
				return notValuesNodes.Count == 0 ? new ValueNode(1) :
					notValuesNodes.Count == 1 ? notValuesNodes.First() :
					new FuncNode(KnownFuncType.Mult, notValuesNodes);
			}
			else if (result == -1)
			{
				return notValuesNodes.Count == 0 ? (MathFuncNode)(new ValueNode(-1)) :
					notValuesNodes.Count == 1 ? new FuncNode(KnownFuncType.Neg, notValuesNodes.First()) :
					new FuncNode(KnownFuncType.Neg, new FuncNode(KnownFuncType.Mult, notValuesNodes));
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
						return new FuncNode(KnownFuncType.Neg, new FuncNode(KnownFuncType.Mult, notValuesNodes));
					}
					else
					{
						notValuesNodes.Add(new ValueNode(result));
						return new FuncNode(KnownFuncType.Mult, notValuesNodes);
					}
				}
			}
		}

		private MathFuncNode ReducePowers(MathFuncNode funcNode)
		{
			if (funcNode.Type != MathNodeType.Function)
				return funcNode;

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
					newChilds.Add(Simplify(new FuncNode(KnownFuncType.Pow,
						basis,
						Simplify(new FuncNode(KnownFuncType.Add,
							nodesWithSameBasis.Select(node => PowerExpr(node)).ToList()))
					)));
				}
				else if (!markedNodes[i])
					newChilds.Add(funcNode.Childs[i]);
			}

			if (funcNode.Childs.Count != newChilds.Count)
			{
				if (newChilds.Count == 1)
					return newChilds[0];
				else
					return new FuncNode(KnownFuncType.Mult, newChilds);
			}
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
				/*else if (funcNode.Childs[0].Type == MathNodeType.Value)
				{
					double a = funcNode.Childs[0].Value.ToDouble();
					double b = bValue.Value.ToDouble();
					double r = b == 0.5 ? Math.Sqrt(a) : Math.Pow(a, b);
					Rational<long> result;
					Rational<long>.FromDecimal((decimal)r, out result);
					return new ValueNode(result);
				}*/
				else if (bValue.Value.IsInteger && funcNode.Childs[0].Type == MathNodeType.Function &&
					((FuncNode)funcNode.Childs[0]).FunctionType == KnownFuncType.Neg)
				{
					if (bValue.Value.Numerator % 2 == 0)
						return new FuncNode(KnownFuncType.Pow, funcNode.Childs[0].Childs[0], new ValueNode(bValue.Value));
					else
						return new FuncNode(KnownFuncType.Neg, new FuncNode(KnownFuncType.Pow, funcNode.Childs[0].Childs[0], new ValueNode(bValue.Value)));
				}
			}

			var aValue = funcNode.Childs[0] as ValueNode;
			if (aValue != null)
			{
				if (aValue.Value == 0)
					return new ValueNode(0);
				else if (aValue.Value == 1)
					return new ValueNode(1);
				else if (bValue != null)
					return (MathFuncNode)SimplifyValues(KnownFuncType.Pow, new List<ValueNode>() { aValue, bValue }) 
						?? (MathFuncNode)funcNode;
			}

			return funcNode;
		}

		private MathFuncNode PowerExpr(MathFuncNode node)
		{
			return (node.Type == MathNodeType.Function &&
					   ((FuncNode)node).FunctionType == KnownFuncType.Pow) ?
					   node.Childs[1] : new ValueNode(1);
		}

		private MathFuncNode PowerIntoMult(IEnumerable<MathFuncNode> multChilds, MathFuncNode expNode)
		{
			var newChilds = multChilds.Select(child => 
				Simplify(new FuncNode(KnownFuncType.Pow, child, expNode)));
			return Simplify(new FuncNode(KnownFuncType.Mult, newChilds.ToList()));
		}
		
		private MathFuncNode UnderPowerExpr(MathFuncNode node)
		{
			return (node.Type == MathNodeType.Function &&
					   ((FuncNode)node).FunctionType == KnownFuncType.Pow) ?
					   node.Childs[0] : node;
		}
		#endregion

		#endregion
	}
}
