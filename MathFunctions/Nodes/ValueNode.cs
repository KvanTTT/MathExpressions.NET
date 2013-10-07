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

		public Rational<long> Value
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

		public override double DoubleValue
		{
			get
			{
				return Value.ToDouble(null);
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
			Name = Value.ToString();
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
				if ((string)obj == Value.ToString())
					return true;
				else
					return false;
			}

			if (obj is double || obj is decimal)
			{
				Rational<long> r;
				if (Rational<long>.FromDecimal((decimal)obj, out r) && r == Value)
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

		public override string ToString(FuncNode parent)
		{
			if (Value.Denominator == 1 || !parent.IsKnown)
				return Name;

			if (parent.FunctionType == KnownFuncType.Mult || parent.FunctionType == KnownFuncType.Div ||
				parent.FunctionType == KnownFuncType.Exp || parent.FunctionType == KnownFuncType.Neg)
			{
				return '(' + Name + ')';
			}
			else
				return Name;
		}
	}
}
