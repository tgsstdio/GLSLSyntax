using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class MarkerInput : BasePreprocessorInput
	{
		public MarkerInput(PreprocessorContext pp) :base(pp)
		{
				
		}

		public override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;
			done = true;

			return marker;
		}

		public override int getch()
		{
			throw new NotSupportedException ();
		}

		public override void ungetch()
		{
			throw new NotSupportedException ();
		}

		public static int marker = -3;
	}
}

