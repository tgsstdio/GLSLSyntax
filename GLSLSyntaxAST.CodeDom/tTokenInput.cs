using System;
using System.Diagnostics;

namespace GLSLSyntaxAST.CodeDom
{
	public class tTokenInput : tInput {
		public tTokenInput(IPreprocessorContext pp, TokenStream t) : base(pp)
		{
			tokens = t;
		}

		public override int scan(TPpToken ppToken)
		{
			return pp.ReadToken(tokens, ppToken);
		}

		public override int getch() 
		{
			Debug.Assert(0);
			throw new NotImplementedException ();
			return endOfInput; 
		}

		public override void ungetch()
		{
			Debug.Assert(0); 
			throw new NotImplementedException ();
		}

		protected TokenStream tokens;
	}
}

