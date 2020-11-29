using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace MathExpressionsNET.Tests
{
	[TestFixture]
	public class MathFuncPrecompileTests
	{
		[SetUp]
		public static void Init()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}

		[Test]
		public static void FoldConstantsTest()
		{
			MathFunc func = new MathFunc("cos(ln(sin(2)))").GetPrecompilied();
			Assert.AreEqual(0.99548301275442141d, double.Parse(func.ToString()));
		}

		[Test]
		public static void PrecompileTest2()
		{
			MathFunc func = new MathFunc("-(sin(x) ^ -2 * x) + -(sin(x) ^ -1 * cos(x)) + cos(x) ^ -2 * x + cos(x) ^ -1 * sin(x) ^ -1");
			Assert.IsTrue(WolframAlphaUtils.CheckEquality(func.ToString(), func.GetPrecompilied().ToString()));
		}
	}
}
