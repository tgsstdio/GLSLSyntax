using System;

namespace GLSLSyntaxAST.Preprocessor
{
	public struct MacroSymbol {
		public int argc;
		public int[] args;
		public TokenStream body;
		public bool busy;
		public bool undef;
	};
}

