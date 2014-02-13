using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressionsNET
{
	public class MathFuncGenerator
	{
		private static KnownFuncType[] _unaryFuncs;
		private static KnownFuncType[] _binaryFuncs;
		private Random _rand = new Random();

		public double ValueProb = 0.2;
		public double ConstProb = 0.05;
		public double VarProb = 0.15;
		public double FuncProb = 0.6;

		public int MinDepth = 1;
		public int MaxDepth = 5;

		public double FracProb = 0.2;
		public double IntProb = 0.8;
		public int MinValue = -100;
		public int MaxValue = 100;

		public double UnknownFuncProb = 0.05;
		public double KnownFuncProb = 0.95;

		public double UnaryFuncProb = 0.6;
		public double BinaryFuncProb = 0.4;

		public double AdditionFuncProb = 0.3;
		public double MultiplicationFuncProb = 0.3;

		public int MaxSummandsCount = 5;
		public int MaxFactorsCount = 4;

		static MathFuncGenerator()
		{
			_unaryFuncs = KnownFunc.UnaryFuncsNames.Keys.ToArray();
			_binaryFuncs = KnownFunc.BinaryFuncsNames.Keys.Where(func => func != KnownFuncType.Add && func != KnownFuncType.Mult).ToArray();
		}

		public MathFunc Generate(string varName, string[] constNames, string[] unknownFuncNames)
		{
			bool error = false;
			MathFunc result = null;
			do
			{
				error = false;
				try
				{
					result = new MathFunc(Generate(0, varName, constNames, unknownFuncNames), new VarNode(varName));
					var precompilied = new MathFunc(result.ToString(), varName, true, true);
					if (precompilied.ContainsNaN())
						error = true;
				}
				catch
				{
					error = true;
				}
			}
			while (error);
			return result;
		}

		public MathFuncNode Generate(int curDepth, string varName, string[] constNames, string[] unknownFuncNames)
		{
			double r = _rand.NextDouble();
			if (curDepth < MinDepth && r < ValueProb + ConstProb + VarProb)
				r = ValueProb + ConstProb + VarProb;
			if (curDepth == MaxDepth && r > ValueProb + ConstProb + VarProb)
				r *= (ValueProb + ConstProb + VarProb);
			double r1, r2, r3;

			if (r < ValueProb)
			{
				r1 = _rand.NextDouble();
				if (r1 < FracProb)
					return new ValueNode(Rational<long>.Approximate((decimal)(MinValue + _rand.NextDouble() * (MaxValue - MinValue)), 0.001m));
				else
					return new ValueNode(_rand.Next(MinValue, MaxValue));
			}
			else if (r < ValueProb + ConstProb && constNames != null && constNames.Length != 0)
			{
				string randConstName = constNames[_rand.Next(constNames.Length)];
				return new ConstNode(randConstName);
			}
			else if (r < ValueProb + ConstProb + VarProb)
			{
				return new VarNode(varName);
			}
			else
			{
				r1 = _rand.NextDouble();
				if (r1 < UnknownFuncProb && unknownFuncNames != null && unknownFuncNames.Length != 0)
				{
					string randFuncName = unknownFuncNames[_rand.Next(unknownFuncNames.Length)];
					return new FuncNode(randFuncName, new List<MathFuncNode>() 
					{
						Generate(curDepth + 1, varName, constNames, unknownFuncNames)
					});
				}
				else
				{
					Array values = Enum.GetValues(typeof(KnownFuncType));
					KnownFuncType randFuncType;
					do
					{
						r2 = _rand.NextDouble();
						if (r2 < AdditionFuncProb)
							randFuncType = KnownFuncType.Add;
						else if (r2 < AdditionFuncProb + MultiplicationFuncProb)
							randFuncType = KnownFuncType.Mult;
						else
						{
							r3 = _rand.NextDouble();
							if (r3 < UnaryFuncProb)
								randFuncType = _unaryFuncs[_rand.Next(_unaryFuncs.Length)];
							else
								randFuncType = _binaryFuncs[_rand.Next(_binaryFuncs.Length)];
						}
					}
					while (KnownFunc.SpecFuncs.Contains(randFuncType));

					if (KnownFunc.UnaryFuncsNames.ContainsKey(randFuncType))
						return new FuncNode(randFuncType, Generate(curDepth + 1, varName, constNames, unknownFuncNames));
					else
					{
						if (randFuncType == KnownFuncType.Add)
						{
							int summandsCount = Math.Min(2, _rand.Next(MaxSummandsCount + 1));
							List<MathFuncNode> summands = new List<MathFuncNode>();
							for (int i = 0; i < summandsCount; i++)
								summands.Add(Generate(curDepth + 1, varName, constNames, unknownFuncNames));
							return new FuncNode(KnownFuncType.Add, summands);
						}
						else if (randFuncType == KnownFuncType.Mult)
						{
							int factorsCount = Math.Min(2, _rand.Next(MaxFactorsCount + 1));
							List<MathFuncNode> factors = new List<MathFuncNode>();
							for (int i = 0; i < factorsCount; i++)
								factors.Add(Generate(curDepth + 1, varName, constNames, unknownFuncNames));
							return new FuncNode(KnownFuncType.Mult, factors);
						}
						else
						{
							return new FuncNode(randFuncType,
								Generate(curDepth + 1, varName, constNames, unknownFuncNames),
								Generate(curDepth + 1, varName, constNames, unknownFuncNames));
						}
					}
				}
			}
		}
	}
}
