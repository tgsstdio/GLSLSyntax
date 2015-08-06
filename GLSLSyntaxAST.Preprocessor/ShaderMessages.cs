using System;

namespace GLSLSyntaxAST.CodeDom
{
	/// <summary>
	/// Message choices for what errors and warnings are given.
	/// </summary>
	[Flags]
	public enum ShaderMessages : int
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
		SuppressWarnings = (1 << 1),
		/// <summary>
		/// print the AST intermediate representation
		/// </summary>
		AST              = (1 << 2),
		/// <summary>
		/// issue messages for SPIR-V generation
		/// </summary>
		SPIRVRules         = (1 << 3), 
		/// <summary>
		/// issue messages for Vulkan-requirements of GLSL for SPIR-V
		/// </summary>
		VulkanRules      = (1 << 4),
		/// <summary>
		/// only print out errors produced by the preprocessor
		/// </summary>
		OnlyPreprocessor = (1 << 5), 
	};
}

