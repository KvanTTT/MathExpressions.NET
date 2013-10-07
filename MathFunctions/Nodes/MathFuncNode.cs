using System;
using System.Collections.Generic;
using System.Globalization;
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

		public bool IsValueOrCalculated
		{
			get
			{
				return Type == MathNodeType.Value || Type == MathNodeType.Calculated;
			}
		}

		public virtual double DoubleValue
		{
			get
			{
				throw new Exception();
			}
		}

		public bool IsCalculated
		{
			get
			{
				foreach (var child in Childs)
					if (child.Type == MathNodeType.Function)
					{
						FuncNode funcNode = (FuncNode)child;
						if (!funcNode.IsKnown || !funcNode.IsCalculated)
							return false;
					}
					else if (child.Type != MathNodeType.Value && child.Type != MathNodeType.Calculated)
					{
						return false;
					}
				return true;
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

		public virtual string ToString(FuncNode parent)
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
							if (Name != other.Name)
								result = Name.CompareTo(other.Name);
							else if (Childs.Count != other.Childs.Count)
								result = -Childs.Count.CompareTo(other.Childs.Count);
							else
							{
								for (int i = 0; i < Childs.Count; i++)
								{
									result = Childs[i].CompareTo(other.Childs[i]);
									if (result != 0)
										break;
								}
							}
							break;
						case MathNodeType.Variable:
						case MathNodeType.Constant:
						case MathNodeType.Value:
						case MathNodeType.Calculated:
							result = -1;
							break;
					}
					break;
				case MathNodeType.Variable:
					switch (other.Type)
					{
						case MathNodeType.Function:
							result = 1;
							break;
						case MathNodeType.Variable:
							result = Name.CompareTo(other.Name);
							break;
						case MathNodeType.Constant:
						case MathNodeType.Value:
						case MathNodeType.Calculated:
							result = -1;
							break;
					}
					break;
				case MathNodeType.Constant:
					switch (other.Type)
					{
						case MathNodeType.Function:
						case MathNodeType.Variable:
							result = 1;
							break;
						case MathNodeType.Constant:
							result = Name.CompareTo(other.Name);
							break;
						case MathNodeType.Value:
						case MathNodeType.Calculated:
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
						case MathNodeType.Calculated:
							result = ((ValueNode)this).Value.ToDouble(CultureInfo.InvariantCulture).CompareTo(((CalculatedNode)other).Value);
							break;
					}
					break;
				case MathNodeType.Calculated:
					switch (other.Type)
					{
						case MathNodeType.Function:
						case MathNodeType.Variable:
						case MathNodeType.Constant:
							result = 1;
							break;
						case MathNodeType.Value:
							result = ((CalculatedNode)this).Value.CompareTo(((ValueNode)other).Value.ToDouble(CultureInfo.InvariantCulture));
							break;
						case MathNodeType.Calculated:
							result = ((CalculatedNode)this).Value.CompareTo(((CalculatedNode)other).Value);
							break;
					}
					break;
			}
			return result;
		}

		public void Sort()
		{
			var funcNode = this as FuncNode;
			if (funcNode != null)
			{
				foreach (var child in Childs)
					child.Sort();
				if (funcNode.FunctionType == KnownFuncType.Add || funcNode.FunctionType == KnownFuncType.Mult)
					Childs.Sort();
			}
		}

		public object Clone()
		{
			switch (Type)
			{
				case MathNodeType.Calculated:
					return new CalculatedNode(((CalculatedNode)this).Value);
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
				case MathNodeType.Calculated:
					return ((CalculatedNode)this).Value < 0;
				case MathNodeType.Value:
					return ((ValueNode)this).Value < 0;
				case MathNodeType.Variable:
				case MathNodeType.Constant:
					return false;
				case MathNodeType.Function:
					return ((FuncNode)this).FunctionType == KnownFuncType.Neg;
				default:
					return false;
			}
		}

		public MathFuncNode Abs()
		{
			switch (Type)
			{
				case MathNodeType.Calculated:
					return new CalculatedNode(Math.Abs(((CalculatedNode)this).Value));
				case MathNodeType.Value:
					return new ValueNode(((ValueNode)this).Value.Abs());
				case MathNodeType.Variable:
				case MathNodeType.Constant:
					return this;
				case MathNodeType.Function:
					if (((FuncNode)this).FunctionType == KnownFuncType.Neg)
						return this.Childs[0];
					else
						return this;
				default:
					return this;
			}
		}
	}
}
