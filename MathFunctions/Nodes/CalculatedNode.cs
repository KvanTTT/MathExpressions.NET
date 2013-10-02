using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public class CalculatedNode : MathFuncNode
	{
		private double _value;

		public double Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public override MathNodeType Type
		{
			get { return MathNodeType.Calculated; }
		}

		public override bool IsTerminal
		{
			get { return true; }
		}

		public CalculatedNode(double value)
		{
			_value = value;
			Name = _value.ToString(CultureInfo.InvariantCulture);
		}

		public CalculatedNode(Rational<long> value)
		{
			_value = (double)value.ToDecimal(CultureInfo.InvariantCulture);
			Name = _value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
