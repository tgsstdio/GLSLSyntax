using System;
using System.IO;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLStructGenerator
	{
		List<StructMember> Uniforms { get; }
		List<StructInfo> Blocks {get;}
		void Initialize();
		int Extract(string code);
		int Extract(Stream stream);
		void SaveAsAssembly(GLSLAssembly assembly);
		string SaveAsText();
	}

}

