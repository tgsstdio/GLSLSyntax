using System;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntax.Preprocessor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var preprocessor = new Standalone ();
			string result = null;
			if (preprocessor.Run ("Sample.vert", out result))
			{
				Console.WriteLine (result);
			}

		}
	}
}
