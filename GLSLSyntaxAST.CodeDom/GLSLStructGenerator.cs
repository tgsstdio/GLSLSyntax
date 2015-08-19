using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

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

		private static CodeTypeDeclaration CreateClassType(CodeNamespace contentNs, string folderName)
		{
			var textures = new CodeTypeDeclaration (folderName);
			textures.IsClass = true;
			textures.TypeAttributes = TypeAttributes.Public;

			return textures;
		}

		private static void SetVersionNumber (CodeCompileUnit contentUnit, string value)
		{
			var attributeType = new CodeTypeReference (typeof(AssemblyVersionAttribute));
			var versionNumber = new CodeAttributeDeclaration (attributeType, new CodeAttributeArgument (new CodePrimitiveExpression (value)));
			contentUnit.AssemblyCustomAttributes.Add (versionNumber);
		}

		private static void AddStruct (CodeNamespace dest, StructInfo info)
		{
			if (info.StructType == GLSLStructType.Struct)
			{
				var structType = new CodeTypeDeclaration (info.Name);
				//structType.IsClass = false;
				structType.IsStruct = true;
				structType.TypeAttributes = TypeAttributes.Public | TypeAttributes.SequentialLayout | TypeAttributes.Sealed;
				var argument = new CodeAttributeArgument (new CodeFieldReferenceExpression (new CodeTypeReferenceExpression (typeof(LayoutKind)), "Sequential"));
				structType.CustomAttributes.Add (new CodeAttributeDeclaration (new CodeTypeReference (typeof(StructLayoutAttribute)), argument));

				dest.Types.Add (structType);

				foreach (var member in info.Members)
				{
					if (member.ClosestType != null)
					{
						var field1 = new CodeMemberField (member.ClosestType, member.Name);
						field1.Attributes = MemberAttributes.Public;
						structType.Members.Add (field1);
					}
					// single reference
					else if (member.ArrayDetails == null)
					{
						var arrayType = new CodeTypeReference (member.TypeString);
						var field1 = new CodeMemberField (arrayType, member.Name);
						field1.Attributes = MemberAttributes.Public;
						structType.Members.Add (field1);					
					}

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
			string outputFile = Path.GetFileNameWithoutExtension (assembly.OutputAssembly) + ".cs";
			string absolutePath = Path.Combine(assembly.Path,outputFile);

			using (var writer = new StreamWriter(absolutePath, false))
			{
				var contentUnit = InitialiseCompileUnit (assembly);

				provider.GenerateCodeFromCompileUnit (contentUnit, writer, options);
			}
		}

		void DeclareStructs (CodeNamespace contentNs)
		{
			foreach (var block in mExtractor.Blocks)
			{
				AddStruct (contentNs, block);
			}
		}

		public CodeCompileUnit InitialiseCompileUnit (GLSLAssembly assembly)
		{
			var contentUnit = new CodeCompileUnit ();
			var globalNs = new CodeNamespace ();
			globalNs.Comments.Add(new CodeCommentStatement("Namespace Comment"));
			contentUnit.Namespaces.Add (globalNs);
			contentUnit.ReferencedAssemblies.Add ("System.Runtime.InteropServices.dll");
			globalNs.Imports.Add(new CodeNamespaceImport("System.Runtime.InteropServices"));

			SetVersionNumber (contentUnit, assembly.Version);
			string nameSpace = assembly.Namespace;
			if (string.IsNullOrWhiteSpace (nameSpace))
			{
				nameSpace = System.IO.Path.GetFileNameWithoutExtension (assembly.OutputAssembly);
			}
			var contentNs = new CodeNamespace (nameSpace);


			contentUnit.Namespaces.Add (contentNs);

			DeclareStructs (contentNs);

			var program = CreateClassType (contentNs, "Program");
			foreach (var member in mExtractor.Uniforms)
			{
				var field1 = new CodeMemberField (typeof(int), member.Name);
				field1.Attributes = MemberAttributes.Public;
				program.Members.Add (field1);
			}

			foreach (var member in mExtractor.Blocks)
			{
				if (member.StructType == GLSLStructType.Buffer)
				{
					var field1 = new CodeMemberField (typeof(int), member.Name);
					field1.Attributes = MemberAttributes.Public;
					program.Members.Add (field1);
				}
			}
			contentNs.Types.Add (program);


			var uniforms = CreateClassType (contentNs, "Uniforms");
			var inputBindings = CreateClassType (contentNs, "InputBindings");
			var outputBindings = CreateClassType (contentNs, "OutputBindings");

			var defaultConstructor = new CodeConstructor ();
			defaultConstructor.Attributes = MemberAttributes.Public;

			AddUniforms (uniforms, defaultConstructor);

			AddAttributes (inputBindings, outputBindings);

			if (uniforms.Members.Count > 0)
			{
				uniforms.Members.Add (defaultConstructor);
				contentNs.Types.Add (uniforms);
			}

			if (inputBindings.Members.Count > 0)
				contentNs.Types.Add (inputBindings);

			if (outputBindings.Members.Count > 0)
				contentNs.Types.Add (outputBindings);

			//defaultConstructor.Statements.Add (new CodeVariableDeclarationStatement (typeof(int), "testInt", new CodePrimitiveExpression (0)));

			return contentUnit;
		}
		#endregion

		static void AddUniformMember (CodeTypeDeclaration dest, StructMember member, CodeConstructor defaultConstructor)
		{
			if (member.ClosestType != null)
			{
				var field1 = new CodeMemberField (member.ClosestType, member.Name);
				field1.Attributes = MemberAttributes.Public;
				dest.Members.Add (field1);
			}
			else if (member.ArrayDetails != null)
			{
				var arrayType = new CodeTypeReference (member.ArrayDetails.StructType.Name + "[]");
				var field1 = new CodeMemberField (arrayType, member.Name);
				field1.Attributes = MemberAttributes.Public;
				dest.Members.Add (field1);
				defaultConstructor.Statements.Add (new CodeVariableDeclarationStatement (arrayType, member.Name, new CodeArrayCreateExpression (arrayType, member.ArrayDetails.ArraySize)));
			}
		}

		void AddUniforms (CodeTypeDeclaration dest, CodeConstructor defaultConstructor)
		{
			foreach (var member in mExtractor.Uniforms)
			{
				AddUniformMember (dest, member, defaultConstructor);
			}
		}

		void AddAttributes (CodeTypeDeclaration inputBindings, CodeTypeDeclaration outputBindings)
		{
			foreach (var member in mExtractor.Attributes)
			{
				if (member.ClosestType != null)
				{
					if (member.Layout != null)
					{
						if (member.Direction == "in" || member.Direction == "inout")
						{
							AddLocationIndex (member, inputBindings);
						}
						if (member.Direction == "out" || member.Direction == "inout")
						{
							AddLocationIndex (member, outputBindings);
						}
					}
				}
			}
			foreach (var member in mExtractor.Blocks)
			{
				if (member.StructType == GLSLStructType.Buffer)
				{
					if (member.Layout != null)
					{
						var field1 = new CodeMemberField (typeof(int), member.Name);
						field1.Attributes = MemberAttributes.Public;
						if (member.Layout.Binding.HasValue)
						{
							field1.InitExpression = new CodePrimitiveExpression (member.Layout.Binding.Value);
						}
						inputBindings.Members.Add (field1);
						outputBindings.Members.Add (field1);
					}
				}
			}

		}

		static void AddLocationIndex (InputAttribute member, CodeTypeDeclaration dest)
		{
			var field1 = new CodeMemberField (typeof(int), member.Name);
			field1.Attributes = MemberAttributes.Public;
			if (member.Layout.Location.HasValue)
			{
				field1.InitExpression = new CodePrimitiveExpression (member.Layout.Location.Value);
			}
			dest.Members.Add (field1);
		}
	}
}

