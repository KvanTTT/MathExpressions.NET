namespace MathExpressionsNET
{
	public class ValueNode : MathFuncNode
	{
		public Rational<long> Value { get; set; }

		public override double DoubleValue => Value.ToDouble(null);

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

		public override bool IsTerminal => true;

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
				parent.FunctionType == KnownFuncType.Pow || parent.FunctionType == KnownFuncType.Neg)
			{
				return '(' + Name + ')';
			}
			else
				return Name;
		}
	}
}
