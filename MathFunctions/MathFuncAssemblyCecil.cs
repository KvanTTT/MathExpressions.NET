using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;

namespace MathFunctions
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

		public Dictionary<KnownMathFunctionType, MethodReference> TypesReferences;

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

		public readonly string NamespaceName;
		public readonly string ClassName;

		public MathFuncAssemblyCecil(string namespaceName = "MathFuncLib", string className = "MathFunc")
		{
			NamespaceName = namespaceName;
			ClassName = className;
		}

		public void Init()
		{
			var name = new AssemblyNameDefinition(NamespaceName, new Version(1, 0, 0, 0));
			Assembly = AssemblyDefinition.CreateAssembly(name, NamespaceName + ".dll", ModuleKind.Dll);

			ImportMath(Assembly);
			InvokeFuncRef = Assembly.MainModule.Import(typeof(Func<double, double>).GetMethod("Invoke"));
			DoubleType = Assembly.MainModule.TypeSystem.Double;

			Class = new TypeDefinition(NamespaceName, ClassName,
				TypeAttributes.Public | TypeAttributes.BeforeFieldInit |
				TypeAttributes.Sealed |TypeAttributes.AnsiClass | TypeAttributes.Abstract,
				Assembly.MainModule.TypeSystem.Object);
		}

		public void Finalize(string path, string fileName = "")
		{
			Assembly.MainModule.Types.Add(Class);
			Assembly.Write(Path.Combine(path, string.IsNullOrEmpty(fileName) ? NamespaceName + ".dll" : fileName));
		}

		private void ImportMath(AssemblyDefinition assembly)
		{
			TypesReferences = new Dictionary<KnownMathFunctionType, MethodReference>();
			foreach (var typeMethod in KnownMathFunction.TypesMethods)
				TypesReferences.Add(typeMethod.Key, assembly.MainModule.Import(typeMethod.Value));
		}
	}
}
