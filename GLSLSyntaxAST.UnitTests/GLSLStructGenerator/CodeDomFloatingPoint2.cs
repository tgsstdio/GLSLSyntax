using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFloatingPoint2
	{
		const string FLOAT_POINT_2_PARAMS = "void main() { corners = vec4(in_position, 1.0); }";

		[Test ()]
		public void ExtractFloatingPoint2 ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FLOAT_POINT_2_PARAMS);
		}

		[Test ()]
		public void ExpressFloatingPoint2 ()
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
									+ "       expression_statement\n"
									+ "        expression\n"
									+ "         assignment_expression\n"
									+ "          variable_identifier\n"
									+ "           corners (IDENTIFIER)\n"
									+ "          assignment_operator\n"
									+ "           = (Key symbol)\n"
									+ "          assignment_expression\n"
									+ "           function_call\n"
									+ "            function_call_header_with_parameters\n"
									+ "             assignment_expression\n"
									+ "              variable_identifier\n"
									+ "               in_position (IDENTIFIER)\n"
									+ "             assignment_expression\n"
									+ "              floating_number_value\n"
									+ "               1 (INTCONSTANT)\n"
									+ "               . (Key symbol)\n"
									+ "               0 (REMAINDER)\n";
				
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FLOAT_POINT_2_PARAMS);
			Assert.AreEqual (expected, actual);
		}
	}
}
