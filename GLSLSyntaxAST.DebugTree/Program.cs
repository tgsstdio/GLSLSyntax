using System;
using GLSLSyntaxAST.CodeDom;
using Irony.Parsing;
using System.Diagnostics;

namespace GLSLSyntaxAST.DebugTree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//var compiler = new Parser (new GLSLGrammar());

			//Debug.WriteLine(ParserDataPrinter.PrintStateList(compiler.Language));

			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();			
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			test.DebugCode (
				"void main() { float in_position = 1.0; }"	
			);
		}
	}
}
