using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MathFunctions.Tests
{
	[TestFixture]
	public class MathFuncCompilationTests
	{
		//[Test]
		public double TestFunc(double x)
		{
			return Math.Pow((2 * -x + 1), (2 * x + 1)) * Math.Sin((2 * x + 1) * (2 * x + 1) * (2 * x + 1));
		}

		[Test]
		public void FindIdenticalFuncsTest()
		{
			var mathFunc = new MathFunc("(2 * -x + 1) ^ (2 * x + 1) * Sin((2 * x + 1) ^ 3)");
			//var mathFunc = new MathFunc("(2 * x + 1) * (2 * x + 1) * (3 * (x + 5) + x)");

			//mathFunc.Compile();
		}
	}
}
