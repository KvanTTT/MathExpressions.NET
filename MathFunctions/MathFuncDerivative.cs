using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace MathFunctions
{
	public partial class MathFunc
	{
		public MathFunc GetDerivative()
		{
			var result = Simplify(GetDerivative(Root));
			result.Sort();
			return new MathFunc(result);
		}

		#region Helpers

		private FuncNode _currentFunc;

		private MathFuncNode GetDerivative(MathFuncNode node)
		{
			switch (node.Type)
			{
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
					if (((FuncNode)root.Childs[i]).FunctionType == KnownMathFunctionType.Diff)
						root.Childs[i] = GetDerivative(root.Childs[i].Childs[0]);
					else
						GetDerivatives(root.Childs[i]);
		}

		private MathFuncNode GetFuncDerivative(FuncNode funcNode)
		{
			if (funcNode.IsKnown)
			{
				if (funcNode.FunctionType == KnownMathFunctionType.Add ||
					funcNode.FunctionType == KnownMathFunctionType.Sub)
				{
					var newChilds = new List<MathFuncNode>(funcNode.Childs.Count);
					for (int i = 0; i < funcNode.Childs.Count; i++)
						newChilds.Add(GetDerivative(funcNode.Childs[i]));
					return new FuncNode(KnownMathFunctionType.Add, newChilds);
				}
				else if (funcNode.FunctionType == KnownMathFunctionType.Mult)
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
						newChilds.Add(new FuncNode(KnownMathFunctionType.Mult, addNode));
					}
					return new FuncNode(KnownMathFunctionType.Add, newChilds);
				}
				else if (funcNode.FunctionType == KnownMathFunctionType.Exp)
				{
					var valueNode = funcNode.Childs[1] as ValueNode;
					if (valueNode != null)
					{
						return new FuncNode(KnownMathFunctionType.Mult,
								new ValueNode(valueNode.Value),
								new FuncNode(KnownMathFunctionType.Exp,
									(MathFuncNode)funcNode.Childs[0].Clone(),
									new ValueNode(valueNode.Value - 1)),
								GetDerivative(funcNode.Childs[0]));
					}

					var constNode = funcNode.Childs[1] as ConstNode;
					if (constNode != null)
						return new FuncNode(KnownMathFunctionType.Mult, 
								new ConstNode(constNode.Name),
								new FuncNode(KnownMathFunctionType.Exp, 
									(MathFuncNode)funcNode.Childs[0].Clone(),
									new FuncNode(KnownMathFunctionType.Add, new ConstNode(constNode.Name), new ValueNode(-1))
								),
								GetDerivative(funcNode.Childs[0])
							);
				}
				else if (funcNode.FunctionType == KnownMathFunctionType.Diff)
					return GetDerivative(GetDerivative(funcNode.Childs[0]));

				var sub = Helper.Derivatives[(KnownMathFunctionType)funcNode.FunctionType];
				var subNode = MakeSubstitution(sub.LeftNode.Childs[0], sub.RightNode, funcNode);
				GetDerivatives(subNode);
				return subNode;
			}
			else
			{
				return new FuncNode(KnownMathFunctionType.Mult,
					new FuncNode(KnownMathFunctionType.Diff, (MathFuncNode)funcNode.Clone()),
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
						new FuncNode((KnownMathFunctionType)((FuncNode)node).FunctionType) :
						new FuncNode(node.Name);
					break;
			}

			for (int i = 0; i < node.Childs.Count; i++)
				switch (node.Childs[i].Type)
				{
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
