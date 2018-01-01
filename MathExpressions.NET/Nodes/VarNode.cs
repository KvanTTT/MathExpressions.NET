namespace MathExpressionsNET
{
	public class VarNode : MathFuncNode
	{
		public VarNode(string variable)
		{
			Name = variable;
		}

		public override MathNodeType Type => MathNodeType.Variable;

		public override bool IsTerminal => true;
	}
}
