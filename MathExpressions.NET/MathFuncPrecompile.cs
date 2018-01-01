using System;
using System.Collections.Generic;
using System.Linq;

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
			for (int i = 0; i < node.Children.Count; i++)
				if (ContainsNaNHelper(node.Children[i]))
					return true;

			if (node is CalculatedNode calcNode && double.IsNaN(calcNode.DoubleValue))
				return true;
			else
				return false;
		}

		private MathFuncNode Precompile(MathFuncNode parent, MathFuncNode node)
		{
			if (node is ValueNode valueNode)
			{
				return new CalculatedNode(valueNode.Value);
			}
			else if (node is FuncNode func)
			{
				for (int i = 0; i < node.Children.Count; i++)
					node.Children[i] = Precompile(node, node.Children[i]);

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

				if (node.Children.Count > 0 && node.Children.All(child =>
					child is ValueNode || child is CalculatedNode))
				{
					return CalculateValues(func.FunctionType, node.Children) ?? node;
				}
				else
				{
					return node;
				}
			}
			else
				return node;
		}

		private MathFuncNode PrecompileAddFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem;
			MathFuncNode result = null;

			var funcNode2 = FoldCalculatedSummands(funcNode);

			firstItem = funcNode2.Children.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && func.LessThenZero());
			});

			if (firstItem == null)
				firstItem = funcNode2.Children[0];

			result = firstItem;

			for (int i = 0; i < funcNode2.Children.Count; i++)
			{
				if (funcNode2.Children[i] == firstItem)
					continue;

				if (funcNode2.Children[i].LessThenZero())
					result = new FuncNode(KnownFuncType.Sub, result, funcNode2.Children[i].Abs());
				else
					result = new FuncNode(KnownFuncType.Add, result, funcNode2.Children[i]);
			}

			return result;
		}

		private MathFuncNode PrecompileMultFunc(FuncNode funcNode)
		{
			MathFuncNode firstItem;
			MathFuncNode result = null;

			var funcNode2 = MultCalculatedFactors(funcNode);
			//var funcNode2 = funcNode;

			firstItem = funcNode2.Children.FirstOrDefault(node =>
			{
				var func = node as FuncNode;
				return !(func != null && func.FunctionType == KnownFuncType.Pow && func.Children[1].LessThenZero());
			});

			if (firstItem == null)
			{
				firstItem = funcNode2.Children[0];
				result = PrecompileExpFunc(null, (FuncNode)funcNode2.Children[0]);
			}
			else
				result = firstItem;

			for (int i = 0; i < funcNode2.Children.Count; i++)
			{
				if (funcNode2.Children[i] == firstItem)
					continue;

				if (funcNode2.Children[i] is FuncNode funcChildNode && funcChildNode.FunctionType == KnownFuncType.Pow && funcChildNode.Children[1].LessThenZero())
				{
					if (!funcChildNode.Children[1].IsValueOrCalculated || funcChildNode.Children[1].DoubleValue != -1.0)
					{
						FuncNode second;
						if (!funcChildNode.Children[1].IsValueOrCalculated)
							second = new FuncNode(KnownFuncType.Pow, funcChildNode.Children[0], funcChildNode.Children[1].Abs());
						else
							second = Math.Abs(funcChildNode.Children[1].DoubleValue) != 0.5 ?
								new FuncNode(KnownFuncType.Pow, funcChildNode.Children[0], funcChildNode.Children[1].Abs()) :
								new FuncNode(KnownFuncType.Sqrt, funcChildNode.Children[0]);
						result = new FuncNode(KnownFuncType.Div, result, second);
					}
					else
						result = new FuncNode(KnownFuncType.Div, result, funcChildNode.Children[0]);
				}
				else
					result = new FuncNode(KnownFuncType.Mult, result, funcNode2.Children[i]);
			}

			return result;
		}

		private FuncNode PrecompileExpFunc(MathFuncNode parent, FuncNode funcNode)
		{
			if ((parent == null ||
				((FuncNode)parent).FunctionType != KnownFuncType.Mult) &&
				funcNode.Children[1].LessThenZero())
			{
				if (!funcNode.Children[1].IsValueOrCalculated || funcNode.Children[1].DoubleValue != -1.0)
				{
					FuncNode second;
					if (!funcNode.Children[1].IsValueOrCalculated)
						second = new FuncNode(KnownFuncType.Pow, funcNode.Children[0], funcNode.Children[1].Abs());
					else
						second = Math.Abs(funcNode.Children[1].DoubleValue) != 0.5 ?
							new FuncNode(KnownFuncType.Pow, funcNode.Children[0], funcNode.Children[1].Abs()) :
							new FuncNode(KnownFuncType.Sqrt, funcNode.Children[0]);
					return new FuncNode(KnownFuncType.Div, new CalculatedNode(1.0), second);
				}
				else
					return new FuncNode(KnownFuncType.Div, new CalculatedNode(1.0), funcNode.Children[0]);
			}
			if (funcNode.Children[1].IsValueOrCalculated && funcNode.Children[1].DoubleValue == 0.5)
				return new FuncNode(KnownFuncType.Sqrt, funcNode.Children[0]);
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
			var result = sum.Children
				.Where(child => child is CalculatedNode || child is ValueNode)
				.Select(summand => summand.DoubleValue)
				.Aggregate(0.0, (t, factor) => t += factor);

			if (result != 0.0)
			{
				var newChildren = new List<MathFuncNode>() { new CalculatedNode(result) };
				newChildren.AddRange(sum.Children.Where(c => !(c is CalculatedNode) && !(c is ValueNode)));

				return new FuncNode(KnownFuncType.Sub, newChildren);
			}
			else
				return sum;
		}

		private FuncNode MultCalculatedFactors(FuncNode mult)
		{
			var result = mult.Children
				.Where(child => child is CalculatedNode || child is ValueNode)
				.Select(factor => factor.DoubleValue)
				.Aggregate(1.0, (t, factor) => t *= factor);

			if (result != 1.0)
			{
				var newChildren = new List<MathFuncNode>() { new CalculatedNode(result) };
				newChildren.AddRange(mult.Children.Where(c => !(c is CalculatedNode) && !(c is ValueNode)));

				return new FuncNode(KnownFuncType.Mult, newChildren);
			}
			else
				return mult;
		}


		#endregion
	}
}
