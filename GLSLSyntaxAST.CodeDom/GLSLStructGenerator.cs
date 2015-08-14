using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using System.IO;

namespace GLSLSyntaxAST.CodeDom
{
	public class GLSLStructGenerator : IGLSLStructGenerator
	{
		private readonly IGLSLUniformExtractor mExtractor;
		public GLSLStructGenerator (IGLSLUniformExtractor extractor)
		{
			mExtractor = extractor;
		}

		#region IStructGenerator implementation

		public void Initialize ()
		{

		}

		private static CodeTypeDeclaration CreateClass(CodeNamespace contentNs, string folderName)
		{
			var textures = new CodeTypeDeclaration (folderName);
			textures.IsClass = true;
			textures.TypeAttributes = TypeAttributes.Public;
			//textures.Members.

			contentNs.Types.Add (textures);

			return textures;
		}

		private static void SetVersionNumber (CodeCompileUnit contentUnit, string value)
		{
			var attributeType = new CodeTypeReference (typeof(AssemblyVersionAttribute));
			var versionNumber = new CodeAttributeDeclaration (attributeType, new CodeAttributeArgument (new CodePrimitiveExpression (value)));
			contentUnit.AssemblyCustomAttributes.Add (versionNumber);
		}

		private static void AddStruct (CodeNamespace dest, CodeTypeDeclaration folder, StructInfo info)
		{
			var structType = new CodeTypeDeclaration (info.Name);
			//structType.IsClass = false;
			structType.IsStruct = true;
			structType.TypeAttributes = TypeAttributes.Public | TypeAttributes.SequentialLayout | TypeAttributes.Sealed;

//			structType.CustomAttributes.Add(
//				new CodeAttributeDeclaration(typeof(StructLayoutAttribute),
//				new CodeAttributeArgument(
//					new CodeFieldReferenceExpression(
//							new CodeTypeReferenceExpression(typeof(LayoutKind)), "Sequential")				
//					)
//				)
//			);

			dest.Types.Add (structType);

			foreach (var member in info.Members)
			{
				if (member.ClosestType != null)
				{
					var field1 = new CodeMemberField (member.ClosestType, member.Name);
					field1.Attributes = MemberAttributes.Public;
					structType.Members.Add (field1);
				}
			}

//			var localVariable = "m" + alias;
//			var field1 = new CodeMemberField (typeof(string), localVariable);
//			folder.Members.Add (field1);
//
//			CodeMemberProperty property1 = new CodeMemberProperty ();
//			property1.Name = alias;
//			property1.Type = new CodeTypeReference ("System.String");
//			property1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
//			property1.HasGet = true;
//			property1.HasSet = true;
//			property1.GetStatements.Add (new CodeMethodReturnStatement (new CodeFieldReferenceExpression (new CodeThisReferenceExpression (), localVariable)));
//			property1.SetStatements.Add (new CodeAssignStatement (new CodeFieldReferenceExpression (new CodeThisReferenceExpression (), localVariable), new CodePropertySetValueReferenceExpression ()));
//			folder.Members.Add (property1);
		}

		public void SaveAsAssembly (CodeDomProvider provider, GLSLAssembly assembly)
		{
			// Build the parameters for source compilation.
			var cp = new CompilerParameters();

			// Add an assembly reference.
			cp.ReferencedAssemblies.Add( "System.dll" );
			cp.ReferencedAssemblies.Add ("System.Runtime.InteropServices.dll");

			if (assembly.ReferencedAssemblies != null)
			{
				foreach (var assemblyName in assembly.ReferencedAssemblies)
				{
					cp.ReferencedAssemblies.Add( assemblyName );
				}
			}

			// Generate an executable instead of
			// a class library.
			cp.GenerateExecutable = false;

			// Set the assembly file name to generate.
			cp.OutputAssembly = System.IO.Path.Combine(assembly.Path,assembly.OutputAssembly);

			// Save the assembly as a physical file.
			cp.GenerateInMemory = false;

			var contentUnit = InitialiseCompileUnit (assembly);

			// Invoke compilation.
			CompilerResults cr = provider.CompileAssemblyFromDom(cp, contentUnit);

			if (cr.Errors.Count > 0)
			{
				Debug.WriteLine(string.Format("Source built into {0} unsuccessfully.", cr.PathToAssembly));				
				// Display compilation errors.
				foreach (CompilerError ce in cr.Errors)
				{
					Debug.WriteLine("  {0}", ce.ToString());		
				}
			}
			else
			{
				Debug.WriteLine(string.Format("Source built into {0} successfully.", cr.PathToAssembly));
			}
		}

		public void SaveAsCode (CodeDomProvider provider, GLSLAssembly assembly, IGLSLUniformExtractor extractor, CodeGeneratorOptions options)
		{
			string outputFile = System.IO.Path.GetFileNameWithoutExtension (assembly.OutputAssembly) + ".cs";
			string absolutePath = System.IO.Path.Combine(assembly.Path,outputFile);

			using (var fs = File.OpenWrite (absolutePath))
			using (var writer = new StreamWriter(fs))
			{
				var contentUnit = InitialiseCompileUnit (assembly);
				provider.GenerateCodeFromCompileUnit (contentUnit, writer, options);
			}
		}

		public CodeCompileUnit InitialiseCompileUnit (GLSLAssembly assembly)
		{
			var contentUnit = new CodeCompileUnit ();
			SetVersionNumber (contentUnit, assembly.Version);
			string nameSpace = assembly.Namespace;
			if (string.IsNullOrWhiteSpace (nameSpace))
			{
				nameSpace = System.IO.Path.GetFileNameWithoutExtension (assembly.OutputAssembly);
			}
			var contentNs = new CodeNamespace (nameSpace);
			contentUnit.Namespaces.Add (contentNs);
			var uniforms = CreateClass (contentNs, "Uniforms");
			var defaultConstructor = new CodeTypeConstructor ();
			defaultConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			uniforms.Members.Add (defaultConstructor);
			defaultConstructor.Statements.Add (new CodeVariableDeclarationStatement (typeof(int), "testInt", new CodePrimitiveExpression (0)));
			foreach (var block in mExtractor.Blocks)
			{
				AddStruct (contentNs, uniforms, block);
			}
			return contentUnit;
		}
		#endregion
	}
}

