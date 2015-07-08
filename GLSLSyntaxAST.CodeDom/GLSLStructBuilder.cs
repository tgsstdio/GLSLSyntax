using System;
using System.Diagnostics;
using Irony.Parsing;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GLSLSyntaxAST.CodeDom
{
	public class GLSLStructBuilder : IGLSLStructGenerator
	{
		public GLSLStructBuilder (IGLSLTypeLookup lookup)
		{
			mTypeLookup = lookup;
		}

		#region IStructGenerator implementation

		public string SaveAsText ()
		{
			throw new NotImplementedException ();
		}

		public List<StructMember> Uniforms {
			get;
			private set;
		}

		private IGLSLTypeLookup mTypeLookup;
		public void Initialize ()
		{
			Uniforms = new List<StructMember> ();
			Blocks = new List<StructInfo> ();
		}

		private bool CheckForUniformTag (StructInfo info, ParseTreeNode child)
		{
			if (child.Term.Name != "type_qualifier")
			{
				return false;
			}

			var layoutQualifier = child.ChildNodes [0];
			ParseTreeNode qualifierlist = layoutQualifier.ChildNodes.Find ((p) => p.Term.Name == "layout_qualifier_id_list");

			if (qualifierlist != null)
			{
				const string FORMAT_REQUIRED = "std140";
				if (qualifierlist.ChildNodes.Find ((p) => p.Token.ValueString == FORMAT_REQUIRED) == null)
				{
					return false;
				} 
				else
				{
					info.LayoutFormat = FORMAT_REQUIRED;
				}
			}
			else
			{
				return false;
			}

			var uniformTag = child.ChildNodes [1].Token;
			if (uniformTag.ValueString != "uniform")
			{
				return false;
			}
			return true;
		}

		private bool ExtractName (StructInfo info, ParseTreeNode child)
		{
			info.Name = child.Token.ValueString;
			return true;
		}

		private bool ExtractMembers (StructInfo info, ParseTreeNode child)
		{
			if (child.Term.Name == "struct_declaration_list")
			{
				var members = child.ChildNodes.FindAll ((p) => p.Term.Name == "struct_declaration");

				info.Members = new List<StructMember> ();
				foreach (var member in members)
				{
					var temp = new StructMember ();
					temp.TypeString = member.ChildNodes [0].Token.ValueString;
					temp.ClosestType = mTypeLookup.FindClosestType (temp.TypeString);

					var declarator = member.ChildNodes [1];
					if (declarator.Term.Name == "struct_declarator")
					{
						// ASSUME type is first child
						temp.Name = declarator.ChildNodes [0].Token.ValueString;
					}

					info.Members.Add (temp);
				}
				return true;
			} 
			else
			{
				return false;
			}
		}

		public List<StructInfo> Blocks { get; private set; }
		private int FindStructs(ParseTreeNode node, int level)
		{
			if (node.Term.Name == "block_structure")
			{
				var temp = new StructInfo ();

				// first child is uniform keyword
				bool isValid = CheckForUniformTag(temp, node.ChildNodes[0]);
				if (isValid)
				{
					// second child is struct type name
					if (!ExtractName (temp, node.ChildNodes [1]))
					{
						return 0;
					}

					// third child is list of member inside
					if (ExtractMembers (temp, node.ChildNodes [2]))
					{
						Blocks.Add (temp);
						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return 0;
				}

			} 
			else
			{
				int total = 0;
				foreach (ParseTreeNode child in node.ChildNodes)
				{
					total += FindStructs (child, level + 1);
				}
				return total;
			}
		}

		public int Extract (string code)
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			var tree = compiler.Parse (code);
			int total = FindStructs (tree.Root, 0);	

			return total;
		}

		public int Extract (System.IO.Stream stream)
		{
			using (var reader = new StreamReader (stream))
			{
				return Extract (reader.ReadToEnd ());
			}
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

		public void SaveAsAssembly (GLSLAssembly assembly)
		{
			using (CSharpCodeProvider provider = new CSharpCodeProvider ())
			{
				// Build the parameters for source compilation.
				CompilerParameters cp = new CompilerParameters();

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
				cp.GenerateInMemory = assembly.InMemory;

				var contentUnit = new CodeCompileUnit ();

				SetVersionNumber (contentUnit, assembly.Version);

				string nameSpace = assembly.Namespace;
				if (string.IsNullOrWhiteSpace (nameSpace))
				{
					nameSpace = System.IO.Path.GetFileNameWithoutExtension (assembly.OutputAssembly);
				}

				var contentNs  = new CodeNamespace(nameSpace);
				contentUnit.Namespaces.Add (contentNs);

				var uniforms = CreateClass (contentNs, "Uniforms");
				CodeTypeConstructor defaultConstructor = new CodeTypeConstructor ();	
				defaultConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				uniforms.Members.Add (defaultConstructor);
			
				defaultConstructor.Statements.Add (new CodeVariableDeclarationStatement (typeof(int), "testInt", new CodePrimitiveExpression (0)));

				foreach (var block in Blocks)
				{
					AddStruct (contentNs, uniforms, block);
				}

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

		}

		#endregion
	}
}

