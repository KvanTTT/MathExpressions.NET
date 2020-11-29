using System.Linq;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using static MathExpressionsNET.MathExprParser;

namespace MathExpressionsNET
{
	public class MathExprConverter : IMathExprVisitor<MathFuncNode>
	{
		private List<MathFunc> _matchFuncs;
		private Dictionary<string, ConstNode> _parameters;

		public List<MathFunc> MatchFuncs => _matchFuncs;

		public List<MathFunc> Convert(string input)
		{
			_parameters = new Dictionary<string, ConstNode>();
			_matchFuncs = new List<MathFunc>();

			var inputStream = new AntlrInputStream(input);
			var lexer = new MathExprLexer(inputStream);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new MathExprParser(tokenStream);

			StatementsContext statements = parser.statements();
			Visit(statements);

			return MatchFuncs;
		}

		public MathFuncNode VisitStatements(StatementsContext context)
		{
			MathFuncNode result = null;
			foreach (StatementContext statement in context.statement())
			{
				result = Visit(statement);
			}
			return result;
		}

		public MathFuncNode VisitStatement(StatementContext context)
		{
			MathFuncNode left = Visit(context.expression(0));
			MathFuncNode right = context.expression().Length > 1
				? Visit(context.expression(1))
				: null;
			if (right != null)
			{
				_matchFuncs.Add(new MathFunc(left, right, null, _parameters.Select(p => p.Value)));
			}
			else if (left != null)
			{
				_matchFuncs.Add(new MathFunc(left, null, _parameters.Select(p => p.Value)));
			}
			return left;
		}

		public MathFuncNode VisitUnaryExpression(UnaryExpressionContext context)
		{
			var result = Visit(context.expression());
			if (context.Minus() != null)
			{
				if (result is ValueNode valueNode)
				{
					result = new ValueNode(-valueNode.Value);
				}
				else
				{
					result = new FuncNode(KnownFuncType.Neg, result);
				}
			}
			return result;
		}

		public MathFuncNode VisitBinaryExpression(BinaryExpressionContext context)
		{
			MathFuncNode arg1 = Visit(context.expression(0));
			MathFuncNode arg2 = Visit(context.expression(1));

			KnownFuncType funcType;
			if (context.Plus() != null)
			{
				funcType = KnownFuncType.Add;
			}
			else if (context.Minus() != null)
			{
				funcType = KnownFuncType.Sub;
				var second = new FuncNode(KnownFuncType.Neg, new MathFuncNode[] { arg2 });
				var result = new FuncNode(KnownFuncType.Add, new MathFuncNode[] { arg1, second });
				return result;
			}
			else if (context.Mult() != null)
			{
				funcType = KnownFuncType.Mult;
			}
			else if (context.Div() != null)
			{
				var second = new FuncNode(KnownFuncType.Pow, new MathFuncNode[] { arg2, new ValueNode(-1) });
				var result = new FuncNode(KnownFuncType.Mult, new MathFuncNode[] { arg1, second });
				return result;
			}
			else
			{
				funcType = KnownFuncType.Pow;
			}

			return new FuncNode(funcType, new MathFuncNode[] { arg1, arg2 });
		}

		public MathFuncNode VisitParenthesisExpression(ParenthesisExpressionContext context)
		{
			var result = Visit(context.expression());
			if (context.Quote() != null)
			{
				result = new FuncNode(KnownFuncType.Diff, result);
			}
			return result;
		}

		public MathFuncNode VisitAbsoluteExpression(AbsoluteExpressionContext context)
		{
			var result = new FuncNode(KnownFuncType.Abs, Visit(context.expression()));
			if (context.Quote() != null)
			{
				result = new FuncNode(KnownFuncType.Diff, result);
			}
			return result;
		}

		public MathFuncNode VisitFuncExpression(FuncExpressionContext context)
		{
			IEnumerable<MathFuncNode> expressions = context.expressionList().expression()
				.Select(e => Visit(e));

			var result = new FuncNode(context.Id().GetText(), expressions);
			if (context.Quote() != null)
			{
				result = new FuncNode(KnownFuncType.Diff, result);
			}
			return result;
		}

		public MathFuncNode VisitExpressionList(ExpressionListContext context)
		{
			throw new ShouldNotBeVisitedException(context);
		}

		public MathFuncNode VisitIdExpression(IdExpressionContext context)
		{
			string id = context.Id().ToString();

			if (!_parameters.TryGetValue(id, out ConstNode idValue))
			{
				idValue = new ConstNode(id);
				_parameters.Add(id, idValue);
			}

			return idValue;
		}

		public MathFuncNode VisitNumberExpression(NumberExpressionContext context)
		{
			return Visit(context.number());
		}

		public MathFuncNode VisitExpression([NotNull] ExpressionContext context)
		{
			throw new ShouldNotBeVisitedException(context);
		}

		public MathFuncNode VisitNumber([NotNull] NumberContext context)
		{
			string intPart = context.intPart != null ? context.intPart.Text : "0";
			string fracPart;
			string periodPart;

			if (context.fracTail() != null)
			{
				var fracTail = context.fracTail();
				fracPart = fracTail.fracPart != null ? fracTail.fracPart.Text : "0";
				periodPart = fracTail.periodPart != null ? fracTail.periodPart.Text : "0";
			}
			else
			{
				fracPart = "0";
				periodPart = "0";
			}

			var rational = Rational<long>.FromDecimal(intPart, fracPart, periodPart);
			return new ValueNode(rational);
		}

		public MathFuncNode VisitFracTail([NotNull] FracTailContext context)
		{
			throw new ShouldNotBeVisitedException(context);
		}

		public MathFuncNode Visit(Antlr4.Runtime.Tree.IParseTree tree)
		{
			return tree?.Accept(this);
		}

		public MathFuncNode VisitChildren(Antlr4.Runtime.Tree.IRuleNode node)
		{
			MathFuncNode firstNode = null;
			for (int i = 0; i < node.ChildCount; i++)
			{
				MathFuncNode result = Visit(node.GetChild(i));
				if (i == 0)
				{
					firstNode = result;
				}
			}
			return firstNode;
		}

		public MathFuncNode VisitErrorNode(Antlr4.Runtime.Tree.IErrorNode node)
		{
			throw new System.NotImplementedException();
		}

		public MathFuncNode VisitTerminal(Antlr4.Runtime.Tree.ITerminalNode node)
		{
			throw new System.NotImplementedException();
		}
	}
}