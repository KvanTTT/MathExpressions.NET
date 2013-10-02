using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;

namespace MathFunctions
{
	public class KnownFunc
	{
		public static Dictionary<string, KnownFuncType> UnaryNamesFuncs = new Dictionary<string, KnownFuncType>()
		{
			{ "-", KnownFuncType.Neg},
			{ "neg", KnownFuncType.Neg},

			{ "sqrt", KnownFuncType.Sqrt},

			{ "sin", KnownFuncType.Sin},
			{ "cos", KnownFuncType.Cos},
			{ "tan", KnownFuncType.Tan},
			{ "cot", KnownFuncType.Cot},

			{ "arcsin", KnownFuncType.Arcsin},
			{ "arccos", KnownFuncType.Arccos},
			{ "arctan", KnownFuncType.Arctan},
			{ "arccot", KnownFuncType.Arccot},

			{ "sinh", KnownFuncType.Sinh},
			{ "cosh", KnownFuncType.Cosh},

			{ "arcsinh", KnownFuncType.Arcsinh},
			{ "arcosh", KnownFuncType.Arcosh},

			{ "ln", KnownFuncType.Ln},
			{ "log10", KnownFuncType.Log10},

			{ "abs", KnownFuncType.Abs},
			{ "sgn", KnownFuncType.Sgn},
			{ "trunc", KnownFuncType.Trunc},
			{ "round", KnownFuncType.Round}
		};

		public static Dictionary<string, KnownFuncType> BinaryNamesFuncs = new Dictionary<string, KnownFuncType>()
		{
			{ "+", KnownFuncType.Add},
			{ "add", KnownFuncType.Add},
			{ "-", KnownFuncType.Sub },
			{ "sub", KnownFuncType.Sub },
			{ "*", KnownFuncType.Mult},
			{ "mult", KnownFuncType.Mult},
			{ "/", KnownFuncType.Div},
			{ "div", KnownFuncType.Div},
			{ "^", KnownFuncType.Exp},
			{ "exp", KnownFuncType.Exp},
			
			{ "log", KnownFuncType.Log},
			{ "diff", KnownFuncType.Diff}
		};

		public static KnownFuncType[] AddKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub };

		public static KnownFuncType[] MultKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub, KnownFuncType.Mult, KnownFuncType.Div };

		public static KnownFuncType[] ExpKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub, KnownFuncType.Mult, KnownFuncType.Div, KnownFuncType.Exp };

		public static Dictionary<KnownFuncType, string> UnaryFuncsNames = new Dictionary<KnownFuncType, string>();

		public static Dictionary<KnownFuncType, string> BinaryFuncsNames = new Dictionary<KnownFuncType, string>();

		public static Dictionary<KnownFuncType, MethodInfo> TypesMethods;

		static KnownFunc()
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

			
			TypesMethods = new Dictionary<KnownFuncType, MethodInfo>()
			{
				{ KnownFuncType.Sin, typeof(Math).GetMethod("Sin", new Type[] { typeof(double) }) },
				{ KnownFuncType.Cos, typeof(Math).GetMethod("Cos", new Type[] { typeof(double) }) },
				{ KnownFuncType.Tan, typeof(Math).GetMethod("Tan", new Type[] { typeof(double) }) },

				{ KnownFuncType.Arcsin, typeof(Math).GetMethod("Asin", new Type[] { typeof(double) }) },
				{ KnownFuncType.Arccos, typeof(Math).GetMethod("Acos", new Type[] { typeof(double) }) },
				{ KnownFuncType.Arctan, typeof(Math).GetMethod("Atan", new Type[] { typeof(double) }) },

				{ KnownFuncType.Sinh, typeof(Math).GetMethod("Sinh", new Type[] { typeof(double) }) },
				{ KnownFuncType.Cosh, typeof(Math).GetMethod("Cosh", new Type[] { typeof(double) }) },

				{ KnownFuncType.Ln, typeof(Math).GetMethod("Log", new Type[] { typeof(double) }) },
				{ KnownFuncType.Log10, typeof(Math).GetMethod("Log10", new Type[] { typeof(double) }) },

				{ KnownFuncType.Abs, typeof(Math).GetMethod("Abs", new Type[] { typeof(double) }) },
				{ KnownFuncType.Sgn, typeof(Math).GetMethod("Sign", new Type[] { typeof(double) }) },
				{ KnownFuncType.Trunc, typeof(Math).GetMethod("Truncate", new Type[] { typeof(double) }) },
				{ KnownFuncType.Round, typeof(Math).GetMethod("Round", new Type[] { typeof(double) }) },

				{ KnownFuncType.Exp, typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) }) },
				{ KnownFuncType.Sqrt, typeof(Math).GetMethod("Sqrt", new Type[] { typeof(double) }) },
				{ KnownFuncType.Log, typeof(Math).GetMethod("Log", new Type[] { typeof(double), typeof(double) }) },
			};
		}
	}
}
