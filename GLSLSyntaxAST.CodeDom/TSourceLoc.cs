using System;

namespace GLSLSyntaxAST.CodeDom
{
	public struct TSourceLoc
	{
		public char[] name;
		public int stringBias;
		public int line;
		public int column;
	}
}

