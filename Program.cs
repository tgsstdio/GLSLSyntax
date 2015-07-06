using System;
using Irony.Parsing;

namespace GLSLSyntaxAST
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			var lang = new GLSLGrammar ();
			var parser = new Parser (lang);

			var tree = parser.Parse ("void main(){}");

			Console.WriteLine (tree.ToString ());
		}
	}
}
 