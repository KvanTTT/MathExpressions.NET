using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MathFunctions
{
	public class MathAssembly : IDisposable
	{
		private AppDomain _domain;
		private string _fileName = "MathFuncLib.dll";
		private object _mathFuncObj;

		public MethodInfo Func
		{
			get;
			private set;
		}

		public MethodInfo FuncDerivative
		{
			get;
			private set;
		}

		public double SimpleFunc(double x)
		{
			return (double)Func.Invoke(_mathFuncObj, new object[] { x });
		}

		public double SimpleFuncDerivative(double x)
		{
			return (double)FuncDerivative.Invoke(_mathFuncObj, new object[] { x });
		}

		public MathAssembly(string expression, string variable)
		{
			var mathAssembly = new MathFuncAssemblyCecil();
			mathAssembly.CompileFuncAndDerivative(expression, variable, "", _fileName);
			_domain = AppDomain.CreateDomain("MathFuncDomain");
			_mathFuncObj = _domain.CreateInstanceFromAndUnwrap(_fileName, mathAssembly.NamespaceName + "." + mathAssembly.ClassName);
			var mathFuncObjType = _mathFuncObj.GetType();
			Func = mathFuncObjType.GetMethod(mathAssembly.FuncName);
			FuncDerivative = mathFuncObjType.GetMethod(mathAssembly.FuncDerivativeName);
		}

		public void Dispose()
		{
			if (_domain != null)
				AppDomain.Unload(_domain);
			File.Delete(_fileName);
		}
	}
}
