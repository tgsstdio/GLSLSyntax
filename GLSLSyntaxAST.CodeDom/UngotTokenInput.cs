using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class UngotTokenInput : BasePreprocessorInput {
		public UngotTokenInput(PreprocessorContext pp, int t, PreprocessorToken p) 
			: base(pp)
		{
			token = t;
			lval = p;
		}

		public override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;

			int ret = token;
			ppToken = lval;
			done = true;

			return ret;
		}


		public override int getch() 
		{
			throw new NotSupportedException ();
		}

		public override void ungetch() 
		{ 
			throw new NotSupportedException ();
		}

		protected int token;
		protected PreprocessorToken lval;
	};
}

