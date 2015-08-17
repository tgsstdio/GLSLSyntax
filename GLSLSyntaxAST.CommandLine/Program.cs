using System;
using Irony.Parsing;
using System.Diagnostics;
using GLSLSyntaxAST.CodeDom;
using System.Reflection;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using GLSLSyntaxAST.Preprocessor;

namespace GLSLSyntaxAST.CommandLine
{
	class MainClass
	{
		static Standalone InitialisePreprocessor (InfoSink infoSink)
		{

			var intermediate = new GLSLIntermediate ();
			var symbols = new SymbolLookup ();
			symbols.SetPreambleManually (Profile.CoreProfile);
			symbols.DefineAs ("GL_ARB_shader_storage_buffer_object", 1);
			return new Standalone (infoSink, intermediate, symbols);
		}

		static void PrintHelp ()
		{
			Console.WriteLine ("Invalid arguments");
			Console.WriteLine ("[optional arguments] -F {1} {n}... ");
			Console.WriteLine (" -F = any argument after this switch will be processed as a shader file");
			Console.WriteLine (" {1} => glsl shader file 1");
			Console.WriteLine (" {n} => glsl shader file n");
			Console.WriteLine ("==================================");
			Console.WriteLine (" [optional arguments]");
			Console.WriteLine (" Must appear before -F switch");
			Console.WriteLine (" -c {file} => will generate C# source code");
			Console.WriteLine (" i.e. -c Sample.c => Sample.c contain generated classes");
			Console.WriteLine (" -a {file} => will generate C# assembly ");
			Console.WriteLine (" i.e. -a Program.dll => Program.dll contain generated classes");
			Console.WriteLine (" -ns {file} => namespace for the classes ");
		}

		public static int Main (string[] args)
		{
			int returnCode = 0;

			try
			{
				var parser = new ArgumentParser ();
				parser.Parse(args);
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
				PrintHelp ();
				returnCode = 1;
			}

			return returnCode;

//			try
//			{


			foreach(var arg in args)
			{
				Console.WriteLine(arg);
			}

			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var extractor = new GLSLUniformExtractor (lookup);
			extractor.Initialize ();

			var debug = new InfoSinkBase (SinkType.StdOut);
			var info = new InfoSinkBase (SinkType.StdOut);
			var infoSink = new InfoSink (info, debug);
			var preprocessor = InitialisePreprocessor (infoSink);

			for (int i = 1; i < args.Length; ++i)
			{
				var fileName = args[i];
				using (var fs = File.Open(fileName, FileMode.Open))
				{
					var stage = Standalone.FindLanguage(fileName);
					string result;
					preprocessor.Run(fs, stage, out result);

					int actual = extractor.Extract (result);
					Console.WriteLine("{0} - no of blocks extracted : {1}", fileName, actual);
				}
			}

			GLSLAssembly output = new GLSLAssembly ();
			output.OutputAssembly = System.IO.Path.GetFileName(args[0]);
			output.Version = "1.0.0.1";
			output.Namespace = "";
			output.Path = System.IO.Path.GetPathRoot(args[0]);
			output.ReferencedAssemblies = new string[]{"OpenTK.dll"};

			IGLSLStructGenerator generator = new GLSLStructGenerator(extractor);
			using (var provider = new CSharpCodeProvider ())
			{
				//generator.SaveAsAssembly (provider, output);
				var options = new CodeGeneratorOptions();
				options.BlankLinesBetweenMembers = true;
				generator.SaveAsCode(provider, output, extractor, options);
			}

				return 0;
//			}
//			catch (Exception ex)
//			{
//				Debug.WriteLine (ex.Message);
//				return 1;
//			}
		//	Test();
		}

		public static void Compare(Type entityType)
		{
			// Create an array of types.
			Type[] types = { entityType, typeof(ArgIterator)};

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
	}
}
