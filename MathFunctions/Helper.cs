using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public static class Helper
	{
		public static MathExprParser Parser;
		public static Dictionary<KnownMathFunctionType, MathFunc> Derivatives;
		public static List<MathFunc> Simplications;
		public static List<MathFunc> Permutations;

		static Helper()
		{
			Parser = new MathExprParser();
		}

		public static void InitDerivatives(string str)
		{
			Parser.Parse(str);

			Derivatives = new Dictionary<KnownMathFunctionType, MathFunc>();

			foreach (var statement in Parser.Statements)
			{
				var funcNode = statement.LeftNode.Childs[0].Name;
				if (KnownMathFunction.UnaryNamesFuncs.ContainsKey(funcNode))
					Derivatives.Add(KnownMathFunction.UnaryNamesFuncs[funcNode], statement);
				else if (KnownMathFunction.BinaryNamesFuncs.ContainsKey(funcNode))
					Derivatives.Add(KnownMathFunction.BinaryNamesFuncs[funcNode], statement);
			}
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
