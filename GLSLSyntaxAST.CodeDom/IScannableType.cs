using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IScannableType
	{
		int scan(TPpToken p);
		int getch();
		void ungetch();
	}
}

