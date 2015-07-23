using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IPreprocessorContext
	{
		void SetInput(TInputScanner input, bool versionWillBeError);
	}
}

