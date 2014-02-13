using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressionsNET
{
	public class FuncNode : MathFuncNode
	{
		public readonly KnownFuncType? FunctionType;

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

		public FuncNode(KnownFuncType type, IList<MathFuncNode> args)
		{
			FunctionType = type;
			string name;
			if (KnownFunc.UnaryFuncsNames.TryGetValue(type, out name))
				Name = name;
			else if (KnownFunc.BinaryFuncsNames.TryGetValue(type, out name))
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
				KnownFuncType functionType;
				if (KnownFunc.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			else if (args.Count == 1)
			{
				KnownFuncType functionType;
				if (KnownFunc.UnaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
				else if (KnownFunc.BinaryNamesFuncs.TryGetValue(lowercasename, out functionType))
					FunctionType = functionType;
			}
			Name = lowercasename;
			foreach (var arg in args)
				Childs.Add(arg);
			if (FunctionType == KnownFuncType.Sqrt)
			{
				FunctionType = KnownFuncType.Exp;
				Childs.Add(new ValueNode(new Rational<long>(1, 2)));
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
						Childs.Add(new ValueNode(((ValueNode)node.Childs[i]).Value));
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

					case KnownFuncType.Exp:
						return ToString(parent, funcType, KnownFunc.ExpKnownFuncs);

					case KnownFuncType.Neg:
						if (Childs[0].Type == MathNodeType.Function)
						{
							var func = (FuncNode)Childs[0];
							if (KnownFunc.NegKnownFuncs.Contains((KnownFuncType)func.FunctionType))
								return "-(" + Childs[0].ToString(this) + ")";
							else
								return "-" + Childs[0].ToString(this);
						}
						else
							return "-" + Childs[0].ToString(this);

					case KnownFuncType.Diff:
						return Childs[0].Type == MathNodeType.Function && ((FuncNode)Childs[0]).FunctionType != KnownFuncType.Diff ?
							Childs[0].ToString(this) + "'" : "(" + 
							Childs[0].ToString(this) + ")'";

					case KnownFuncType.Abs:
						return string.Format("|{0}|", Childs[0].ToString(this));
				}
			}
			var builder = new StringBuilder((FunctionType == KnownFuncType.Sqrt ? "√" : Name) + "(");
			foreach (var arg in Childs)
				builder.AppendFormat("{0}, ", arg.ToString(this));
			if (Childs.Count != 0)
				builder.Remove(builder.Length - 2, 2);
			builder.Append(')');
			return builder.ToString();
		}

		protected string ToString(FuncNode parent,
			KnownFuncType funcType,
			IList<KnownFuncType> types)
		{
			var builder = new StringBuilder();

			if (parent == null || parent.Name == null || parent.Childs.Count <= 1)
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
			builder.Append(Childs[0].ToString(this) + " ");
			for (int i = 1; i < Childs.Count; i++)
				builder.AppendFormat("{0} {1} ", KnownFunc.BinaryFuncsNames[funcType], Childs[i].ToString(this));
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
			return hash ^ (FunctionType != null ? (int)FunctionType : Name.GetHashCode());
		}
	}
}
