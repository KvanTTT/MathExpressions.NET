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
		public double TestFunc(double x)
		{
			return Math.Pow((2 * -x + 1), (2 * x + 1)) * Math.Sin((2 * x + 1) * (2 * x + 1) * (2 * x + 1));
		}

		[Test]
		public void FindIdenticalFuncsTest()
		{
			double x = 3;
			double func, funcDer;
			CompileAndCalculate("Sin(x) + x ^ (Ln(5 * x) - 10 / x)", "x", x, out func, out funcDer);
			double expected = Math.Sin(x) + Math.Pow(x, Math.Log(5 * x) - 10 / x);
			Assert.AreEqual(expected, func);
		}

		private bool CompileAndCalculate(string expression, string variable, double x,
			out double funcResult, out double funcDerivativeResult)
		{
			funcResult = double.NaN;
			funcDerivativeResult = double.NaN;
			string tempDllName = "MathFuncLib.dll";
			try
			{
				var mathAssembly = new MathFuncAssemblyCecil();
				mathAssembly.CompileFuncAndDerivative(expression, variable, "", tempDllName);
				var domain = AppDomain.CreateDomain("MathFuncDomain");
				var pathToDll = tempDllName;
				var mathFuncObj = domain.CreateInstanceFromAndUnwrap(pathToDll, mathAssembly.NamespaceName + "." + mathAssembly.ClassName);
				var mathFuncObjType = mathFuncObj.GetType();
				funcResult = (double)mathFuncObjType.GetMethod(mathAssembly.FuncName).Invoke(mathFuncObj, new object[] { x });
				funcDerivativeResult = (double)mathFuncObjType.GetMethod(mathAssembly.FuncDerivativeName).Invoke(mathFuncObj, new object[] { x });
				AppDomain.Unload(domain);
				File.Delete(tempDllName);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
