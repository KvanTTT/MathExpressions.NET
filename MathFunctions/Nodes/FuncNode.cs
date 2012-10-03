using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public class FuncNode : MathFuncNode
	{
		public readonly KnownMathFunctionType? FunctionType;

		public bool IsKnown
		{
			get { return FunctionType != null; }
		}

		public override MathNodeType Type
		{
			get { return MathNodeType.Function; }
		}

		public override bool IsTerminal
		{
			get { return false; }
		}

		public FuncNode(KnownMathFunctionType type)
			: this(type, new List<MathFuncNode>())
		{
		}

		public FuncNode(KnownMathFunctionType type, MathFuncNode arg)
			: this(type, new List<MathFuncNode>() { arg })
		{
		}

		public FuncNode(KnownMathFunctionType type, MathFuncNode arg1, MathFuncNode arg2)
			: this(type, new List<MathFuncNode>() { arg1, arg2 })
		{
		}

		public FuncNode(KnownMathFunctionType type, MathFuncNode arg1, MathFuncNode arg2, MathFuncNode arg3)
			: this(type, new List<MathFuncNode>() { arg1, arg2, arg3 })
		{
		}

		public FuncNode(KnownMathFunctionType type, IList<MathFuncNode> args)
		{
			FunctionType = type;
			string name;
			if (KnownMathFunction.UnaryFuncsNames.TryGetValue(type, out name))
				Name = name;
			else if (KnownMathFunction.UnaryFuncsNames.TryGetValue(type, out name))
				Name = name;
			else
				Name = FunctionType.ToString();
			foreach (var arg in args)
				Childs.Add(arg);
		}

		public FuncNode(string name)
			: this(name, new List<MathFuncNode>())
		{
		}

		public FuncNode(string name, IList<MathFuncNode> args)
		{
			var lowercasename = name.ToLower();
			if (args.Count >= 2)
			{
				KnownMathFunctionType functionType;
				if (KnownMathFunction.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			else if (args.Count == 1)
			{
				KnownMathFunctionType functionType;
				if (KnownMathFunction.UnaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
				else if (KnownMathFunction.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			Name = lowercasename;
			foreach (var arg in args)
				Childs.Add(arg);
			if (FunctionType == KnownMathFunctionType.Sqrt)
			{
				FunctionType = KnownMathFunctionType.Exp;
				Childs.Add(new ValueNode(0.5m));
			}
		}

		public FuncNode(FuncNode node)
		{
			FunctionType = node.FunctionType;
			Name = node.Name;
			Number = node.Number;
			for (int i = 0; i < node.Childs.Count; i++)
				switch (node.Childs[i].Type)
				{
					case MathNodeType.Value:
						Childs.Add(new ValueNode(node.Childs[i].Value));
						break;
					case MathNodeType.Constant:
					case MathNodeType.Variable:
						Childs.Add(node.Childs[i]);
						break;
					case MathNodeType.Function:
						Childs.Add(new FuncNode((FuncNode)node.Childs[i]));
						break;
				}
		}

		public MathFuncNode LeftNode
		{
			get { return Childs[0]; }
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public override string ToString(MathFuncNode parent)
		{
			if (IsKnown)
			{
				var funcNodeParent = parent as FuncNode;
				var funcType = (KnownMathFunctionType)FunctionType;
				switch ((KnownMathFunctionType)FunctionType)
				{
					case KnownMathFunctionType.Add:
					case KnownMathFunctionType.Sub:
						return ToString(parent, funcType, new KnownMathFunctionType[] {
							KnownMathFunctionType.Add, KnownMathFunctionType.Sub });

					case KnownMathFunctionType.Mult:
					case KnownMathFunctionType.Div:
						return ToString(parent, funcType, new KnownMathFunctionType[] { 
							KnownMathFunctionType.Add, KnownMathFunctionType.Sub,
							KnownMathFunctionType.Mult, KnownMathFunctionType.Div });

					case KnownMathFunctionType.Exp:
						return ToString(parent, funcType, new KnownMathFunctionType[] { 
							KnownMathFunctionType.Add, KnownMathFunctionType.Sub,
							KnownMathFunctionType.Mult, KnownMathFunctionType.Div,
							KnownMathFunctionType.Exp });

					case KnownMathFunctionType.Neg:
						return "-" + Childs[0].ToString(this);

					case KnownMathFunctionType.Diff:
						return "(" + Childs[0].ToString(this) + ")'";

					case KnownMathFunctionType.Abs:
						return string.Format("|{0}|", Childs[0].ToString(this));
				}
			}
			var builder = new StringBuilder(Name + "(");
			foreach (var arg in Childs)
				builder.AppendFormat("{0}, ", arg.ToString(this));
			if (Childs.Count != 0)
				builder.Remove(builder.Length - 2, 2);
			builder.Append(')');
			return builder.ToString();
		}

		protected string ToString(MathFuncNode parent,
			KnownMathFunctionType funcType,
			IList<KnownMathFunctionType> types)
		{
			var builder = new StringBuilder();

			if (parent == null || parent.Name == null || parent.Childs.Count <= 1)
			{
				AppendMathFunctionNode(builder, funcType);
				return builder.ToString();
			}

			var funcNodeParent = parent as FuncNode;
			if (funcNodeParent != null && funcNodeParent.IsKnown)
				if (types.Contains((KnownMathFunctionType)funcNodeParent.FunctionType))
				{
					AppendMathFunctionNode(builder, funcType);
					return builder.ToString();
				}

			builder.Append("(");
			AppendMathFunctionNode(builder, funcType);
			builder.Append(")");
			return builder.ToString();
		}

		private void AppendMathFunctionNode(StringBuilder builder, KnownMathFunctionType funcType)
		{
			builder.Append(Childs[0].ToString(this) + " ");
			for (int i = 1; i < Childs.Count; i++)
				builder.AppendFormat("{0} {1} ", KnownMathFunction.BinaryFuncsNames[funcType], Childs[i].ToString(this));
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
				if (Name != funcNode.Name || Childs.Count != funcNode.Childs.Count)
					return false;

				bool allChildsAreEqual = true;
				for (int i = 0; i < funcNode.Childs.Count; i++)
					if (!Childs[i].Equals(funcNode.Childs[i]))
					{
						allChildsAreEqual = false;
						break;
					}
				if (!allChildsAreEqual)
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
			foreach (var child in Childs)
				hash ^= child.GetHashCode();
			return hash;
		}
	}
}
