using System;
using System.Collections.Generic;
using System.IO;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLUniformExtractor
	{
		ICollection<StructMember> Uniforms { get; }
		ICollection<StructInfo> Blocks {get;}
		void Initialize();
		int Extract(string code);
		int Extract(Stream stream);		
	}

}

