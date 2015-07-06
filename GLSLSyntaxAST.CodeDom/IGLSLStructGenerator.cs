using System;
using System.IO;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLStructGenerator
	{
		int NoOfBlocks { get; }
		void Initialize();
		int Extract(string code);
		int Extract(Stream stream);
		void SaveAsAssembly(string path);
	}

}

