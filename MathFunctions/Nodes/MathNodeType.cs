using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressions.NET
{
	public enum MathNodeType
	{
		Calculated,
		Value,
		Constant,
		Variable,
		Function
	}
}
