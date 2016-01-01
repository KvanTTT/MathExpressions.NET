using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;

namespace MathExpressionsNET
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

			{ "asin", KnownFuncType.Arcsin},
			{ "acos", KnownFuncType.Arccos},
			{ "atan", KnownFuncType.Arctan},
			{ "acot", KnownFuncType.Arccot},

			{ "sinh", KnownFuncType.Sinh},
			{ "cosh", KnownFuncType.Cosh},

			{ "asinh", KnownFuncType.Arcsinh},
			{ "acosh", KnownFuncType.Arcosh},

			{ "exp", KnownFuncType.Exp },
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
			{ "^", KnownFuncType.Pow},
			{ "pow", KnownFuncType.Pow},
			
			{ "log", KnownFuncType.Log },
			{ "diff", KnownFuncType.Diff }
		};

		public static List<KnownFuncType> SpecFuncs = new List<KnownFuncType>()
		{
			KnownFuncType.Abs, KnownFuncType.Sgn, KnownFuncType.Trunc, KnownFuncType.Round, KnownFuncType.Diff
		};

		public static KnownFuncType[] AddKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub 
		};

		public static KnownFuncType[] MultKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub, KnownFuncType.Mult, KnownFuncType.Div
		};

		public static KnownFuncType[] ExpKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub, KnownFuncType.Mult, KnownFuncType.Div, KnownFuncType.Pow
		};

		public static KnownFuncType[] NegKnownFuncs = new KnownFuncType[] {
			KnownFuncType.Add, KnownFuncType.Sub, KnownFuncType.Mult, KnownFuncType.Div, KnownFuncType.Pow, KnownFuncType.Neg
		};

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

			Type mathType = typeof(Math);
			Type[] unaryFuncArgTypes = new Type[] { typeof(double) };
			Type[] binaryFuncArgTypes = new Type[] { typeof(double), typeof(double) };

			TypesMethods = new Dictionary<KnownFuncType, MethodInfo>()
			{
				{ KnownFuncType.Sin, mathType.GetMethod("Sin", unaryFuncArgTypes) },
				{ KnownFuncType.Cos, mathType.GetMethod("Cos", unaryFuncArgTypes) },
				{ KnownFuncType.Tan, mathType.GetMethod("Tan", unaryFuncArgTypes) },

				{ KnownFuncType.Arcsin, mathType.GetMethod("Asin", unaryFuncArgTypes) },
				{ KnownFuncType.Arccos, mathType.GetMethod("Acos", unaryFuncArgTypes) },
				{ KnownFuncType.Arctan, mathType.GetMethod("Atan", unaryFuncArgTypes) },

				{ KnownFuncType.Sinh, mathType.GetMethod("Sinh", unaryFuncArgTypes) },
				{ KnownFuncType.Cosh, mathType.GetMethod("Cosh", unaryFuncArgTypes) },

				{ KnownFuncType.Ln, mathType.GetMethod("Log", unaryFuncArgTypes) },
				{ KnownFuncType.Log10, mathType.GetMethod("Log10", unaryFuncArgTypes) },

				{ KnownFuncType.Abs, mathType.GetMethod("Abs", unaryFuncArgTypes) },
				{ KnownFuncType.Sgn, mathType.GetMethod("Sign", unaryFuncArgTypes) },
				{ KnownFuncType.Trunc, mathType.GetMethod("Truncate", unaryFuncArgTypes) },
				{ KnownFuncType.Round, mathType.GetMethod("Round", unaryFuncArgTypes) },

				{ KnownFuncType.Exp, mathType.GetMethod("Exp", unaryFuncArgTypes)},
				{ KnownFuncType.Pow, mathType.GetMethod("Pow", binaryFuncArgTypes) },
				{ KnownFuncType.Sqrt, mathType.GetMethod("Sqrt", unaryFuncArgTypes) },
				{ KnownFuncType.Log, mathType.GetMethod("Log", binaryFuncArgTypes) },
			};

			foreach (KnownFuncType knownFuncType in Enum.GetValues(typeof(KnownFuncType)))
				if (!MultKnownFuncs.Contains(knownFuncType) && knownFuncType != KnownFuncType.Neg && !TypesMethods.ContainsKey(knownFuncType))
				{
					//throw new Exception("Not all Known func defined");
				}
					
		}
	}
}
