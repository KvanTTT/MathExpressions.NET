using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public partial class MathFunc
	{
		public VarNode Variable
		{
			get;
			private set;
		}

		private Dictionary<string, ConstNode> Parameters = new Dictionary<string, ConstNode>();
		private Dictionary<string, FuncNode> UnknownFuncs = new Dictionary<string, FuncNode>();

		public MathFuncNode LeftNode
		{
			get;
			private set;
		}

		public MathFuncNode RightNode
		{
			get;
			private set;
		}

		public readonly bool CalculateConstants;

		public MathFuncNode Root
		{
			get;
			protected set;
		}

		public MathFunc(string str, string v = null, bool calculateConstants = false, bool simplify = false)
		{
			if (!Helper.Parser.Parse(str))
				throw new Exception("Impossible to parse input string");

			LeftNode = Helper.Parser.FirstStatement.LeftNode;
			RightNode = Helper.Parser.FirstStatement.RightNode;
			Root = RightNode;

			if (string.IsNullOrEmpty(v))
				ConstToVars();
			else
			{
				Variable = new VarNode(v);
				ConstToVar(Root);
			}

			FindParamsAndUnknownFuncs(Root);

			Root.Sort();
			Root = Simplify(Root);
			CalculateConstants = calculateConstants;
			if (calculateConstants || simplify)
				Root = Simplify(Root);
		}

		public MathFunc(MathFuncNode root, 
			VarNode variable = null, IEnumerable<ConstNode> parameters = null,
			bool calculateConstants = false, bool simplify = false)
			: this(new FuncNode("f", new List<MathFuncNode>() { variable }), root, variable, parameters,
				calculateConstants, simplify)
		{
		}

		public MathFunc(MathFuncNode left, MathFuncNode right, 
			VarNode variable = null, IEnumerable<ConstNode> parameters = null,
			bool calculateConstants = false, bool simplify = false)
		{
			LeftNode = left;
			RightNode = right;
			Root = RightNode;

			Variable = variable;
			if (Variable == null)
				ConstToVars();
			if (parameters != null)
				Parameters = parameters.Except(parameters.Where(p => p.Name == Variable.Name))
					.ToDictionary(node => node.Name);

			FindParamsAndUnknownFuncs(Root);

			Root.Sort();
			CalculateConstants = calculateConstants;
			if (calculateConstants || simplify)
				Root = Simplify(Root);
		}

		public MathFuncNode Calculate(KnownMathFunctionType? funcType, IList<ValueNode> args)
		{
			Rational<long> result;
			double temp = 0.0;

			switch (funcType)
			{
				case KnownMathFunctionType.Add:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result += args[i].Value;
					return new ValueNode(result);

				case KnownMathFunctionType.Sub:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result -= args[i].Value;
					return new ValueNode(result);

				case KnownMathFunctionType.Mult:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result *= args[i].Value;
					return new ValueNode(result);

				case KnownMathFunctionType.Div:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result /= args[i].Value;
					return new ValueNode(result);

				case KnownMathFunctionType.Exp:
					temp = Math.Pow(args[0].Value.ToDouble(), args[1].Value.ToDouble());
					break;
				
				case KnownMathFunctionType.Neg:
					return new ValueNode(-args[0].Value);

				case KnownMathFunctionType.Sqrt:
					temp = Math.Sqrt(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Sin:
					temp = Math.Sin(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Cos:
					temp = Math.Cos(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Tan:
					temp = Math.Tan(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Cot:
					temp = 1 / Math.Tan(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Arcsin:
					temp = Math.Asin(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Arccos:
					temp = Math.Acos(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Arctan:
					temp = Math.Atan(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Arccot:
					temp = Math.PI / 2 - Math.Atan(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Sinh:
					temp = Math.Sinh(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Cosh:
					temp = Math.Cosh(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Arcsinh:
					temp = Math.Log(args[0].Value.ToDouble() + Math.Sqrt(args[0].Value.ToDouble() * args[0].Value.ToDouble() + 1));
					break;

				case KnownMathFunctionType.Arcosh:
					temp = Math.Log(args[0].Value.ToDouble() + Math.Sqrt(args[0].Value.ToDouble() * args[0].Value.ToDouble() - 1));
					break;

				case KnownMathFunctionType.Ln:
					temp = Math.Log(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Log10:
					temp = Math.Log10(args[0].Value.ToDouble());
					break;

				case KnownMathFunctionType.Log:
					temp = Math.Log(args[0].Value.ToDouble(), args[1].Value.ToDouble());
					break;

				case KnownMathFunctionType.Abs:
					temp = args[0].Value.Abs().ToDouble();
					break;

				case KnownMathFunctionType.Sgn:
					return new ValueNode(new Rational<long>((long)Math.Sign(args[0].Value.ToDouble()), 1, false));

				case KnownMathFunctionType.Trunc:
					return new ValueNode(new Rational<long>((long)Math.Truncate(args[0].Value.ToDouble()), 1, false));

				case KnownMathFunctionType.Round:
					return new ValueNode(new Rational<long>((long)Math.Round(args[0].Value.ToDouble()), 1, false));

				case KnownMathFunctionType.Diff:
					return new ValueNode(0);

				default:
					return null;
			}

			if (Rational<long>.FromDecimal((decimal)temp, out result, 12, false, 2, 8))
				return new ValueNode(result);
			else if (CalculateConstants)
				return new ValueNode(Rational<long>.Approximate((decimal)temp));
			else
				return null;
		}

		public void Sort()
		{
			Root.Sort();
		}

		public void ConstToVars()
		{
			if (LeftNode.Type == MathNodeType.Variable)
				Variable = (VarNode)LeftNode;
			if (LeftNode.Type == MathNodeType.Constant)
				Variable = new VarNode(LeftNode.Name);
			else
				if (LeftNode.Type == MathNodeType.Function)
				{
					Variable = null;
					if (LeftNode.Childs.Count > 1 && ((FuncNode)LeftNode).Childs[1] != null)
					{
						var secondNode = ((FuncNode)LeftNode).Childs[1];
						if (secondNode.Type == MathNodeType.Constant)
							Variable = new VarNode(secondNode.Name);
						else if (secondNode.Type == MathNodeType.Variable)
							Variable = (VarNode)secondNode;
					}
					GetFirstParam(RightNode);
					if (Variable == null)
						Variable = new VarNode("x");
				}

			ConstToVar(Root);
			if (Root.Name == Variable.Name)
				Root = Variable;
		}

		protected void GetFirstParam(MathFuncNode node)
		{
			if (Variable == null)
				for (int i = 0; i < node.Childs.Count; i++)
					if (Variable == null)
						if (node.Childs[i].Type == MathNodeType.Constant)
						{
							Variable = new VarNode(node.Childs[i].Name);
							break;
						}
						else if (node.Childs[i].Type == MathNodeType.Variable)
						{
							Variable = (VarNode)node.Childs[i];
							break;
						}
						else
							GetFirstParam(node.Childs[i]);
		}

		protected void ConstToVar(MathFuncNode node)
		{
			for (int i = 0; i < node.Childs.Count; i++)
				if (node.Childs[i] == null || node.Childs[i].Name == Variable.Name)
					node.Childs[i] = Variable;
				else
					ConstToVar(node.Childs[i]);
		}

		protected void FindParamsAndUnknownFuncs(MathFuncNode node)
		{
			foreach (var child in node.Childs)
				FindParamsAndUnknownFuncs(child);

			if (node.Type == MathNodeType.Function && !((FuncNode)node).IsKnown)
			{
				if (!UnknownFuncs.ContainsKey(node.Name))
					UnknownFuncs.Add(node.Name, (FuncNode)node);
			}
			else if (node.Type == MathNodeType.Constant)
			{
				if (!Parameters.ContainsKey(node.Name))
					Parameters.Add(node.Name, (ConstNode)node);
			}
		}

		public bool IsValue
		{
			get
			{
				return Root.IsValue;
			}
		}

		public override string ToString()
		{
			return Root.ToString();
		}

		public override bool Equals(object obj)
		{
			return Root.Equals(obj);
		}

		public bool Equals(string str)
		{
			return Root.Equals(str);
		}

		public override int GetHashCode()
		{
			return Root.GetHashCode();
		}

		public static bool operator ==(MathFunc func1, MathFunc func2)
		{
			return func1.Equals(func2);
		}

		public static bool operator ==(MathFunc func1, string func2)
		{
			return func1.Equals(func2);
		}

		public static bool operator ==(string func1, MathFunc func2)
		{
			return !func2.Equals(func1);
		}

		public static bool operator !=(MathFunc func1, MathFunc func2)
		{
			return !func1.Equals(func2);
		}

		public static bool operator !=(MathFunc func1, string func2)
		{
			return !func1.Equals(func2);
		}

		public static bool operator !=(string func1, MathFunc func2)
		{
			return func2.Equals(func1);
		}
	}
} 
