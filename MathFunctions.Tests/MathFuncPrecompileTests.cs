using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace MathFunctions.Tests
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
			Assert.IsTrue(double.Parse(func.ToString()) == 0.995483012754421);
		}

		[Test]
		public static void PrecompileTest2()
		{
			MathFunc func = new MathFunc("-(sin(x) ^ -2 * x) + -(sin(x) ^ -1 * cos(x)) + cos(x) ^ -2 * x + cos(x) ^ -1 * sin(x) ^ -1");
			Assert.IsTrue(WolframAlphaUtils.CheckEquality(func.ToString(), func.GetPrecompilied().ToString()));
		}
	}
}
