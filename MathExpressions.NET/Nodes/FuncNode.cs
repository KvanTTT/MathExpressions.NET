using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressionsNET
{
	public class FuncNode : MathFuncNode
	{
		public readonly KnownFuncType? FunctionType;

		public bool IsKnown => FunctionType != null;

		public override bool IsTerminal => false;

		public FuncNode(KnownFuncType type)
			: this(type, new List<MathFuncNode>())
		{
		}

		public FuncNode(KnownFuncType type, MathFuncNode arg)
			: this(type, new List<MathFuncNode>() { arg })
		{
		}

		public FuncNode(KnownFuncType type, MathFuncNode arg1, MathFuncNode arg2)
			: this(type, new List<MathFuncNode>() { arg1, arg2 })
		{
		}

		public FuncNode(KnownFuncType type, MathFuncNode arg1, MathFuncNode arg2, MathFuncNode arg3)
			: this(type, new List<MathFuncNode>() { arg1, arg2, arg3 })
		{
		}

		public FuncNode(KnownFuncType type, IEnumerable<MathFuncNode> args)
		{
			FunctionType = type;
			string name;
			if (KnownFunc.UnaryFuncsNames.TryGetValue(type, out name))
				Name = name;
			else if (KnownFunc.BinaryFuncsNames.TryGetValue(type, out name))
				Name = name;
			else
				Name = FunctionType.ToString();
			foreach (MathFuncNode arg in args)
				Children.Add(arg);
		}

		public FuncNode(string name)
			: this(name, new List<MathFuncNode>())
		{
		}

		public FuncNode(string name, IEnumerable<MathFuncNode> args)
		{
			var lowercasename = name.ToLower();
			if (args.Count() >= 2)
			{
				KnownFuncType functionType;
				if (KnownFunc.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			else if (args.Count() == 1)
			{
				KnownFuncType functionType;
				if (KnownFunc.UnaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
				else if (KnownFunc.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			Name = lowercasename;
			foreach (MathFuncNode arg in args)
				Children.Add(arg);
			if (FunctionType == KnownFuncType.Sqrt)
			{
				FunctionType = KnownFuncType.Pow;
				Children.Add(new ValueNode(new Rational<long>(1, 2)));
			}
		}

		public FuncNode(FuncNode node)
		{
			FunctionType = node.FunctionType;
			Name = node.Name;
			Number = node.Number;
			for (int i = 0; i < node.Children.Count; i++)
				switch (node.Children[i])
				{
					case ValueNode valueNode:
						Children.Add(new ValueNode(valueNode.Value));
						break;
					case ConstNode constNode:
					case VarNode varNode:
						Children.Add(node.Children[i]);
						break;
					case FuncNode funcNode:
						Children.Add(new FuncNode(funcNode));
						break;
				}
		}

		public MathFuncNode LeftNode => Children[0];

		public override string ToString()
		{
			return ToString(null);
		}

		public override string ToString(FuncNode parent)
		{
			if (IsKnown)
			{
				var funcType = (KnownFuncType)FunctionType;
				switch ((KnownFuncType)FunctionType)
				{
					case KnownFuncType.Add:
					case KnownFuncType.Sub:
						return ToString(parent, funcType, KnownFunc.AddKnownFuncs);

					case KnownFuncType.Mult:
					case KnownFuncType.Div:
						return ToString(parent, funcType, KnownFunc.MultKnownFuncs);

					case KnownFuncType.Pow:
						return ToString(parent, funcType, KnownFunc.ExpKnownFuncs);

					case KnownFuncType.Neg:
						if (Children[0] is FuncNode funcNode)
						{
							if (KnownFunc.NegKnownFuncs.Contains((KnownFuncType)funcNode.FunctionType))
								return "-(" + Children[0].ToString(this) + ")";
							else
								return "-" + Children[0].ToString(this);
						}
						else
							return "-" + Children[0].ToString(this);

					case KnownFuncType.Diff:
						return Children[0] is FuncNode funcNode2 && funcNode2.FunctionType != KnownFuncType.Diff ?
							Children[0].ToString(this) + "'" : "(" + 
							Children[0].ToString(this) + ")'";

					case KnownFuncType.Abs:
						return string.Format("|{0}|", Children[0].ToString(this));
				}
			}
			var builder = new StringBuilder((FunctionType == KnownFuncType.Sqrt ? "√" : Name) + "(");
			foreach (var arg in Children)
				builder.AppendFormat("{0}, ", arg.ToString(this));
			if (Children.Count != 0)
				builder.Remove(builder.Length - 2, 2);
			builder.Append(')');
			return builder.ToString();
		}

		protected string ToString(FuncNode parent,
			KnownFuncType funcType,
			IList<KnownFuncType> types)
		{
			var builder = new StringBuilder();

			if (parent == null || parent.Name == null || parent.Children.Count <= 1)
			{
				AppendMathFunctionNode(builder, funcType);
				return builder.ToString();
			}

			var funcNodeParent = parent as FuncNode;
			if (funcNodeParent != null && funcNodeParent.IsKnown)
				if (types.Contains((KnownFuncType)funcNodeParent.FunctionType))
				{
					AppendMathFunctionNode(builder, funcType);
					return builder.ToString();
				}

			builder.Append("(");
			AppendMathFunctionNode(builder, funcType);
			builder.Append(")");
			return builder.ToString();
		}

		private void AppendMathFunctionNode(StringBuilder builder, KnownFuncType funcType)
		{
			builder.Append(Children[0].ToString(this) + " ");
			for (int i = 1; i < Children.Count; i++)
				builder.AppendFormat("{0} {1} ", KnownFunc.BinaryFuncsNames[funcType], Children[i].ToString(this));
			builder.Remove(builder.Length - 1, 1);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			FuncNode funcNode;

			if (obj is MathFunc)
				funcNode = (obj as MathFunc).Root as FuncNode;
			else
				funcNode = obj as FuncNode;

			if (funcNode != null)
			{
				if (Name != funcNode.Name || Children.Count != funcNode.Children.Count)
					return false;

				bool allChildrenAreEqual = true;
				for (int i = 0; i < funcNode.Children.Count; i++)
					if (!Children[i].Equals(funcNode.Children[i]))
					{
						allChildrenAreEqual = false;
						break;
					}
				if (!allChildrenAreEqual)
					return false;

				return true;
			}

			return false;
		}

		public override bool Equals(string s)
		{
			if (s != null)
				return Equals(new MathFunc(s));
			else
				return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			foreach (var child in Children)
				hash ^= child.GetHashCode();
			return hash ^ (FunctionType != null ? (int)FunctionType : Name.GetHashCode());
		}
	}
}
