using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class tMarkerInput : tInput
	{
		public tMarkerInput(PreprocessorContext pp) :base(pp)
		{
				
		}

		public override int scan(ref TPpToken ppToken)
		{
			if (done)
				return tInput.END_OF_INPUT;
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

