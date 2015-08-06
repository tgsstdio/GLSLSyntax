using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IPreprocessorContext
	{
		void SetInput(InputScanner input, bool versionWillBeError);
	}
}

