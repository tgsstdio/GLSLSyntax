using System;

namespace GLSLSyntaxAST.Preprocessor
{
	[Flags]
	public enum SinkType : int
	{
		Null = 0,
		Debugger = 0x01,
		StdOut = 0x02,
		String = 0x04,
	};
}

