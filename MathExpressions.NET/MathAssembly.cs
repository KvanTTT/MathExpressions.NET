using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MathExpressionsNET
{
	public class MathAssembly : IDisposable
	{
		private static string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private static Random _rand = new Random();

		private AppDomain _domain;
		private string _fileName = "MathFuncLib.dll";
		private object _mathFuncObj;

		public MethodInfo FuncMethodInfo
		{
			get;
			private set;
		}

		public MethodInfo FuncDerivativeMethodInfo
		{
			get;
			private set;
		}

		public double Func(params object[] x)
		{
			return (double)FuncMethodInfo.Invoke(_mathFuncObj, x);
		}

		public double FuncDerivative(params object[] x)
		{
			return (double)FuncDerivativeMethodInfo.Invoke(_mathFuncObj, x);
		}

		public MathAssembly(string expression, string variable)
		{
			var mathAssembly = new MathFuncAssemblyCecil();
			_fileName = "MathFuncLib" + "_" + GenerateRandomString(6) + ".dll";
			mathAssembly.CompileFuncAndDerivative(expression, variable, "", _fileName);
			_domain = AppDomain.CreateDomain("MathFuncDomain");
			_mathFuncObj = _domain.CreateInstanceFromAndUnwrap(_fileName, mathAssembly.NamespaceName + "." + mathAssembly.ClassName);
			var mathFuncObjType = _mathFuncObj.GetType();
			FuncMethodInfo = mathFuncObjType.GetMethod(mathAssembly.FuncName);
			FuncDerivativeMethodInfo = mathFuncObjType.GetMethod(mathAssembly.FuncDerivativeName);
		}

		public void Dispose()
		{
			if (_domain != null)
				AppDomain.Unload(_domain);
			if (File.Exists(_fileName))
				File.Delete(_fileName);
		}

		~MathAssembly()
		{
			Dispose();
		}

		public static string GenerateRandomString(int length)
		{
			byte[] bytes = new byte[length];
			var random = new Random();
			random.NextBytes(bytes);
			StringBuilder result = new StringBuilder(length);
			foreach (var b in bytes)
				result.Append(_chars[b % _chars.Length]);

			return result.ToString();
		}
	}
}
