using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class UngotTokenInput : BasePreprocessorInput {
		internal UngotTokenInput(PreprocessorContext pp, int t, PreprocessorToken p) 
			: base(pp)
		{
			token = t;
			lval = p;
		}

		internal override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;

			int ret = token;
			ppToken = lval;
			done = true;

			return ret;
		}


		internal override int getch() 
		{
			throw new NotSupportedException ();
		}

		internal override void ungetch() 
		{ 
			throw new NotSupportedException ();
		}

		protected int token;
		protected PreprocessorToken lval;
	};
}

