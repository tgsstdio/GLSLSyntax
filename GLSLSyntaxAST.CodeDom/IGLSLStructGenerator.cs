using System.CodeDom.Compiler;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLStructGenerator
	{
		void Initialize();
		void SaveAsAssembly(CodeDomProvider provider, GLSLAssembly assembly);
		void SaveAsCode(CodeDomProvider provider, GLSLAssembly assembly, IGLSLUniformExtractor extractor, CodeGeneratorOptions options);
	}

}

