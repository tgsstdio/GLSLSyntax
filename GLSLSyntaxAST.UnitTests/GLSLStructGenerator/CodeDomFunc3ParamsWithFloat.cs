using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFunc3ParamsWithFloat
	{
		const string FUNC_3_PARAMS_FLOAT = "void main() { float value = sin(in_position, 1.0, 1.0); }";

		[Test ()]
		public void ExtractFunc3ParamsWithFloat ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FUNC_3_PARAMS_FLOAT);
		}

		[Test ()]
		public void ExpressFunc3ParamsWithFloat ()
		{
			const string expected 	= "translation_unit\n"
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
									+ "                0 (REMAINDER)\n"
									+ "              assignment_expression\n"
									+ "               floating_number_value\n"
									+ "                1 (INTCONSTANT)\n"
									+ "                . (Key symbol)\n"
									+ "                0 (REMAINDER)\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FUNC_3_PARAMS_FLOAT);
			Assert.AreEqual (expected, actual);
		}
	}
}
