using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class GLSLAssembly
	{
		public string Path { get; set; }
		public string OutputAssembly { get; set; }
		public string Version { get; set; }
		public string Namespace { get; set; }
		public bool InMemory {get;set;}
		public string[] ReferencedAssemblies { get; set; }
	}
}

