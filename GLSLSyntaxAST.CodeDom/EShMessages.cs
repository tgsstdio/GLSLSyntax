using System;

namespace GLSLSyntaxAST.CodeDom
{
	/// <summary>
	/// Message choices for what errors and warnings are given.
	/// </summary>
	[Flags]
	public enum EShMessages : int
	{
		/// <summary>
		/// default is to give all required errors and extra warnings
		/// </summary>
		Default    = 0,         
		/// <summary>
		/// be liberal in accepting input
		/// </summary>
		RelaxedErrors    = (1 << 0), 
		/// <summary>
		/// suppress all warnings, except those required by the specification
		/// </summary>
		EShMsgSuppressWarnings = (1 << 1),
		/// <summary>
		/// print the AST intermediate representation
		/// </summary>
		EShMsgAST              = (1 << 2),
		/// <summary>
		/// issue messages for SPIR-V generation
		/// </summary>
		EShMsgSpvRules         = (1 << 3), 
		/// <summary>
		/// issue messages for Vulkan-requirements of GLSL for SPIR-V
		/// </summary>
		EShMsgVulkanRules      = (1 << 4),
		/// <summary>
		/// only print out errors produced by the preprocessor
		/// </summary>
		EShMsgOnlyPreprocessor = (1 << 5), 
	};
}

