using Antlr4.Runtime;
using System;

namespace MathExpressionsNET
{
	public class ShouldNotBeVisitedException : Exception
	{
		private readonly string _nodeDescription;

		public ShouldNotBeVisitedException(ParserRuleContext context)
		{
			_nodeDescription = MathExprParser.ruleNames[context.RuleIndex];
		}

		public override string ToString()
		{
			return $"Node `{_nodeDescription}` should not be visited.";
		}
	}
}
