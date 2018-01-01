using System.Globalization;

namespace MathExpressionsNET
{
	public class CalculatedNode : MathFuncNode
	{
		public double Value { get; set; }

		public override double DoubleValue => Value;

		public override bool IsTerminal => true;

		public CalculatedNode(CalculatedNode node)
		{
			Value = node.Value;
			Name = Value.ToString(CultureInfo.InvariantCulture);
		}

		public CalculatedNode(double value)
		{
			Value = value;
			Name = Value.ToString(CultureInfo.InvariantCulture);
		}

		public CalculatedNode(Rational<long> value)
		{
			Value = (double)value.ToDecimal(CultureInfo.InvariantCulture);
			Name = Value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
