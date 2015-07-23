using System;

namespace GLSLSyntaxAST.CodeDom
{
	[Flags]
	public enum Profile : int
	{
		BadProfile           = 0,
		NoProfile            = (1 << 0), // only for desktop, before profiles showed up
		CoreProfile          = (1 << 1),
		CompatibilityProfile = (1 << 2),
		EsProfile            = (1 << 3)
	}
}

