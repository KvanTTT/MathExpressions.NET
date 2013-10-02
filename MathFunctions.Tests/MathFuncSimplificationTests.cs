using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MathFunctions.Tests
{
	[TestFixture]
	public class MathFuncSimplificationTests
	{
		[Test]
		public void SimpleSimplificationTest()
		{
			Assert.IsTrue(new MathFunc("a + 0").Simplify() == "a");
			Assert.IsTrue(new MathFunc("0 + a").Simplify() == "a");
			Assert.IsTrue(new MathFunc("a * 0").Simplify() == "0");
			Assert.IsTrue(new MathFunc("0 * a").Simplify() == "0");
			Assert.IsTrue(new MathFunc("a * 1").Simplify() == "a");
			Assert.IsTrue(new MathFunc("1 * a").Simplify() == "a");
			Assert.IsTrue(new MathFunc("a ^ 0").Simplify() == "1");
			Assert.IsTrue(new MathFunc("a ^ 1").Simplify() == "a");
		}

		[Test]
		public void UnknownFuncsAddTest()
		{
			var f = new MathFunc("arcsin(x) + arccos(x)").Simplify();
			Assert.IsTrue(f == "arcsin(x) + arccos(x)");
		}

		[Test]
		public void SimplicateMultTest()
		{
			var f = new MathFunc("(2 * x ^ 2 - 1) ^ -1 * (2 * x ^ 2 - 1) ^ -1").Simplify();
			Assert.IsTrue(f == "(2 * x ^ 2 + -1) ^ -2");
		}

		[Test]
		public void SimplicateMultTest2()
		{
			var f = new MathFunc("(2 * x ^ 2 - 1) ^ -1 * (2 * x ^ 2 - 1) ^ -1").Simplify();
			Assert.IsTrue(f == "(2 * x ^ 2 + -1) ^ -2");
		}

		[Test]
		public void SubstractionTest()
		{
			var f = new MathFunc("a - a").Simplify();
			Assert.IsTrue(f == "0");
		}

		[Test]
		public void UnknownFuncMultAndDivTest()
		{
			var f = new MathFunc("g(x) * g(x) / g(x)").Simplify();
			Assert.IsTrue(f == "g(x)");
		}

		[Test]
		public void UnknownFuncsMultTest()
		{
			var f = new MathFunc("f(x) * g(x)").Simplify();
			Assert.IsTrue(f == "f(x) * g(x)");
		}

		[Test]
		public void PowerInPowerSimplicateTest()
		{
			var f = new MathFunc("1 / x ^ 2").Simplify();
			Assert.IsTrue(f == "x ^ -2");
		}
	}
}
