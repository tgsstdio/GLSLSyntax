using System;
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

		static void AddBindlessTextures (OpenTKTypeLookup lookup)
		{
			var texTypes = new string[] {
				"sampler1d",
				"sampler2d",
				"sampler3d",
				"samplercube",
				"sampler1dshadow",
				"sampler2dshadow",
				"samplercubeshadow",
				"sampler1darray",
				"sampler2darray",
				"sampler1darrayshadow",
				"sampler2darrayshadow",
				"samplercubearray",
				"samplercubearrayshadow",
			};
			foreach (var tex in texTypes)
			{
				lookup.AddNewTranslation (tex, typeof(long));
			}
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
				var shaderFiles = parser.Parse(args);

				var lookup = new OpenTKTypeLookup ();
				lookup.Initialize ();

				AddBindlessTextures (lookup);


				var extractor = new GLSLUniformExtractor (lookup);
				extractor.Initialize ();

				var debug = new InfoSinkBase (SinkType.StdOut);
				var info = new InfoSinkBase (SinkType.StdOut);
				var infoSink = new InfoSink (info, debug);
				var preprocessor = InitialisePreprocessor (infoSink);

				foreach (var fileName in shaderFiles)
				{
					using (var fs = File.Open(fileName, FileMode.Open))
					{
						var stage = Standalone.FindLanguage(fileName);
						string result;
						preprocessor.Run(fs, stage, out result);

						int actual = extractor.Extract (result);
						Console.WriteLine("{0} - no of blocks extracted : {1}", fileName, actual);
					}
				}

				var output = new GLSLAssembly ();
				output.Version = "1.0.0.1";
				output.Namespace = parser.Namespace;
				output.ReferencedAssemblies = new string[]{"OpenTK.dll"};

				var generator = new GLSLStructGenerator(extractor);
				if (parser.GenerateAssembly)
				{
					var fileName = parser.AssemblyFileName;
					output.Path = Path.GetPathRoot(fileName);
					output.OutputAssembly = Path.GetFileName(fileName);

					using (var provider = new CSharpCodeProvider ())
					{
						generator.SaveAsAssembly(provider, output);
					}
				}

				if (parser.GenerateCode)
				{
					var fileName = parser.SourceFileName;

					output.OutputAssembly = Path.GetFileName(fileName);
					output.Path = Path.GetPathRoot(fileName);

					using (var provider = new CSharpCodeProvider ())
					{
						//generator.SaveAsAssembly (provider, output);
						var options = new CodeGeneratorOptions();
						options.BlankLinesBetweenMembers = true;
						generator.SaveAsCode(provider, output, extractor, options);
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
				PrintHelp ();
				returnCode = 1;
			}

			return returnCode;
		}
	}
}
