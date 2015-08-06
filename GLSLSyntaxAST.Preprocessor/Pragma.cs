using System.Collections.Generic;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class Pragma
	{
		internal Pragma(bool o, bool d)
		{
			optimize = o;
			debug = d;
			pragmaTable = new Dictionary<string, string> ();
		}
		internal bool optimize;
		internal bool debug;
		internal Dictionary<string, string> pragmaTable;
	}
}
