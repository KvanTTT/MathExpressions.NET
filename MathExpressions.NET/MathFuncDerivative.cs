using System.Collections.Generic;
using System.Linq;

namespace MathExpressionsNET
{
	public partial class MathFunc
	{
		public MathFunc GetDerivative()
		{
			var result = Simplify(GetDerivative(Root));
			result.Sort();
			return new MathFunc(result, Variable, Parameters.Select(p => p.Value));
		}

		#region Helpers

		private FuncNode _currentFunc;

		private MathFuncNode GetDerivative(MathFuncNode node)
		{
			switch (node)
			{
				case CalculatedNode calculatedNode:
					return new CalculatedNode(0.0);
				case ValueNode valueNode:
				case ConstNode constNode:
					return new ValueNode(0);
				case VarNode varNode:
					return new ValueNode(1);
				case FuncNode funcNode:
					return GetFuncDerivative(funcNode);
			}
			return null;
		}

		private void GetDerivatives(MathFuncNode root)
		{
			for (int i = 0; i < root.Children.Count; i++)
				if (root.Children[i] is FuncNode funcNode)
					if (funcNode.FunctionType == KnownFuncType.Diff)
						root.Children[i] = GetDerivative(root.Children[i].Children[0]);
					else
						GetDerivatives(root.Children[i]);
		}

		private MathFuncNode GetFuncDerivative(FuncNode funcNode)
		{
			MathFunc value;
			if (funcNode.IsKnown)
			{
				if (funcNode.FunctionType == KnownFuncType.Add ||
					funcNode.FunctionType == KnownFuncType.Sub)
				{
					var newChildren = new List<MathFuncNode>(funcNode.Children.Count);
					for (int i = 0; i < funcNode.Children.Count; i++)
						newChildren.Add(GetDerivative(funcNode.Children[i]));
					return new FuncNode(KnownFuncType.Add, newChildren);
				}
				else if (funcNode.FunctionType == KnownFuncType.Mult)
				{
					var newChildren = new List<MathFuncNode>(funcNode.Children.Count);
					for (int i = 0; i < funcNode.Children.Count; i++)
					{
						var addNode = new List<MathFuncNode>();
						for (int j = 0; j < funcNode.Children.Count; j++)
						{
							if (i == j)
								addNode.Add(GetDerivative(funcNode.Children[i]));
							else
								addNode.Add((MathFuncNode)funcNode.Children[j].Clone());
						}
						newChildren.Add(new FuncNode(KnownFuncType.Mult, addNode));
					}
					return new FuncNode(KnownFuncType.Add, newChildren);
				}
				else if (funcNode.FunctionType == KnownFuncType.Pow)
				{
					if (funcNode.Children[1].IsValueOrCalculated)
					{
						var node1 = funcNode.Children[1] is ValueNode childValueNode1 ?
							(MathFuncNode)new ValueNode(childValueNode1) :
							new CalculatedNode((CalculatedNode)funcNode.Children[1]);
						var node2 = funcNode.Children[1] is ValueNode childValueNode2 ?
							(MathFuncNode)new ValueNode(childValueNode2.Value - 1) :
							new CalculatedNode(((CalculatedNode)funcNode.Children[1]).Value - 1);

						return new FuncNode(KnownFuncType.Mult,
								node1,
								new FuncNode(KnownFuncType.Pow, (MathFuncNode)funcNode.Children[0].Clone(), node2),
								GetDerivative(funcNode.Children[0]));
					}

					if (funcNode.Children[1] is ConstNode constNode)
						return new FuncNode(KnownFuncType.Mult,
								new ConstNode(constNode.Name),
								new FuncNode(KnownFuncType.Pow,
									(MathFuncNode)funcNode.Children[0].Clone(),
									new FuncNode(KnownFuncType.Add, new ConstNode(constNode.Name), new ValueNode(-1))
								),
								GetDerivative(funcNode.Children[0])
							);
				}
				else if (funcNode.FunctionType == KnownFuncType.Diff)
				{
					if (funcNode.Children[0] is FuncNode childFuncNode)
					{
						if (childFuncNode.IsKnown)
						{
							var der = GetDerivative(funcNode.Children[0]);
							if (der.Children[0] is FuncNode && ((FuncNode)der).FunctionType == KnownFuncType.Diff)
								return new FuncNode(KnownFuncType.Diff, der, Variable);
							else
								return GetDerivative(der);
						}
						else
							return new FuncNode(KnownFuncType.Diff, GetDerivative(funcNode.Children[0]), Variable);
					}
					else
						return new ValueNode(0);
				}
				else if (funcNode.FunctionType == KnownFuncType.Neg)
				{
					return new FuncNode(KnownFuncType.Neg, GetDerivative(funcNode.Children[0]));
				}
			}

			if (Helper.Derivatives.TryGetValue(funcNode.Name, out value))
			{
				var sub = value;
				var subNode = MakeSubstitution(sub.LeftNode.Children[0], sub.RightNode, funcNode);
				GetDerivatives(subNode);
				return subNode;
			}
			else
			{
				return new FuncNode(KnownFuncType.Mult,
					new FuncNode(KnownFuncType.Diff, (MathFuncNode)funcNode.Clone(), Variable),
					GetDerivative(funcNode.Children[0]));
			}
		}

		private MathFuncNode MakeSubstitution(MathFuncNode left, MathFuncNode right, FuncNode currentFunc)
		{
			LeftNode = left;
			RightNode = right;
			_currentFunc = currentFunc;
			return MakeSubstitution(right);
		}

		private MathFuncNode MakeSubstitution(MathFuncNode node)
		{
			int ind = -1;
			for (int j = 0; j < LeftNode.Children.Count; j++)
				if (LeftNode.Children[j].Name == node.Name)
				{
					ind = j;
					break;
				}
			if (ind != -1)
			{
				if (_currentFunc.Children[ind].IsTerminal)
					return _currentFunc.Children[ind];
				else
					return new FuncNode((FuncNode)_currentFunc.Children[ind]);
			}

			MathFuncNode result = null;
			switch (node)
			{
				case CalculatedNode calculatedNode:
					result = new CalculatedNode(calculatedNode.Value);
					break;
				case ValueNode valueNode:
					result = new ValueNode(valueNode);
					break;
				case ConstNode constNode:
				case VarNode varNode:
					result = node;
					break;
				case FuncNode funcNode:
					result = funcNode.FunctionType != null ?
						new FuncNode((KnownFuncType)funcNode.FunctionType) :
						new FuncNode(node.Name);
					break;
			}

			for (int i = 0; i < node.Children.Count; i++)
				switch (node.Children[i])
				{
					case CalculatedNode calculatedNode:
						result.Children.Add(new CalculatedNode(calculatedNode));
						break;
					case ValueNode valueNode:
						result.Children.Add(new ValueNode(valueNode));
						break;
					case ConstNode constNode:
					case VarNode varNode:
						result.Children.Add(node.Children[i]);
						break;
					case FuncNode funcNode:
						result.Children.Add(MakeSubstitution(funcNode));
						break;
				}
			return result;
		}

		#endregion
	}
}
