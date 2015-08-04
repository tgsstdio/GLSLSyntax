using System;

namespace GLSLSyntaxAST.CodeDom
{
	/// <summary>
	/// Optimization level for the compiler.
	/// </summary>
	public enum EShOptimizationLevel 
	{
		EShOptNoGeneration,
		EShOptNone,
		/// <summary>
		/// Optimizations that can be done quickly
		/// </summary>
		EShOptSimple,       
		/// <summary>
		/// Optimizations that will take more time
		/// </summary>
		EShOptFull,
	}
}

