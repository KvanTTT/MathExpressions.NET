using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;
using Mono.Cecil.Cil;

namespace MathExpressionsNET
{
	public class MathFuncAssemblyCecil
	{
		public AssemblyDefinition Assembly
		{
			get;
			private set;
		}

		public TypeDefinition Class
		{
			get;
			private set;
		}

		public Dictionary<KnownFuncType, MethodReference> TypesReferences;

		public MethodReference InvokeFuncRef
		{
			get;
			private set;
		}

		public TypeReference DoubleType
		{
			get;
			private set;
		}

		public string NamespaceName = "MathFuncLib";
		public string ClassName = "MathFunc";
		public string FuncName = "Func";
		public string FuncDerivativeName = "FuncDerivative";

		public MathFuncAssemblyCecil(string namespaceName = "MathFuncLib", string className = "MathFunc")
		{
			NamespaceName = namespaceName;
			ClassName = className;
		}

		public void CompileFuncAndDerivative(string expression, string variable, string path = "", string fileName = "")
		{
			var func = new MathFunc(expression, variable, true, true);
			var funcDer = new MathFunc(expression, variable, true, false).GetDerivative().GetPrecompilied();

			Init(fileName);

			func.Compile(this, FuncName);
			funcDer.Compile(this, FuncDerivativeName);

			Finalize(path, fileName);
		}

		public void Init(string fileName = "MathFuncLib.dll")
		{
			var name = new AssemblyNameDefinition(fileName.Replace(".dll", ""), new Version(1, 0, 0, 0));
			Assembly = AssemblyDefinition.CreateAssembly(name, fileName, ModuleKind.Dll);

			ImportMath(Assembly);
			InvokeFuncRef = Assembly.MainModule.Import(typeof(Func<double, double>).GetMethod("Invoke"));
			DoubleType = Assembly.MainModule.TypeSystem.Double;

			Class = new TypeDefinition(NamespaceName, ClassName,
				TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable |
				TypeAttributes.AnsiClass, Assembly.MainModule.TypeSystem.Object);

			var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
			var method = new MethodDefinition(".ctor", methodAttributes, Assembly.MainModule.TypeSystem.Void);
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, Assembly.MainModule.Import(typeof(object).GetConstructor(new Type[0]))));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			Class.Methods.Add(method);
		}

		public void Finalize(string path = "", string fileName = "")
		{
			Assembly.MainModule.Types.Add(Class);
			Assembly.Write(Path.Combine(path, string.IsNullOrEmpty(fileName) ? NamespaceName + ".dll" : fileName));
		}

		private void ImportMath(AssemblyDefinition assembly)
		{
			TypesReferences = new Dictionary<KnownFuncType, MethodReference>();
			foreach (var typeMethod in KnownFunc.TypesMethods)
				TypesReferences.Add(typeMethod.Key, assembly.MainModule.Import(typeMethod.Value));
		}
	}
}
