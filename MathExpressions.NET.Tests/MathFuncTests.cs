using NUnit.Framework;

namespace MathExpressionsNET.Tests
{
	[TestFixture]
	public class MathFuncTests
	{
		MathExprConverter Parser;

		[SetUp]
		public void Init()
		{
			Parser = new MathExprConverter();
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
		public void IsCalculatedTest()
		{
			Assert.IsTrue(new MathFunc("3 + sin(5 + 7 ^ 0.342345 - sqrt(2)) * 3 * 1").IsCalculated);
		}

		[Test]
		public void IsNotCalculatedTest()
		{
			Assert.IsFalse(new MathFunc("3 + f(5 + 7 ^ 0.342345 - sqrt(2) + x) * 3 * 1").IsCalculated);
		}

		[Test]
		public void SortTest()
		{
			var f = new MathFunc("sqrt(2) + x^2 + 1");
			Assert.IsTrue(f == "x^2 + sqrt(2) + 1");
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
