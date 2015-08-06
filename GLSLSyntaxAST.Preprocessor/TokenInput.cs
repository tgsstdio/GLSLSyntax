using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class TokenInput : BasePreprocessorInput {
		internal TokenInput(PreprocessorContext pp, TokenStream t) : base(pp)
		{
			tokens = t;
		}

		internal override int scan(ref PreprocessorToken ppToken)
		{
			return pp.ReadToken(tokens, ppToken);
		}

		internal override int getch() 
		{
			throw new NotSupportedException ();
		}

		internal override void ungetch()
		{
			throw new NotSupportedException ();
		}

		protected TokenStream tokens;
	}
}

