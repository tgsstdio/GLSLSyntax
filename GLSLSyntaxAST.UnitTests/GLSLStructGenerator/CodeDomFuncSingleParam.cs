using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFuncSingleParam
	{
		const string FUNC_SINGLE_PARAM = "void main() { float value = sin(in_position); }";

		[Test ()]
		public void ExtractFuncSingleParam ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FUNC_SINGLE_PARAM);
			Assert.AreEqual (0, test.Blocks.Count);
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);
		}

		[Test ()]
		public void ExpressFuncSingleParam ()
		{
			const string expected = "translation_unit\n" 
				+ " external_declaration\n"
				+ "  function_definition\n" 
				+ "   function_prototype\n" 
				+ "    function_declarator\n" 
				+ "     function_header\n"
				+ "      fully_specified_type\n"
				+ "       VOID\n" 
				+ "      IDENTIFIER\n"
				+ "   compound_statement_no_new_scope\n"
				+ "    statement_list\n"
				+ "     statement\n"
				+ "      simple_statement\n"
				+ "       declaration_statement\n"
				+ "        declaration\n" 
				+ "         single_declaration\n"
				+ "          fully_specified_type\n"
				+ "           FLOAT\n"
				+ "          IDENTIFIER\n"
				+ "          EQUAL\n" 
				+ "          initializer\n"
				+ "           assignment_expression\n"
				+ "            function_call\n"
				+ "             function_call_header_with_parameters\n"
				+ "              function_call_parameter\n"
				+ "               IDENTIFIER\n";

			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FUNC_SINGLE_PARAM);
			Assert.AreEqual (expected, actual);
		}
	}
}
