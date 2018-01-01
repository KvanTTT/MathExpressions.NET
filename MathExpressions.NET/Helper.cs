using System.Collections.Generic;
using System.Text;

namespace MathExpressionsNET
{
	public static class Helper
	{
		public static Dictionary<string, MathFunc> Derivatives;
		public static List<MathFunc> Simplications;
		public static List<MathFunc> Permutations;

		public static void InitDerivatives(string str)
		{
			List<MathFunc> mathFuncs = new MathExprConverter().Convert(str);

			Derivatives = new Dictionary<string, MathFunc>();

			foreach (var statement in mathFuncs)
			{
				string funcNodeName = statement.LeftNode.Children[0].Name;
				Derivatives.Add(funcNodeName, statement);
			}
		}

		public static void InitDefaultDerivatives()
		{
			var derivatives = new StringBuilder();
			derivatives.AppendLine("(f(x) / g(x))' = (f(x)' * g(x) + f(x) * g(x)') / g(x)^2;");
			derivatives.AppendLine("(f(x) ^ g(x))' = f(x) ^ g(x) * (f(x)' * g(x) / f(x) + g(x)' * ln(f(x)));");
			derivatives.AppendLine("neg(f(x))' = neg(f(x)');");
			derivatives.AppendLine("sin(f(x))' = cos(f(x)) * f(x)';");
			derivatives.AppendLine("cos(f(x))' = -sin(f(x)) * f(x)';");
			derivatives.AppendLine("tan(f(x))' = f(x)' / cos(f(x)) ^ 2;");
			derivatives.AppendLine("cot(f(x))' = -f(x)' / sin(f(x)) ^ 2;");
			derivatives.AppendLine("asin(f(x))' = f(x)' / sqrt(1 - f(x) ^ 2);");
			derivatives.AppendLine("acos(f(x))' = -f(x)' / sqrt(1 - f(x) ^ 2);");
			derivatives.AppendLine("atan(f(x))' = f(x)' / (1 + f(x) ^ 2);");
			derivatives.AppendLine("acot(f(x))' = -f(x)' / (1 + f(x) ^ 2);");
			derivatives.AppendLine("sinh(f(x))' = f(x)' * cosh(x);");
			derivatives.AppendLine("cosh(f(x))' = f(x)' * sinh(x);");
			derivatives.AppendLine("asinh(f(x))' = f(x)' / sqrt(f(x) ^ 2 + 1);");
			derivatives.AppendLine("acosh(f(x))' = f(x)' / sqrt(f(x) ^ 2 - 1);");
			derivatives.AppendLine("exp(f(x))' = exp(f(x)) * f(x)';");
			derivatives.AppendLine("ln(f(x))' = f(x)' / f(x);");
			derivatives.AppendLine("log(f(x), g(x))' = (ln(f(x)) * g(x)' / g(x) - f(x)' * ln(g(x)) / f(x)) / ln(f(x)) ^ 2;");
			derivatives.AppendLine("abs(f(x))' = 1;");
			derivatives.AppendLine("sgn(f(x))' = 0;");
			derivatives.AppendLine("trunc(f(x))' = 0;");
			derivatives.AppendLine("round(f(x))' = 0;");
			InitDerivatives(derivatives.ToString());
		}

		public static void InitRools(string str)
		{
			List<MathFunc> mathFuncs = new MathExprConverter().Convert(str);

			Simplications = new List<MathFunc>();
			Permutations = new List<MathFunc>();

			foreach (var statement in mathFuncs)
			{
				if (statement.RightNode.NodeCount < statement.LeftNode.NodeCount)
					Simplications.Add(statement);
				else
					Permutations.Add(statement);
			}
		}
	}
}
