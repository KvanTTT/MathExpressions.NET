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
				for (int i = 0; i < funcNode.Children.Count; i++)
					funcNode.Children[i] = Simplify(funcNode.Children[i]);

				switch (funcNode.FunctionType)
				{
					case KnownFuncType.Neg:
						if (funcNode.Children[0] is ValueNode valueNode)
							return new ValueNode(-valueNode.Value);
						else if (funcNode.Children[0] is FuncNode firstChildFuncNode && firstChildFuncNode.FunctionType == KnownFuncType.Neg)
							return funcNode.Children[0].Children[0];
						break;

					case KnownFuncType.Add:
						BuildMultichildTree(funcNode);
						ReduceSummands(funcNode);
						return FoldValues(funcNode);

					case KnownFuncType.Mult:
						BuildMultichildTree(funcNode);
						var addResult = BreakOnAddNodes(funcNode);
						var multResult = MultValues(funcNode);
						bool isNeg = multResult is FuncNode && ((FuncNode)multResult).FunctionType == KnownFuncType.Neg;
						var powerNode = ReducePowers(isNeg ? multResult.Children[0] : multResult);
						powerNode.Children = powerNode.Children.Except(powerNode.Children.Where(child =>
							child is ValueNode childValueNode && childValueNode.Value == 1)).ToList();

						return addResult != null && addResult.NodeCount <= (isNeg ? powerNode.NodeCount + 1 : powerNode.NodeCount) ?
							addResult :
								(isNeg ? (powerNode is ValueNode powerValueNode ?
									(MathFuncNode)(new ValueNode(-powerValueNode.Value)) : new FuncNode(KnownFuncType.Neg, powerNode)) : powerNode);

					case KnownFuncType.Pow:
						if (funcNode.Children[0] is FuncNode childFuncNode)
						{
							if (childFuncNode.FunctionType == KnownFuncType.Pow)
							{
								funcNode.Children[1] = Simplify(
									new FuncNode(KnownFuncType.Mult, funcNode.Children[0].Children[1], funcNode.Children[1]));
								funcNode.Children[0] = funcNode.Children[0].Children[0];
								return ExpValue(funcNode);
							}
							else if (childFuncNode.FunctionType == KnownFuncType.Mult)
							{
								var expResult = ExpValue(funcNode);
								var multNode = PowerIntoMult(funcNode.Children[0].Children, funcNode.Children[1]);
								return (multNode.NodeCount <= expResult.NodeCount) ? multNode : expResult;
							}
						}

						return ExpValue(funcNode);

					/*case KnownFuncType.Diff:
						return Simplify(GetDerivative(funcNode.Children[0]));*/

					default:
						if (funcNode.Children.All(child => child is ValueNode childValueNode))
							return (MathFuncNode)SimplifyValues(funcNode.FunctionType, funcNode.Children.Select(child => (ValueNode)child).ToList())
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
			var values = funcNode.Children
				.Where(child => child is ValueNode)
				.Select(valueChild => ((ValueNode)valueChild).Value);

			Rational<long> result = 0;
			foreach (Rational<long> value in values)
				result += value;

			var notValuesNodes = funcNode.Children
				.Where(child => !(child is ValueNode)).ToList();

			if (result == 0)
			{
				return
					notValuesNodes.Count == 0 ? new ValueNode(0) :
					notValuesNodes.Count == 1 ? notValuesNodes.First() :
					new FuncNode(KnownFuncType.Add, notValuesNodes.ToList());
			}
			else
			{
				if (notValuesNodes.Count == 0)
				{
					return new ValueNode(result);
				}
				else
				{
					notValuesNodes.Add(new ValueNode(result));
					return new FuncNode(KnownFuncType.Add, notValuesNodes);
				}
			}
		}

		private void BuildMultichildTree(FuncNode beginNode)
		{
			var newChildren = new List<MathFuncNode>();
			BuildMultichildTree(beginNode, beginNode, newChildren, false);
			beginNode.Children = newChildren;
		}

		private void BuildMultichildTree(FuncNode beginNode, FuncNode funcNode, List<MathFuncNode> newChildren, bool neg)
		{
			foreach (var child in funcNode.Children)
			{
				var childFuncNode = child as FuncNode;
				if (childFuncNode != null)
				{
					if (childFuncNode.FunctionType == beginNode.FunctionType)
					{
						BuildMultichildTree(beginNode, childFuncNode, newChildren, neg);
						continue;
					}
					else if (childFuncNode.FunctionType == KnownFuncType.Neg)
					{
						BuildMultichildTree(beginNode, childFuncNode, newChildren, !neg);
						continue;
					}
				}

				if (neg && (beginNode.FunctionType == KnownFuncType.Add ||
					(beginNode.FunctionType == KnownFuncType.Mult && child == funcNode.Children.First())))
				{
					if (child is ValueNode valueNode)
						newChildren.Add(new ValueNode(-valueNode.Value));
					else
						newChildren.Add(new FuncNode(KnownFuncType.Neg, child));
				}
				else
					newChildren.Add(child);
			}
		}

		private MathFuncNode ReduceAddition(MathFuncNode node1, MathFuncNode node2)
		{
			var node1neg = node1 is FuncNode funcNode1 && funcNode1.FunctionType == KnownFuncType.Neg;
			var node2neg = node2 is FuncNode funcNode2 && funcNode2.FunctionType == KnownFuncType.Neg;
			var node11 = node1neg ? node1.Children[0] : node1;
			var node21 = node2neg ? node2.Children[0] : node2;

			MathFuncNode valueNode1 = null;
			if (node11 is FuncNode funcNode11 && funcNode11.FunctionType == KnownFuncType.Mult)
				valueNode1 = node11.Children.Where(child => child.IsValueOrCalculated).FirstOrDefault();
			if (valueNode1 == null)
				valueNode1 = node11.IsValueOrCalculated ? node11 : new ValueNode(new Rational<long>(1, 1));
			var value1 = ((ValueNode)valueNode1).Value;
			if (node1neg)
				value1 *= -1;

			MathFuncNode valueNode2 = null;
			if (node21 is FuncNode funcNode21 && funcNode21.FunctionType == KnownFuncType.Mult)
				valueNode2 = node21.Children.Where(child => child.IsValueOrCalculated).FirstOrDefault();
			if (valueNode2 == null)
				valueNode2 = node21.IsValueOrCalculated ? node21 : new ValueNode(new Rational<long>(1, 1));
			var value2 = ((ValueNode)valueNode2).Value;
			if (node2neg)
				value2 *= -1;

			var notValueNodes1 = node11 is FuncNode funcNode111 &&
				funcNode111.FunctionType == KnownFuncType.Mult ?
				node11.Children.Where(child => !child.IsValueOrCalculated).ToList() :
				node11.IsValueOrCalculated ?
				new List<MathFuncNode>() { } :
				new List<MathFuncNode>() { node11 };
			var notValueNodes2 = node21 is FuncNode funcNode211 &&
				funcNode211.FunctionType == KnownFuncType.Mult ?
				node21.Children.Where(child => !child.IsValueOrCalculated).ToList() :
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
			while (i < funcNode.Children.Count)
			{
				int j = i + 1;
				while (j < funcNode.Children.Count)
				{
					var newNode = ReduceAddition(funcNode.Children[i], funcNode.Children[j]);
					if (newNode != null)
					{
						funcNode.Children[i] = newNode;
						funcNode.Children.RemoveAt(j);
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
			var addNodes = funcNode.Children.Where(child =>
						child is FuncNode childFuncNode &&
						childFuncNode.FunctionType == KnownFuncType.Add).ToList();

			if (addNodes.Count != 0)
			{
				var funcNodesExceptAddNodes = funcNode.Children.Except(addNodes);
				var ar = new int[addNodes.Count];
				var lengthAr = addNodes.Select(addNode => addNode.Children.Count).ToArray();

				var newAddChildren = new List<MathFuncNode>();
				var newChildren = new List<MathFuncNode>();

				do
				{
					newAddChildren.Clear();
					newAddChildren.AddRange(funcNodesExceptAddNodes);
					for (int j = 0; j < ar.Length; j++)
						newAddChildren.Add(addNodes[j].Children[ar[j]]);

					newChildren.Add(Simplify(new FuncNode(KnownFuncType.Mult, newAddChildren)));
				}
				while (IncArray(ar, lengthAr));

				return Simplify(new FuncNode(KnownFuncType.Add, newChildren));
			}
			else
			{
				return null;
			}
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
			var values = funcNode.Children
				.Where(child => child is ValueNode)
				.Cast<ValueNode>()
				.Select(valueChild => valueChild.Value);

			values = values.Concat(funcNode.Children
				.Where(child => child is FuncNode childFuncNode && childFuncNode.FunctionType == KnownFuncType.Neg)
				.Select(valueChild => new Rational<long>(-1, 1)));

			Rational<long> result = 1;
			foreach (var value in values)
				result *= value;

			if (result == 0)
				return new ValueNode(0);

			var notValuesNodes = funcNode.Children
				.Where(child => !(child is ValueNode) && !(child is FuncNode childFuncNode && childFuncNode.FunctionType == KnownFuncType.Neg))
				.Concat(funcNode.Children
				.Where(child => child is FuncNode childFuncNode && childFuncNode.FunctionType == KnownFuncType.Neg)
				.Select(negChild => negChild.Children[0])).ToList();

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
			if (!(funcNode is FuncNode))
				return funcNode;

			var newChildren = new List<MathFuncNode>();
			bool[] markedNodes = new bool[funcNode.Children.Count];

			for (int i = 0; i < funcNode.Children.Count; i++)
			{
				var basis = UnderPowerExpr(funcNode.Children[i]);
				var nodesWithSameBasis = new List<MathFuncNode>();
				if (!markedNodes[i])
					nodesWithSameBasis.Add(funcNode.Children[i]);

				for (int j = i + 1; j < funcNode.Children.Count; j++)
					if (basis.Equals(UnderPowerExpr(funcNode.Children[j])) && !markedNodes[j])
					{
						nodesWithSameBasis.Add(funcNode.Children[j]);
						markedNodes[j] = true;
					}

				if (nodesWithSameBasis.Count > 1)
				{
					newChildren.Add(Simplify(new FuncNode(KnownFuncType.Pow,
						basis,
						Simplify(new FuncNode(KnownFuncType.Add,
							nodesWithSameBasis.Select(node => PowerExpr(node)).ToList()))
					)));
				}
				else if (!markedNodes[i])
					newChildren.Add(funcNode.Children[i]);
			}

			if (funcNode.Children.Count != newChildren.Count)
			{
				if (newChildren.Count == 1)
					return newChildren[0];
				else
					return new FuncNode(KnownFuncType.Mult, newChildren);
			}
			else
				return funcNode;
		}
		#endregion

		#region Exp

		private MathFuncNode ExpValue(FuncNode funcNode)
		{
			var bValue = funcNode.Children[1] as ValueNode;
			if (bValue != null)
			{
				if (bValue.Value == 0)
					return new ValueNode(1);
				else if (bValue.Value == 1)
					return funcNode.Children[0];
				/*else if (funcNode.Children[0].Type == MathNodeType.Value)
				{
					double a = funcNode.Children[0].Value.ToDouble();
					double b = bValue.Value.ToDouble();
					double r = b == 0.5 ? Math.Sqrt(a) : Math.Pow(a, b);
					Rational<long> result;
					Rational<long>.FromDecimal((decimal)r, out result);
					return new ValueNode(result);
				}*/
				else if (bValue.Value.IsInteger && funcNode.Children[0] is FuncNode childFuncNode &&
					childFuncNode.FunctionType == KnownFuncType.Neg)
				{
					if (bValue.Value.Numerator % 2 == 0)
						return new FuncNode(KnownFuncType.Pow, funcNode.Children[0].Children[0], new ValueNode(bValue.Value));
					else
						return new FuncNode(KnownFuncType.Neg, new FuncNode(KnownFuncType.Pow, funcNode.Children[0].Children[0], new ValueNode(bValue.Value)));
				}
			}

			if (funcNode.Children[0] is ValueNode aValue)
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
			return (node is FuncNode funcNode && funcNode.FunctionType == KnownFuncType.Pow)
				? node.Children[1] : new ValueNode(1);
		}

		private MathFuncNode PowerIntoMult(IEnumerable<MathFuncNode> multChildren, MathFuncNode expNode)
		{
			var newChildren = multChildren.Select(child => 
				Simplify(new FuncNode(KnownFuncType.Pow, child, expNode)));
			return Simplify(new FuncNode(KnownFuncType.Mult, newChildren.ToList()));
		}
		
		private MathFuncNode UnderPowerExpr(MathFuncNode node)
		{
			return (node is FuncNode funcNode && funcNode.FunctionType == KnownFuncType.Pow) ?
				node.Children[0] : node;
		}

		#endregion

		#endregion
	}
}
