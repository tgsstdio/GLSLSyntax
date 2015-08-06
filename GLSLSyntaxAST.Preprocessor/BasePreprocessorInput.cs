namespace GLSLSyntaxAST.Preprocessor
{
	internal abstract class BasePreprocessorInput
	{
		protected BasePreprocessorInput(PreprocessorContext p) 
		{
			done = false;
			pp = p;
		}

		internal const int EOF = -12345;
		internal const int END_OF_INPUT = -1;

		protected bool done;
		protected PreprocessorContext pp;

		#region IScannableType implementation

		internal abstract int scan (ref PreprocessorToken ppToken);

		internal abstract int getch ();

		internal abstract void ungetch ();

		#endregion
	};
}

