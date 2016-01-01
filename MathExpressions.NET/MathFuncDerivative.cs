using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

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
			for (int i = 0; i < root.Childs.Count; i++)
				if (root.Childs[i].Type == MathNodeType.Function)
					if (((FuncNode)root.Childs[i]).FunctionType == KnownFuncType.Diff)
						root.Childs[i] = GetDerivative(root.Childs[i].Childs[0]);
					else
						GetDerivatives(root.Childs[i]);
		}

		private MathFuncNode GetFuncDerivative(FuncNode funcNode)
		{
			MathFunc value;
			if (funcNode.IsKnown)
			{
				if (funcNode.FunctionType == KnownFuncType.Add ||
					funcNode.FunctionType == KnownFuncType.Sub)
				{
					var newChilds = new List<MathFuncNode>(funcNode.Childs.Count);
					for (int i = 0; i < funcNode.Childs.Count; i++)
						newChilds.Add(GetDerivative(funcNode.Childs[i]));
					return new FuncNode(KnownFuncType.Add, newChilds);
				}
				else if (funcNode.FunctionType == KnownFuncType.Mult)
				{
					var newChilds = new List<MathFuncNode>(funcNode.Childs.Count);
					for (int i = 0; i < funcNode.Childs.Count; i++)
					{
						var addNode = new List<MathFuncNode>();
						for (int j = 0; j < funcNode.Childs.Count; j++)
						{
							if (i == j)
								addNode.Add(GetDerivative(funcNode.Childs[i]));
							else
								addNode.Add((MathFuncNode)funcNode.Childs[j].Clone());
						}
						newChilds.Add(new FuncNode(KnownFuncType.Mult, addNode));
					}
					return new FuncNode(KnownFuncType.Add, newChilds);
				}
				else if (funcNode.FunctionType == KnownFuncType.Pow)
				{
					if (funcNode.Childs[1].IsValueOrCalculated)
					{
						var node1 = funcNode.Childs[1].Type == MathNodeType.Value ? (MathFuncNode)
							new ValueNode((ValueNode)funcNode.Childs[1]) :
							new CalculatedNode((CalculatedNode)funcNode.Childs[1]);
						var node2 = funcNode.Childs[1].Type == MathNodeType.Value ? (MathFuncNode)
							new ValueNode(((ValueNode)funcNode.Childs[1]).Value - 1) :
							new CalculatedNode(((CalculatedNode)funcNode.Childs[1]).Value - 1);

						return new FuncNode(KnownFuncType.Mult,
								node1,
								new FuncNode(KnownFuncType.Pow, (MathFuncNode)funcNode.Childs[0].Clone(), node2),
								GetDerivative(funcNode.Childs[0]));
					}

					var constNode = funcNode.Childs[1] as ConstNode;
					if (constNode != null)
						return new FuncNode(KnownFuncType.Mult,
								new ConstNode(constNode.Name),
								new FuncNode(KnownFuncType.Pow,
									(MathFuncNode)funcNode.Childs[0].Clone(),
									new FuncNode(KnownFuncType.Add, new ConstNode(constNode.Name), new ValueNode(-1))
								),
								GetDerivative(funcNode.Childs[0])
							);
				}
				else if (funcNode.FunctionType == KnownFuncType.Diff)
				{
					if (funcNode.Childs[0].Type == MathNodeType.Function)
					{
						if (((FuncNode)funcNode.Childs[0]).IsKnown)
						{
							var der = GetDerivative(funcNode.Childs[0]);
							if (der.Childs[0].Type == MathNodeType.Function && ((FuncNode)der).FunctionType == KnownFuncType.Diff)
								return new FuncNode(KnownFuncType.Diff, der, Variable);
							else
								return GetDerivative(der);
						}
						else
							return new FuncNode(KnownFuncType.Diff, GetDerivative(funcNode.Childs[0]), Variable);
					}
					else
						return new ValueNode(0);
				}
				else if (funcNode.FunctionType == KnownFuncType.Neg)
				{
					return new FuncNode(KnownFuncType.Neg, GetDerivative(funcNode.Childs[0]));
				}
			}

			if (Helper.Derivatives.TryGetValue(funcNode.Name, out value))
			{
				var sub = value;
				var subNode = MakeSubstitution(sub.LeftNode.Childs[0], sub.RightNode, funcNode);
				GetDerivatives(subNode);
				return subNode;
			}
			else
			{
				return new FuncNode(KnownFuncType.Mult,
					new FuncNode(KnownFuncType.Diff, (MathFuncNode)funcNode.Clone(), Variable),
					GetDerivative(funcNode.Childs[0]));
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
			for (int j = 0; j < LeftNode.Childs.Count; j++)
				if (LeftNode.Childs[j].Name == node.Name)
				{
					ind = j;
					break;
				}
			if (ind != -1)
			{
				if (_currentFunc.Childs[ind].IsTerminal)
					return _currentFunc.Childs[ind];
				else
					return new FuncNode((FuncNode)_currentFunc.Childs[ind]);
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

			for (int i = 0; i < node.Childs.Count; i++)
				switch (node.Childs[i].Type)
				{
					case MathNodeType.Calculated:
						result.Childs.Add(new CalculatedNode((CalculatedNode)node.Childs[i]));
						break;
					case MathNodeType.Value:
						result.Childs.Add(new ValueNode((ValueNode)node.Childs[i]));
						break;
					case MathNodeType.Constant:
					case MathNodeType.Variable:
						result.Childs.Add(node.Childs[i]);
						break;
					case MathNodeType.Function:
						result.Childs.Add(MakeSubstitution((FuncNode)node.Childs[i]));
						break;
				}
			return result;
		}

		#endregion
	}
}
