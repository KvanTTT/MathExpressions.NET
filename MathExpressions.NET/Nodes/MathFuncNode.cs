using System;
using System.Collections.Generic;
using System.Globalization;

namespace MathExpressionsNET
{
	public abstract class MathFuncNode : IComparable, IComparable<MathFuncNode>, ICloneable
	{
		internal List<MathFuncNode> Children = new List<MathFuncNode>();
		internal int Number = -1;
		internal int ArgNumber = -1;

		#region Properties

		public abstract bool IsTerminal { get; }

		public string Name
		{
			get;
			protected set;
		}

		public bool IsValueOrCalculated => this is ValueNode || this is CalculatedNode;

		public virtual double DoubleValue => throw new Exception();

		public bool IsCalculated
		{
			get
			{
				foreach (MathFuncNode child in Children)
					if (child is FuncNode funcNode)
					{
						if (!funcNode.IsKnown || !funcNode.IsCalculated)
							return false;
					}
					else if (!(child is ValueNode) && !(child is CalculatedNode))
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
				foreach (MathFuncNode child in Children)
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
			switch (this)
			{
				case FuncNode funcNode:
					switch (other)
					{
						case FuncNode otherFuncNode:
							if (Name != other.Name)
								result = Name.CompareTo(other.Name);
							else if (Children.Count != other.Children.Count)
								result = -Children.Count.CompareTo(other.Children.Count);
							else
							{
								for (int i = 0; i < Children.Count; i++)
								{
									result = Children[i].CompareTo(other.Children[i]);
									if (result != 0)
										break;
								}
							}
							break;
						default:
							result = -1;
							break;
					}
					break;
				case VarNode varNode:
					switch (other)
					{
						case FuncNode otherFuncNode:
							result = 1;
							break;
						case VarNode otherVarNode:
							result = Name.CompareTo(other.Name);
							break;
						default:
							result = -1;
							break;
					}
					break;
				case ConstNode constNode:
					switch (other)
					{
						case FuncNode otherFuncNode:
						case VarNode otherVarNode:
							result = 1;
							break;
						case ConstNode otherConstNode:
							result = Name.CompareTo(other.Name);
							break;
						default:
							result = -1;
							break;
					}
					break;
				case ValueNode valueNode:
					switch (other)
					{
						case FuncNode otherFuncNode:
						case VarNode otherVarNode:
						case ConstNode otherConstNode:
							result = 1;
							break;
						case ValueNode otherValueNode:
							result = valueNode.Value.CompareTo(otherValueNode.Value);
							break;
						case CalculatedNode otherCalculatedNode:
							result = valueNode.Value.ToDouble(CultureInfo.InvariantCulture).CompareTo(otherCalculatedNode.Value);
							break;
					}
					break;
				case CalculatedNode calculatedNode:
					switch (other)
					{
						case FuncNode otherFuncNode:
						case VarNode otherVarNode:
						case ConstNode otherConstNode:
							result = 1;
							break;
						case ValueNode otherValueNode:
							result = calculatedNode.Value.CompareTo(otherValueNode.Value.ToDouble(CultureInfo.InvariantCulture));
							break;
						case CalculatedNode otherCalculatedNode:
							result = calculatedNode.Value.CompareTo(otherCalculatedNode.Value);
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
				foreach (MathFuncNode child in Children)
					child.Sort();
				if (funcNode.FunctionType == KnownFuncType.Add || funcNode.FunctionType == KnownFuncType.Mult)
					Children.Sort();
			}
		}

		public object Clone()
		{
			switch (this)
			{
				case CalculatedNode calculatedNode:
					return new CalculatedNode(calculatedNode.Value);
				case ValueNode valueNode:
					return new ValueNode(valueNode);
				case VarNode varNode:
				case ConstNode constNode:
					return this;
				case FuncNode funcNode:
					return new FuncNode((FuncNode)this);
				default:
					return this;
			}
		}

		public bool LessThenZero()
		{
			switch (this)
			{
				case CalculatedNode calculatedNode:
					return calculatedNode.Value < 0;
				case ValueNode valueNode:
					return valueNode.Value < 0;
				case VarNode varNode:
				case ConstNode constNode:
					return false;
				case FuncNode funcNode:
					return funcNode.FunctionType == KnownFuncType.Neg;
				default:
					return false;
			}
		}

		public MathFuncNode Abs()
		{
			switch (this)
			{
				case CalculatedNode calculatedNode:
					return new CalculatedNode(Math.Abs(calculatedNode.Value));
				case ValueNode valueNode:
					return new ValueNode(valueNode.Value.Abs());
				case VarNode varNode:
				case ConstNode constNode:
					return this;
				case FuncNode funcNode:
					if (funcNode.FunctionType == KnownFuncType.Neg)
						return Children[0];
					else
						return this;
				default:
					return this;
			}
		}
	}
}
