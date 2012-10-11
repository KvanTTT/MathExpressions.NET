using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;

namespace MathFunctions
{
	public class KnownMathFunction
	{
		public static Dictionary<string, KnownMathFunctionType> UnaryNamesFuncs = new Dictionary<string, KnownMathFunctionType>()
		{
			{ "-", KnownMathFunctionType.Neg},
			{ "neg", KnownMathFunctionType.Neg},

			{ "sqrt", KnownMathFunctionType.Sqrt},

			{ "sin", KnownMathFunctionType.Sin},
			{ "cos", KnownMathFunctionType.Cos},
			{ "tan", KnownMathFunctionType.Tan},
			{ "cot", KnownMathFunctionType.Cot},

			{ "arcsin", KnownMathFunctionType.Arcsin},
			{ "arccos", KnownMathFunctionType.Arccos},
			{ "arctan", KnownMathFunctionType.Arctan},
			{ "arccot", KnownMathFunctionType.Arccot},

			{ "sinh", KnownMathFunctionType.Sinh},
			{ "cosh", KnownMathFunctionType.Cosh},

			{ "arcsinh", KnownMathFunctionType.Arcsinh},
			{ "arcosh", KnownMathFunctionType.Arcosh},

			{ "ln", KnownMathFunctionType.Ln},
			{ "log10", KnownMathFunctionType.Log10},

			{ "abs", KnownMathFunctionType.Abs},
			{ "sgn", KnownMathFunctionType.Sgn},
			{ "trunc", KnownMathFunctionType.Trunc},
			{ "round", KnownMathFunctionType.Round}
		};

		public static Dictionary<string, KnownMathFunctionType> BinaryNamesFuncs = new Dictionary<string, KnownMathFunctionType>()
		{
			{ "+", KnownMathFunctionType.Add},
			{ "add", KnownMathFunctionType.Add},
			{ "-", KnownMathFunctionType.Sub },
			{ "sub", KnownMathFunctionType.Sub },
			{ "*", KnownMathFunctionType.Mult},
			{ "mult", KnownMathFunctionType.Mult},
			{ "/", KnownMathFunctionType.Div},
			{ "div", KnownMathFunctionType.Div},
			{ "^", KnownMathFunctionType.Exp},
			{ "exp", KnownMathFunctionType.Exp},
			
			{ "log", KnownMathFunctionType.Log},
			{ "diff", KnownMathFunctionType.Diff}
		};

		public static Dictionary<KnownMathFunctionType, string> UnaryFuncsNames = new Dictionary<KnownMathFunctionType, string>();

		public static Dictionary<KnownMathFunctionType, string> BinaryFuncsNames = new Dictionary<KnownMathFunctionType, string>();

		public static Dictionary<KnownMathFunctionType, MethodInfo> TypesMethods;

		static KnownMathFunction()
		{
			foreach (var unaryNameFunc in UnaryNamesFuncs)
				if (!UnaryFuncsNames.ContainsKey(unaryNameFunc.Value))
					UnaryFuncsNames.Add(unaryNameFunc.Value, unaryNameFunc.Key);
				else if (UnaryFuncsNames[unaryNameFunc.Value].Length > unaryNameFunc.Key.Length)
				{
					UnaryFuncsNames.Remove(unaryNameFunc.Value);
					UnaryFuncsNames.Add(unaryNameFunc.Value, unaryNameFunc.Key);
				}

			foreach (var binaryNameFunc in BinaryNamesFuncs)
				if (!BinaryFuncsNames.ContainsKey(binaryNameFunc.Value))
					BinaryFuncsNames.Add(binaryNameFunc.Value, binaryNameFunc.Key);
				else if (BinaryFuncsNames[binaryNameFunc.Value].Length > binaryNameFunc.Key.Length)
				{
					BinaryFuncsNames.Remove(binaryNameFunc.Value);
					BinaryFuncsNames.Add(binaryNameFunc.Value, binaryNameFunc.Key);
				}

			
			TypesMethods = new Dictionary<KnownMathFunctionType, MethodInfo>()
			{
				{ KnownMathFunctionType.Sin, typeof(Math).GetMethod("Sin", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Cos, typeof(Math).GetMethod("Cos", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Tan, typeof(Math).GetMethod("Tan", new Type[] { typeof(double) }) },

				{ KnownMathFunctionType.Arcsin, typeof(Math).GetMethod("Asin", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Arccos, typeof(Math).GetMethod("Acos", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Arctan, typeof(Math).GetMethod("Atan", new Type[] { typeof(double) }) },

				{ KnownMathFunctionType.Sinh, typeof(Math).GetMethod("Sinh", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Cosh, typeof(Math).GetMethod("Cosh", new Type[] { typeof(double) }) },

				{ KnownMathFunctionType.Ln, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Log10, typeof(Math).GetMethod("Log10", new Type[] { typeof(double) }) },

				{ KnownMathFunctionType.Abs, typeof(Math).GetMethod("Abs", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Sgn, typeof(Math).GetMethod("Sign", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Trunc, typeof(Math).GetMethod("Truncate", new Type[] { typeof(double) }) },
				{ KnownMathFunctionType.Round, typeof(Math).GetMethod("Round", new Type[] { typeof(double) }) },

				{ KnownMathFunctionType.Exp, typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) }) },
				{ KnownMathFunctionType.Log, typeof(Math).GetMethod("Log", new Type[] { typeof(double), typeof(double) }) },
			};
		}
	}
}
