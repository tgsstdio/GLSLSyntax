using System;

namespace GLSLSyntaxAST.CodeDom
{
	public abstract class tInput : IScannableType
	{
		protected tInput(PreprocessorContext p) 
		{
			done = false;
			pp = p;
		}

		public const int EOF = -12345;
		public const int END_OF_INPUT = -2;

		protected bool done;
		protected PreprocessorContext pp;

		#region IScannableType implementation

		public abstract int scan (TPpToken p);

		public abstract int getch ();

		public abstract void ungetch ();

		#endregion
	};
}

