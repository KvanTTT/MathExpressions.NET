namespace MathExpressionsNET
{
	public class ConstNode : MathFuncNode
	{
		public ConstNode(string value)
		{
			Name = value;
		}

		public override bool IsTerminal => true;
	}
}
