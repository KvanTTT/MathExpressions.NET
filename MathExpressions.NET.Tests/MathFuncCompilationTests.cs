using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Security.Policy;
using System.IO;

namespace MathExpressions.NET.Tests
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
		public void CompileFunc()
		{
			var expectedFunc = new Func<double, double>(x => Math.Sin(x) + Math.Pow(x, Math.Log(5 * x) - 10 / x));
			using (var mathAssembly = new MathAssembly("Sin(x) + x ^ (Ln(5 * x) - 10 / x)", "x"))
			{
				for (int i = 1; i < 10; i++)
					Assert.AreEqual(expectedFunc(i), mathAssembly.SimpleFunc(i));
			}
		}

		[Test]
		public void CompileFuncWithParameter()
		{
			var expectedFunc = new Func<double, double, double, double>((x, a, b) =>
				Math.Cos(x * b) * b - Math.Log(a) / Math.Pow(Math.Log(x), 2) / x); // derivative of log(x, a) + sin(x * b)
			using (var mathAssembly = new MathAssembly(new MathFunc("log(x, a) + sin(x * b)", "x").GetDerivative().ToString(), "x"))
			{
				Assert.AreEqual(expectedFunc(5, 3, 4), mathAssembly.Func.Invoke(null, new object[] { 5, 3, 4 }));
			}
		}

		[Test]
		public void CompileFuncWithUnknownFunc()
		{
			double delta = 0.000001;
			var expectedFunc = new Func<double, Func<double, double>, double>((x, f) => 
				Math.Cos(f(x)) * (f(x + delta) - f(x)) / delta); // derivative of sin(f(x))
			using (var mathAssembly = new MathAssembly("sin(a(x))", "x"))
			{
				var func = new Func<double, double>(x => x * x);
				Assert.AreEqual(expectedFunc(5, func), mathAssembly.FuncDerivative.Invoke(null, new object[] { 5, func }));
			}
		}

		[Test]
		public void CompileSampleFunc()
		{
			var exprString = "x ^ 3 + sin(3 * ln(x * 1)) + x ^ ln(2 * sin(3 * ln(x))) - 2 * x ^ 3";
			var dotnetFunc = new Func<double, double>(x =>
				Math.Pow(x, 3) + Math.Sin(3 * Math.Log(x * 1)) + Math.Pow(x, Math.Log(2 * Math.Sin(3 * Math.Log(x))) - 2 * Math.Pow(x, 3)));
			using (var mathAssembly = new MathAssembly(exprString, "x"))
			{
				var rand = new Random();
				int i = 0;
				while (i < 5)
				{
					double x = rand.NextDouble();
					var dotnet = dotnetFunc(x);
					if (!double.IsNaN(dotnet))
					{
						var correct = WolframAlphaUtils.GetValue(exprString, new KeyValuePair<string, double>("x", x));
						var actual = mathAssembly.SimpleFunc(x);
						Assert.LessOrEqual(Math.Abs(correct - actual), Math.Abs(dotnet - actual));
						i++;
					}
				}
			}
		}
	}
}
