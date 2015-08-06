using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class TokenInput : BasePreprocessorInput {
		public TokenInput(PreprocessorContext pp, TokenStream t) : base(pp)
		{
			tokens = t;
		}

		public override int scan(ref PreprocessorToken ppToken)
		{
			return pp.ReadToken(tokens, ppToken);
		}

		public override int getch() 
		{
			throw new NotSupportedException ();
		}

		public override void ungetch()
		{
			throw new NotSupportedException ();
		}

		protected TokenStream tokens;
	}
}

