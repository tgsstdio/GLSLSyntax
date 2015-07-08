using System;
using Irony.Parsing;
using System.Diagnostics;
using GLSLSyntaxAST.CodeDom;
using System.Reflection;

namespace GLSLSyntaxAST.CommandLine
{
	class MainClass
	{
		static void OldCode ()
		{
			Console.WriteLine ("Hello World!");
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			//var tree = compiler.Parse ("struct Camera { float x; int num; vec3 output; vec2 data[10]; vec4 grid[3][4]; bool samples[]; };");
			var tree = compiler.Parse ("layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};");
			//var tree = compiler.Parse ("uniform vec3 light;");
			CheckNodes (tree.Root, 0);
		}

		static void GenerateAssembly ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLStructGenerator test = new GLSLStructBuilder (lookup);
			test.Initialize ();
			int actual = test.Extract ("layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};");
			GLSLAssembly output = new GLSLAssembly ();
			output.OutputAssembly = "GLSLOutput.dll";
			output.Version = "1.0.0.2";
			output.Namespace = "GLSLOutput";
			output.Path = "";
			output.InMemory = false;
			output.ReferencedAssemblies = new string[]{"OpenTK.dll"};
			test.SaveAsAssembly (output);
		}

		public static void Main (string[] args)
		{
			//OldCode ();
//			try
//			{
//				GenerateAssembly ();
//			}
//			catch (Exception ex)
//			{
//				Debug.WriteLine (ex.Message);
//			}
		//	Test();
			Compare();
		}

		public static void Compare()
		{
			// Create an array of types.
			Type[] types = { typeof(EmptyStruct), typeof(ArgIterator)};

			foreach (var t in types) {
				Console.WriteLine("Attributes for type {0}:", t.Name);

				TypeAttributes attr = t.Attributes;

				// To test for visibility attributes, you must use the visibility mask.
				TypeAttributes visibility = attr & TypeAttributes.VisibilityMask;
				switch (visibility)
				{
				case TypeAttributes.NotPublic:
					Console.WriteLine("   ...is not public");
					break;
				case TypeAttributes.Public:
					Console.WriteLine("   ...is public");
					break;
				case TypeAttributes.NestedPublic:
					Console.WriteLine("   ...is nested and public");
					break;
				case TypeAttributes.NestedPrivate:
					Console.WriteLine("   ...is nested and private");
					break;
				case TypeAttributes.NestedFamANDAssem:
					Console.WriteLine("   ...is nested, and inheritable only within the assembly" +
						"\n         (cannot be declared in C#)");
					break;
				case TypeAttributes.NestedAssembly:
					Console.WriteLine("   ...is nested and internal");
					break;
				case TypeAttributes.NestedFamily:
					Console.WriteLine("   ...is nested and protected");
					break;
				case TypeAttributes.NestedFamORAssem:
					Console.WriteLine("   ...is nested and protected internal");
					break;
				}

				//' Use the layout mask to test for layout attributes.
				TypeAttributes layout = attr & TypeAttributes.LayoutMask;
				switch (layout)
				{
				case TypeAttributes.AutoLayout:
					Console.WriteLine("   ...is AutoLayout");
					break;
				case TypeAttributes.SequentialLayout:
					Console.WriteLine("   ...is SequentialLayout");
					break;
				case TypeAttributes.ExplicitLayout:
					Console.WriteLine("   ...is ExplicitLayout");
					break;
				}

				//' Use the class semantics mask to test for class semantics attributes.
				TypeAttributes classSemantics = attr & TypeAttributes.ClassSemanticsMask;
				switch (classSemantics)
				{
				case TypeAttributes.Class:
					if (t.IsValueType)
					{
						Console.WriteLine("   ...is a value type");
					}
					else
					{
						Console.WriteLine("   ...is a class");
					}
					break;
				case TypeAttributes.Interface:
					Console.WriteLine("   ...is an interface");
					break;
				}

				if (0!=(attr & TypeAttributes.Abstract))
				{
					Console.WriteLine("   ...is abstract");
				}

				if (0!=(attr & TypeAttributes.Sealed))
				{
					Console.WriteLine("   ...is sealed");
				}
				Console.WriteLine();
			}
		}

		public static void CheckNodes(ParseTreeNode node, int level)
		{
			for(int i = 0; i < level; i++)
				Debug.Write("  ");
			Debug.WriteLine (node);

			foreach (ParseTreeNode child in node.ChildNodes)
			{
				CheckNodes (child, level + 1);
			}
		}
	}
}
