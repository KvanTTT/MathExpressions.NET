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
			switch (node.Type)
			{
				case MathNodeType.Calculated:
					return new CalculatedNode(0.0);
				case MathNodeType.Value:
				case MathNodeType.Constant:
					return new ValueNode(0);
				case MathNodeType.Variable:
					return new ValueNode(1);
				case MathNodeType.Function:
					return GetFuncDerivative((FuncNode)node);
			}
			return null;
		}

		private void GetDerivatives(MathFuncNode root)
		{
			for (int i = 0; i < root.Children.Count; i++)
				if (root.Children[i].Type == MathNodeType.Function)
					if (((FuncNode)root.Children[i]).FunctionType == KnownFuncType.Diff)
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
						var node1 = funcNode.Children[1].Type == MathNodeType.Value ? (MathFuncNode)
							new ValueNode((ValueNode)funcNode.Children[1]) :
							new CalculatedNode((CalculatedNode)funcNode.Children[1]);
						var node2 = funcNode.Children[1].Type == MathNodeType.Value ? (MathFuncNode)
							new ValueNode(((ValueNode)funcNode.Children[1]).Value - 1) :
							new CalculatedNode(((CalculatedNode)funcNode.Children[1]).Value - 1);

						return new FuncNode(KnownFuncType.Mult,
								node1,
								new FuncNode(KnownFuncType.Pow, (MathFuncNode)funcNode.Children[0].Clone(), node2),
								GetDerivative(funcNode.Children[0]));
					}

					var constNode = funcNode.Children[1] as ConstNode;
					if (constNode != null)
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
					if (funcNode.Children[0].Type == MathNodeType.Function)
					{
						if (((FuncNode)funcNode.Children[0]).IsKnown)
						{
							var der = GetDerivative(funcNode.Children[0]);
							if (der.Children[0].Type == MathNodeType.Function && ((FuncNode)der).FunctionType == KnownFuncType.Diff)
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

			MathFuncNode result;
			switch (node.Type)
			{
				case MathNodeType.Calculated:
					result = new CalculatedNode(((CalculatedNode)node).Value);
					break;
				case MathNodeType.Value:
					result = new ValueNode((ValueNode)node);
					break;
				case MathNodeType.Constant:
				case MathNodeType.Variable:
					result = node;
					break;
				default:
				case MathNodeType.Function:
					result = ((FuncNode)node).FunctionType != null ?
						new FuncNode((KnownFuncType)((FuncNode)node).FunctionType) :
						new FuncNode(node.Name);
					break;
			}

			for (int i = 0; i < node.Children.Count; i++)
				switch (node.Children[i].Type)
				{
					case MathNodeType.Calculated:
						result.Children.Add(new CalculatedNode((CalculatedNode)node.Children[i]));
						break;
					case MathNodeType.Value:
						result.Children.Add(new ValueNode((ValueNode)node.Children[i]));
						break;
					case MathNodeType.Constant:
					case MathNodeType.Variable:
						result.Children.Add(node.Children[i]);
						break;
					case MathNodeType.Function:
						result.Children.Add(MakeSubstitution((FuncNode)node.Children[i]));
						break;
				}
			return result;
		}

		#endregion
	}
}
