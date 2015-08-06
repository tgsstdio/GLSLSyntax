using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class ZeroInput : BasePreprocessorInput
	{
		public ZeroInput(PreprocessorContext pp) :base(pp)
		{

		}

		// return a zero, for scanning a macro that was never defined
		public override int scan(ref PreprocessorToken ppToken)
		{
			if (done)
				return BasePreprocessorInput.END_OF_INPUT;

			ppToken.name = "0";
			ppToken.ival = 0;
			ppToken.space = false;
			done = true;

			return (int)CppEnums.INTCONSTANT;
		}

		public override int getch()
		{ 
			throw new NotSupportedException ();
		}

		public override void ungetch() {
			throw new NotSupportedException (); 
		}
	};
}

