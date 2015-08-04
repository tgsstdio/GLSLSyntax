using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IScannableType
	{
		int scan(ref TPpToken p);
		int getch();
		void ungetch();
	}
}

