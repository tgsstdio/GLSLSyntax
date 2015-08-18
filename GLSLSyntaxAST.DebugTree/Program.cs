using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.DebugTree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			var compiler = new Parser (new GLSLGrammar());
//
//			Debug.WriteLine(ParserDataPrinter.PrintStateList(compiler.Language));

			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();			
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			test.DebugCode (
				@"layout(binding = 1, std430) buffer LinkedList
{
	NodeType nodes[];
	// Padding[]
};"
			);
		}
	}
}
