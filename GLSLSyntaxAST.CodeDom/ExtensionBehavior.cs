using System;

namespace GLSLSyntaxAST.CodeDom
{
	//
	// The behaviors from the GLSL "#extension extension_name : behavior"
	//
	[Flags]
	public enum ExtensionBehavior : int
	{
		// use as initial state of an extension that is only partially implemented
		Missing = 0,
		Require,
		Enable,
		Warn,
		Disable,
		DisablePartial   		
	}
}
