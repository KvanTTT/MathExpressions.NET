using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Security.Policy;
using System.IO;

namespace MathFunctions.Tests
{
	[TestFixture]
	public class MathFuncCompilationTests
	{
		[SetUp]
		public void Init()
		{
			Helper.InitDefaultDerivatives();
		}

		[Test]
		public void TestFunc(double x)
		{
			//return Math.Pow((2 * -x + 1), (2 * x + 1)) * Math.Sin((2 * x + 1) * (2 * x + 1) * (2 * x + 1));
		}

		[Test]
		public void CompileFuncTest1()
		{
			var expectedFunc = new Func<double, double>(x => Math.Sin(x) + Math.Pow(x, Math.Log(5 * x) - 10 / x));
			using (var mathAssembly = new MathAssembly("Sin(x) + x ^ (Ln(5 * x) - 10 / x)", "x"))
			{
				for (int i = 1; i < 10; i++)
					Assert.AreEqual(expectedFunc(i), mathAssembly.SimpleFunc(i));
			}
		}
	}
}
