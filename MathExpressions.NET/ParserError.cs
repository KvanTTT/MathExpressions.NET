using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOLD;

namespace MathExpressions.NET
{
	public class ParserError
	{
		public Position Position;
		public int Length;
		public string Message;

		public ParserError(string message)
		{
			Position = null;
			Message = message;
		}

		public ParserError(Position pos, string message)
		{
			Position = pos;
			Message = message;
		}

		public ParserError(Position pos, int length, string message)
		{
			Position = pos;
			Length = length;
			Message = message;
		}
	}
}
