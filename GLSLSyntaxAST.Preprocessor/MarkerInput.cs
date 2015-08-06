using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class MarkerInput : BasePreprocessorInput
	{
		internal MarkerInput(PreprocessorContext pp) :base(pp)
		{
				
		}

		internal override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;
			done = true;

			return marker;
		}

		internal override int getch()
		{
			throw new NotSupportedException ();
		}

		internal override void ungetch()
		{
			throw new NotSupportedException ();
		}

		internal static int marker = -3;
	}
}

