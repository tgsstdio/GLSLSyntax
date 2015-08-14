using System;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.DebugTree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();			
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			test.DebugCode ("in vec3 v_normal;");
		}
	}
}
