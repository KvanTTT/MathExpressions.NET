using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public static class Helper
	{
		public static MathExprParser Parser;
		public static Dictionary<string, MathFunc> Derivatives;
		public static List<MathFunc> Simplications;
		public static List<MathFunc> Permutations;

		static Helper()
		{
			Parser = new MathExprParser();
		}

		public static void InitDerivatives(string str)
		{
			Parser.Parse(str);

			Derivatives = new Dictionary<string, MathFunc>();

			foreach (var statement in Parser.Statements)
			{
				var funcNodeName = statement.LeftNode.Childs[0].Name;

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
			derivatives.AppendLine("arcsin(f(x))' = f(x)' / sqrt(1 - f(x) ^ 2);");
			derivatives.AppendLine("arccos(f(x))' = -f(x)' / sqrt(1 - f(x) ^ 2);");
			derivatives.AppendLine("arctan(f(x))' = f(x)' / (1 + f(x) ^ 2);");
			derivatives.AppendLine("arccot(f(x))' = -f(x)' / (1 + f(x) ^ 2);");
			derivatives.AppendLine("sinh(f(x))' = f(x)' * cosh(x);");
			derivatives.AppendLine("cosh(f(x))' = f(x)' * sinh(x);");
			derivatives.AppendLine("arcsinh(f(x))' = f(x)' / sqrt(f(x) ^ 2 + 1);");
			derivatives.AppendLine("arcosh(f(x))' = f(x)' / sqrt(f(x) ^ 2 - 1);");
			derivatives.AppendLine("ln(f(x))' = f(x)' / f(x);");
			derivatives.AppendLine("log(f(x), g(x))' = (ln(f(x)) * g(x)' / g(x) - f(x)' * ln(g(x)) / f(x)) / ln(f(x)) ^ 2;");
			InitDerivatives(derivatives.ToString());
		}

		public static void InitRools(string str)
		{
			Parser.Parse(str);

			Simplications = new List<MathFunc>();
			Permutations = new List<MathFunc>();

			foreach (var statement in Parser.Statements)
			{
				if (statement.RightNode.NodeCount < statement.LeftNode.NodeCount)
					Simplications.Add(statement);
				else
					Permutations.Add(statement);
			}
		}
	}
}
