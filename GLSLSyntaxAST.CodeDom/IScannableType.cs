using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IScannableType
	{
		int scan(ref PreprocessorToken p);
		int getch();
		void ungetch();
	}
}

