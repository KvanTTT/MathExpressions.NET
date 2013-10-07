using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public partial class MathFunc
	{
		public MathFunc GetPrecompilied()
		{
			var result = RationalToDouble(Root);
			return new MathFunc(result, Variable, Parameters.Select(p => p.Value));
		}

		#region Helpers

		private MathFuncNode RationalToDouble(MathFuncNode node)
		{
			if (node.Type == MathNodeType.Value)
				return new CalculatedNode(((ValueNode)node).Value);
			else if (node.Type == MathNodeType.Function)
			{
				for (int i = 0; i < node.Childs.Count; i++)
					node.Childs[i] = RationalToDouble(node.Childs[i]);

				if (node.Childs.All(child => child.Type == MathNodeType.Value || child.Type == MathNodeType.Calculated))
					return (MathFuncNode)CalculateValues(((FuncNode)node).FunctionType, node.Childs) ?? (MathFuncNode)node;
				else
					return node;
			}
			else
				return node;
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

				case KnownFuncType.Exp:
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

		#endregion
	}
}
