using System;
using Irony.Parsing;
using System.Diagnostics;

namespace GLSLSyntaxAST.CommandLine
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			//var tree = compiler.Parse ("struct Camera { float x; int num; vec3 output; vec2 data[10]; vec4 grid[3][4]; bool samples[]; };");
			var tree = compiler.Parse ("layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};");
			//var tree = compiler.Parse ("uniform vec3 light;");
			CheckNodes (tree.Root, 0);			
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
