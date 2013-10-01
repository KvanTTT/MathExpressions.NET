using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Numerics;

namespace MathFunctions
{
	public class ValueNode : MathFuncNode
	{
		private Rational<long> _value;

		public override Rational<long> Value
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

		public ValueNode(ValueNode node)
		{
			Value = node.Value;
			Name = Value.ToString();
		}

		public ValueNode(int value)
		{
			Value = new Rational<long>(value, 1);
		}

		public ValueNode(Rational<long> value)
		{
			Value = value;
			Name = Value.ToString();
		}

		public override MathNodeType Type
		{
			get { return MathNodeType.Value; }
		}

		public override bool IsTerminal
		{
			get { return true; }
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is string)
			{
				if (obj == Value.ToString())
					return true;
				else
					return false;
			}

			if (obj is double)
			{
				if ((double)obj == Value.ToDouble(CultureInfo.InvariantCulture))
					return true;
				else
					return false;
			}

			if (obj is int)
			{
				if ((int)obj == Value)
					return true;
				else
					return false;
			}

			var v = obj as ValueNode;
			if (v != null)
				return v.Value == Value;
			else
				return false;
		}

		public override bool Equals(string s)
		{
			if (s != null)
				return Rational<long>.Parse(s) == Value;
			else
				return false;
		}

		public bool Equals(ValueNode v)
		{
			if (v != null)
				return v.Value == Value;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
