using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class tZeroInput : tInput
	{
		public tZeroInput(PreprocessorContext pp) :base(pp)
		{

		}

		// return a zero, for scanning a macro that was never defined
		public override int scan(ref TPpToken ppToken)
		{
			if (done)
				return tInput.END_OF_INPUT;

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

