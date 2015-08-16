using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFunc2Parameters
	{
		const string FUNC_2_PARAMS = "void main() { float value = sin(in_position, 1.0); }";

		[Test ()]
		public void ExtractFunc2Parameters ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FUNC_2_PARAMS);
			Assert.AreEqual (0, actual);
			Assert.AreEqual (0, test.Blocks.Count);
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);
		}

		[Test ()]
		public void ExpressFunc2Parameters ()
		{ const string expected = "translation_unit\n"
				+ " external_declaration\n"
				+ "  function_definition\n"
				+ "   function_prototype\n"
				+ "    function_declarator\n"
				+ "     function_header\n"
				+ "      fully_specified_type\n"
				+ "       void (Keyword)\n"
				+ "      main (IDENTIFIER)\n"
				+ "   compound_statement_no_new_scope\n"
				+ "    statement_list\n"
				+ "     statement\n"
				+ "      simple_statement\n"
				+ "       declaration_statement\n"
				+ "        declaration\n"
				+ "         single_declaration\n"
				+ "          fully_specified_type\n"
				+ "           float (Keyword)\n"
				+ "          value (IDENTIFIER)\n"
				+ "          = (Key symbol)\n"
				+ "          initializer\n"
				+ "           assignment_expression\n"
				+ "            function_call\n"
				+ "             function_call_header_with_parameters\n"
				+ "              assignment_expression\n"
				+ "               variable_identifier\n"
				+ "                in_position (IDENTIFIER)\n"
				+ "              assignment_expression\n"
				+ "               floating_number_value\n"
				+ "                1 (INTCONSTANT)\n"
				+ "                . (Key symbol)\n"
				+ "                0 (REMAINDER)\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FUNC_2_PARAMS);
			Assert.AreEqual (expected, actual);
		}
	}
}
