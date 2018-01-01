using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionsNET
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

		public MathFuncNode Root
		{
			get;
			protected set;
		}

		public MathFunc(string str, string v = null, bool simplify = true, bool precompile = false)
		{
			List<MathFunc> mathFuncs = new MathExprConverter().Convert(str);
			MathFunc first = mathFuncs.FirstOrDefault();

			LeftNode = first.LeftNode;
			RightNode = first.RightNode;
			Root = RightNode;

			if (string.IsNullOrEmpty(v))
			{
				ConstToVars();
			}
			else
			{
				Variable = new VarNode(v);
				ConstToVar(Root);
			}

			FindParamsAndUnknownFuncs(Root);

			Root.Sort();
			if (simplify)
				Root = Simplify(Root);
			if (precompile)
				Root = Precompile(null, Root);
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
			bool simplify = true, bool calculateConstants = false)
		{
			LeftNode = left;
			RightNode = right;
			Root = RightNode;

			Variable = variable;
			if (Variable == null)
				ConstToVars();
			if (parameters != null)
				Parameters = parameters.Except(parameters.Where(p => p.Name == Variable.Name)).ToDictionary(node => node.Name);

			FindParamsAndUnknownFuncs(Root);

			Root.Sort();
			if (simplify)
				Root = Simplify(Root);
			if (calculateConstants)
				Root = Precompile(null, Root);
		}

		public ValueNode SimplifyValues(KnownFuncType? funcType, IList<ValueNode> args)
		{
			Rational<long> result;
			double temp = 0.0;

			switch (funcType)
			{
				case KnownFuncType.Add:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result += args[i].Value;
					return new ValueNode(result);

				case KnownFuncType.Sub:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result -= args[i].Value;
					return new ValueNode(result);

				case KnownFuncType.Mult:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result *= args[i].Value;
					return new ValueNode(result);

				case KnownFuncType.Div:
					result = args[0].Value;
					for (int i = 1; i < args.Count; i++)
						result /= args[i].Value;
					return new ValueNode(result);

				case KnownFuncType.Pow:
					if (args[1].Value.ToDouble() == 0.5)
						temp = Math.Sqrt(args[0].Value.ToDouble());
					else
						temp = Math.Pow(args[0].Value.ToDouble(), args[1].Value.ToDouble());
					break;
				
				case KnownFuncType.Neg:
					return new ValueNode(-args[0].Value);

				case KnownFuncType.Sgn:
					return new ValueNode(new Rational<long>((long)Math.Sign(args[0].DoubleValue), 1, false));

				case KnownFuncType.Trunc:
					return new ValueNode(new Rational<long>((long)Math.Truncate(args[0].DoubleValue), 1, false));

				case KnownFuncType.Round:
					return new ValueNode(new Rational<long>((long)Math.Round(args[0].DoubleValue), 1, false));

				case KnownFuncType.Diff:
					return new ValueNode(0);

				case KnownFuncType.Sqrt:
					temp = Math.Sqrt(args[0].DoubleValue);
					break;

				case KnownFuncType.Sin:
					temp = Math.Sin(args[0].DoubleValue);
					break;

				case KnownFuncType.Cos:
					temp = Math.Cos(args[0].DoubleValue);
					break;

				case KnownFuncType.Tan:
					temp = Math.Tan(args[0].DoubleValue);
					break;

				case KnownFuncType.Cot:
					temp = 1 / Math.Tan(args[0].DoubleValue);
					break;

				case KnownFuncType.Arcsin:
					temp = Math.Asin(args[0].DoubleValue);
					break;

				case KnownFuncType.Arccos:
					temp = Math.Acos(args[0].DoubleValue);
					break;

				case KnownFuncType.Arctan:
					temp = Math.Atan(args[0].DoubleValue);
					break;

				case KnownFuncType.Arccot:
					temp = Math.PI / 2 - Math.Atan(args[0].DoubleValue);
					break;

				case KnownFuncType.Sinh:
					temp = Math.Sinh(args[0].DoubleValue);
					break;

				case KnownFuncType.Cosh:
					temp = Math.Cosh(args[0].DoubleValue);
					break;

				case KnownFuncType.Arcsinh:
					temp = Math.Log(args[0].DoubleValue + Math.Sqrt(args[0].DoubleValue * args[0].DoubleValue + 1));
					break;

				case KnownFuncType.Arcosh:
					temp = Math.Log(args[0].DoubleValue + Math.Sqrt(args[0].DoubleValue * args[0].DoubleValue - 1));
					break;

				case KnownFuncType.Exp:
					temp = Math.Exp(args[0].DoubleValue);
					break;

				case KnownFuncType.Ln:
					temp = Math.Log(args[0].DoubleValue);
					break;

				case KnownFuncType.Log10:
					temp = Math.Log10(args[0].DoubleValue);
					break;

				case KnownFuncType.Log:
					temp = Math.Log(args[0].DoubleValue, args[1].DoubleValue);
					break;

				case KnownFuncType.Abs:
					temp = Math.Abs(args[0].DoubleValue);
					break;

				default:
					return null;
			}

			try
			{
				if (Rational<long>.FromDecimal((decimal)temp, out result, 14, false, 4, 8))
				return new ValueNode(result);
			}
			catch
			{
			}
			
			return null;
		}

		public void Sort()
		{
			Root.Sort();
		}

		public void ConstToVars()
		{
			if (LeftNode is VarNode varNode)
				Variable = varNode;
			if (LeftNode is ConstNode constNode)
				Variable = new VarNode(LeftNode.Name);
			else
				if (LeftNode is FuncNode funcNode)
				{
					Variable = null;
					if (LeftNode.Children.Count > 1 && funcNode.Children[1] != null)
					{
						var secondNode = funcNode.Children[1];
						if (secondNode is ConstNode)
							Variable = new VarNode(secondNode.Name);
						else if (secondNode is VarNode secondVarNode)
							Variable = secondVarNode;
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
			for (int i = 0; i < node.Children.Count; i++)
				if (Variable == null)
				{
					if (node.Children[i] is ConstNode)
					{
						Variable = new VarNode(node.Children[i].Name);
						break;
					}
					else if (node.Children[i] is VarNode varNode)
					{
						Variable = varNode;
						break;
					}
					else
						GetFirstParam(node.Children[i]);
				}
		}

		protected void ConstToVar(MathFuncNode node)
		{
			for (int i = 0; i < node.Children.Count; i++)
				if (node.Children[i] == null || node.Children[i].Name == Variable.Name)
					node.Children[i] = Variable;
				else if (node.Children[i] is VarNode varNode)
					node.Children[i] = new ConstNode(node.Children[i].Name);
				else
					ConstToVar(node.Children[i]);
		}

		protected void FindParamsAndUnknownFuncs(MathFuncNode node)
		{
			foreach (MathFuncNode child in node.Children)
				FindParamsAndUnknownFuncs(child);

			if (node is FuncNode funcNode && !funcNode.IsKnown)
			{
				if (!UnknownFuncs.ContainsKey(node.Name))
					UnknownFuncs.Add(node.Name, funcNode);
			}
			else if (node is ConstNode constNode)
			{
				if (!Parameters.ContainsKey(node.Name))
					Parameters.Add(node.Name, constNode);
			}
		}

		public bool IsValue => Root.IsValueOrCalculated;

		public bool IsCalculated => Root.IsCalculated;

		public override string ToString() => Root.ToString();

		public string ToShortString() => Root.ToString().Replace(" ", "");

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
