namespace GLSLSyntaxAST.Preprocessor
{
	internal class PreprocessorToken
	{
		internal PreprocessorToken()
		{
			token = 0;
			ival = 0; 
			space = false;
			dval  = 0.0;
			atom = 0;
			space = false;
			name = "";
		}

		internal static int maxTokenLength = 1024;

		internal SourceLocation loc;
		internal int    token;
		internal bool   space;  // true if a space (for white space or a removed comment) should also be recognized, in front of the token returned
		internal int    ival;
		internal double dval;
		internal int    atom;
		internal string name;	
	}
}

