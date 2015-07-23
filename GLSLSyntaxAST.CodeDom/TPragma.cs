using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class TPragma
	{
		public TPragma(bool o, bool d)
		{
			optimize = o;
			debug = d;
			pragmaTable = new Dictionary<string, string> ();
		}
		public bool optimize;
		public bool debug;
		public Dictionary<string, string> pragmaTable;
	}
}
