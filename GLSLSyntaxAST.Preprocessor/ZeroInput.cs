using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class ZeroInput : BasePreprocessorInput
	{
		internal ZeroInput(PreprocessorContext pp) :base(pp)
		{

		}

		// return a zero, for scanning a macro that was never defined
		internal override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;

			ppToken.name = "0";
			ppToken.ival = 0;
			ppToken.space = false;
			done = true;

			return (int)CppEnums.INTCONSTANT;
		}

		internal override int getch()
		{ 
			throw new NotSupportedException ();
		}

		internal override void ungetch() {
			throw new NotSupportedException (); 
		}
	};
}

