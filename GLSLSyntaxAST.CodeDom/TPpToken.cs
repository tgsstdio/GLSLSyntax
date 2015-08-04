using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class TPpToken
	{
		public TPpToken()
		{
			token = 0;
			ival = 0; 
			space = false;
			dval  = 0.0;
			atom = 0;
			space = false;
			name = "";
		}

		public bool Matches(TPpToken right)
		{
			return token == right.token && atom == right.atom &&
				ival == right.ival && Math.Abs (dval - right.dval) < Double.Epsilon &&
				name == right.name;
		}

		public static int maxTokenLength = 1024;

		public TSourceLoc loc;
		public int    token;
		public bool   space;  // true if a space (for white space or a removed comment) should also be recognized, in front of the token returned
		public int    ival;
		public double dval;
		public int    atom;
		public string name;	
	}
}

