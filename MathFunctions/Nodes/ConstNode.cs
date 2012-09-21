using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public class ConstNode : MathFuncNode
	{
		public ConstNode(string value)
		{
			Name = value;
		}

		public override MathNodeType Type
		{
			get { return MathNodeType.Constant; }
		}

		public override bool IsTerminal
		{
			get { return true; }
		}
	}
}
