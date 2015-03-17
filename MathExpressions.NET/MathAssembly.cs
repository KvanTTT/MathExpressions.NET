using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MathExpressionsNET
{
    [Serializable]
    public class MathAssembly : IDisposable
    {
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
            var fileName = "MathFuncLib" + "_" + Guid.NewGuid().ToString() + ".dll";
            var assemblyBytes = mathAssembly.CompileFuncAndDerivativeInMemory(expression, variable, fileName);
            var assembly = Assembly.Load(assemblyBytes);
            _mathFuncObj = assembly.CreateInstance(mathAssembly.NamespaceName + "." + mathAssembly.ClassName);
            var mathFuncObjType = _mathFuncObj.GetType();
            FuncMethodInfo = mathFuncObjType.GetMethod(mathAssembly.FuncName);
            FuncDerivativeMethodInfo = mathFuncObjType.GetMethod(mathAssembly.FuncDerivativeName);
        }

        public void Dispose()
        {
        }

        ~MathAssembly()
        {
            Dispose();
        }
    }
}