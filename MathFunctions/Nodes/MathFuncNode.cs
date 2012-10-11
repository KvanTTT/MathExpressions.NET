using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions
{
	public abstract class MathFuncNode : IComparable, IComparable<MathFuncNode>, ICloneable
	{
		internal List<MathFuncNode> Childs = new List<MathFuncNode>();
		internal int Number = -1;
		internal int ArgNumber = -1;

		#region Properties
		
		public abstract MathNodeType Type
		{
			get;
		}

		public abstract bool IsTerminal
		{
			get;
		}

		public string Name
		{
			get;
			protected set;
		}

		public virtual Rational<long> Value
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public bool IsValue
		{
			get
			{
				bool allChildsAreValues = true;
				foreach (var child in Childs)
					if (child.Type == MathNodeType.Function)
					{
						if (!((FuncNode)child).IsValue)
						{
							allChildsAreValues = false;
							break;
						}
					}
					else if (child.Type != MathNodeType.Value)
					{
						allChildsAreValues = false;
						break;
					}
				return allChildsAreValues && Type == MathNodeType.Value;
			}
		}

		public int NodeCount
		{
			get
			{
				int result = 1;
				foreach (var child in Childs)
					result += child != null ? child.NodeCount : 1;
				return result;
			}
		}

		#endregion

		protected MathFuncNode()
		{
		}

		public virtual string ToString(MathFuncNode parent)
		{
			return Name;
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj is string)
			{
				if ((string)obj == Name)
					return true;
				else
					return false;
			}

			var v = obj as MathFuncNode;
			if (v != null)
				return v.Name == Name;
			else
				return false;
		}

		public virtual bool Equals(string s)
		{
			if (s != null)
				return s == Name;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}

		public int CompareTo(MathFuncNode other)
		{
			int result = 0;
			switch (Type)
			{
				case MathNodeType.Function:
					switch (other.Type)
					{
						case MathNodeType.Function:
							bool isValue = IsValue;
							bool isOtherValue = other.IsValue;
							if ((isValue && isOtherValue) ||
								(!isValue && !isOtherValue))
							{
								if (Name != other.Name)
									result = Name.CompareTo(other.Name);
								else if (Childs.Count != other.Childs.Count)
									result = -Childs.Count.CompareTo(other.Childs.Count);
								else
								{
									for (int i = 0; i < Childs.Count; i++)
									{
										var c = Childs[i].CompareTo(other.Childs[i]);
										if (c != 0)
											result = c;
									}
									result = 0;
								}
							}
							else if (isValue)
								result = 1;
							else
								result = -1;
							break;
						case MathNodeType.Variable:
						case MathNodeType.Constant:
							result = IsValue ? 1 : -1;
							break;
						case MathNodeType.Value:
							result = -1;
							break;
					}
					break;
				case MathNodeType.Variable:
					switch (other.Type)
					{
						case MathNodeType.Function:
							result = other.IsValue ? -1 : 1;
							break;
						case MathNodeType.Variable:
							result = Name.CompareTo(other.Name);
							break;
						case MathNodeType.Constant:
						case MathNodeType.Value:
							result = -1;
							break;
					}
					break;
				case MathNodeType.Constant:
					switch (other.Type)
					{
						case MathNodeType.Function:
							result = other.IsValue ? -1 : 1;
							break;
						case MathNodeType.Variable:
							result = 1;
							break;
						case MathNodeType.Constant:
							result = Name.CompareTo(other.Name);
							break;
						case MathNodeType.Value:
							result = -1;
							break;
					}
					break;
				case MathNodeType.Value:
					switch (other.Type)
					{
						case MathNodeType.Function:
						case MathNodeType.Variable:
						case MathNodeType.Constant:
							result = 1;
							break;
						case MathNodeType.Value:
							result = ((ValueNode)this).Value.CompareTo(((ValueNode)other).Value);
							break;
					}
					break;
			}
			return -result;
		}

		public void Sort()
		{
			foreach (var child in Childs)
				child.Sort();

			var funcNode = this as FuncNode;
			if (funcNode != null)
			{
				if (funcNode.FunctionType == KnownMathFunctionType.Add ||
					funcNode.FunctionType == KnownMathFunctionType.Mult)
					Childs.Sort();
			}
		}

		public object Clone()
		{
			switch (Type)
			{
				case MathNodeType.Value:
					return new ValueNode((ValueNode)this);
				case MathNodeType.Variable:
				case MathNodeType.Constant:
					return this;
				case MathNodeType.Function:
					return new FuncNode((FuncNode)this);
				default:
					return this;
			}
		}

		public bool LessThenZero()
		{
			switch (Type)
			{
				case MathNodeType.Value:
					return Value < 0;
				case MathNodeType.Variable:
				case MathNodeType.Constant:
					return false;
				case MathNodeType.Function:
					return ((FuncNode)this).FunctionType == KnownMathFunctionType.Neg;
				default:
					return false;
			}
		}

		public MathFuncNode Abs()
		{
			switch (Type)
			{
				case MathNodeType.Value:
					return new ValueNode(Value.Abs());
				case MathNodeType.Variable:
				case MathNodeType.Constant:
					return this;
				case MathNodeType.Function:
					if (((FuncNode)this).FunctionType == KnownMathFunctionType.Neg)
						return this.Childs[0];
					else
						return this;
				default:
					return this;
			}
		}
	}
}
