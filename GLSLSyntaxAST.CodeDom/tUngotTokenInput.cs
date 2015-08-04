using System;

namespace GLSLSyntaxAST.CodeDom
{

	public class tUngotTokenInput : tInput {
		public tUngotTokenInput(PreprocessorContext pp, int t, TPpToken p) 
			: base(pp)
		{
			token = t;
			lval = p;
		}

		public override int scan(ref TPpToken ppToken)
		{
			if (done)
				return tInput.END_OF_INPUT;

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
		protected TPpToken lval;
	};
}

