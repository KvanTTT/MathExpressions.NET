using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;

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

		public void CompileFuncAndDerivativeToFile(string expression, string variable, string fileName = "")
		{
			CompileFuncAndDerivativeToFile(expression, variable, Path.GetDirectoryName(fileName), Path.GetFileName(fileName));
		}

		public void CompileFuncAndDerivativeToFile(string expression, string variable, string filePath = "", string name = "")
		{
			CompileFuncAndDerivative(expression, variable, name);
			SaveToFile(filePath, name);
		}

		public byte[] CompileFuncAndDerivativeInMemory(string expression, string variable, string name = "")
		{
			CompileFuncAndDerivative(expression, variable, name);
			return SaveToBytes();
		}

		public void Init(string fileName = "MathFuncLib.dll")
		{
			var name = new AssemblyNameDefinition(Path.GetFileNameWithoutExtension(fileName), new Version(1, 0, 0, 0));
			Assembly = AssemblyDefinition.CreateAssembly(name, fileName, ModuleKind.Dll);

			ImportMath(Assembly);
			InvokeFuncRef = Assembly.MainModule.Import(typeof(Func<double, double>).GetMethod(nameof(System.Reflection.MethodInfo.Invoke)));
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

		public void SaveToFile(string path = "", string fileName = "")
		{
			Assembly.MainModule.Types.Add(Class);
			Assembly.Write(Path.Combine(path, string.IsNullOrEmpty(fileName) ? NamespaceName + ".dll" : fileName));
		}

		public byte[] SaveToBytes()
		{
			Assembly.MainModule.Types.Add(Class);
			byte[] result = null;
			using (var stream = new MemoryStream())
			{
				Assembly.Write(stream);
				result = stream.ToArray();
			}
			return result;
		}

		private void CompileFuncAndDerivative(string expression, string variable, string name = "")
		{
			var func = new MathFunc(expression, variable, true, true);
			var funcDer = new MathFunc(expression, variable, true, false).GetDerivative().GetPrecompilied();

			Init(name);

			func.Compile(this, FuncName);
			funcDer.Compile(this, FuncDerivativeName);
		}

		private void ImportMath(AssemblyDefinition assembly)
		{
			TypesReferences = new Dictionary<KnownFuncType, MethodReference>();
			foreach (var typeMethod in KnownFunc.TypesMethods)
				TypesReferences.Add(typeMethod.Key, assembly.MainModule.Import(typeMethod.Value));
		}
	}
}
