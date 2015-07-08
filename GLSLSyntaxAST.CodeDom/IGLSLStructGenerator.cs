using System;
using System.IO;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLStructGenerator
	{
		void Initialize();
		void SaveAsAssembly(GLSLAssembly assembly);
		string SaveAsText();
	}

}

