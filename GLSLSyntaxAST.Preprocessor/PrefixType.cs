using System;

namespace GLSLSyntaxAST.Preprocessor
{
	[Flags]
	public enum PrefixType : int
	{
		None = 0,
		Warning,
		Error,
		InternalError,
		Unimplemented,
		Note
	};
}

