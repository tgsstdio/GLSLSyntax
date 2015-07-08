using System;

namespace GLSLSyntaxAST.CodeDom
{
	public interface IGLSLTypeLookup
	{
		void Initialize();
		Type FindClosestType (string typeName);
	}
}

