using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace MathFunctions.Tests
{
	[TestFixture]
	public class MathFuncTests
	{
		MathExprParser Parser;

		[SetUp]
		public void InitParser()
		{
			Parser = new MathExprParser();
		}

		[Test]
		public void IsValueTest()
		{
			Assert.IsTrue(new MathFunc("100").IsValue);
		}

		[Test]
		public void IsNotValueTest()
		{
			Assert.IsFalse(new MathFunc("a").IsValue);
		}

		[Test]
		public void IsValueTest2()
		{
			Assert.IsTrue(new MathFunc("3 + f(5 + 7 ^ 0.342345 - sqrt(2)) * 3 * 1").IsValue);
		}

		[Test]
		public void IsNotValueTest2()
		{
			Assert.IsFalse(new MathFunc("3 + f(5 + 7 ^ 0.342345 - sqrt(2) + x) * 3 * 1").IsValue);
		}

		[Test]
		public void SortTest()
		{
			var f = new MathFunc("sqrt(2) + x^2 + 1");
			Assert.IsTrue(f.ToString() == "x^2 + sqrt(2) + 1");
		}

		[Test]
		public void ToStringTest()
		{
			var f = new MathFunc("sin(f(x) + g(x))");
			Assert.IsTrue(f.ToString() == "sin(f(x) + g(x))");
		}

		[Test]
		public void ToStringTest2()
		{
			var f = new MathFunc("x1 + x2 + x3 * x4 * (x5 + x6 + x7) * x8");
			Assert.IsTrue(f.ToString() == "(x5 + x6 + x7) * x3 * x4 * x8 + x1 + x2");
		}
	}
}
