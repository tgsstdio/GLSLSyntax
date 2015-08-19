using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomSimpleVariableAssignment
	{
		public string SIMPLE_VAR_ASSIGN = @"void main(void)
				{
					uv = vs;
				}";

		[TestCase]
		public void ExtractSimpleVariableAssign ()
		{
			int expected = 0;
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (SIMPLE_VAR_ASSIGN);
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (0, test.Blocks.Count);
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);			
		}

		[TestCase]
		public void ExpressSimpleVariableAssign ()
		{
			const string expected = "translation_unit\n"
			                        + " external_declaration\n"
			                        + "  function_definition\n"
			                        + "   function_prototype\n"
			                        + "    function_declarator\n"
			                        + "     function_header_with_parameters\n"
			                        + "      function_header\n"
			                        + "       fully_specified_type\n"
			                        + "        VOID\n"
			                        + "       IDENTIFIER\n"
			                        + "      parameter_declaration\n"
			                        + "       parameter_type_specifier\n"
			                        + "        VOID\n"
			                        + "   compound_statement_no_new_scope\n"
			                        + "    statement_list\n"
			                        + "     statement\n"
			                        + "      simple_statement\n"
			                        + "       expression_statement\n"
			                        + "        expression\n"
			                        + "         assignment_expression\n"
			                        + "          variable_identifier\n"
			                        + "           IDENTIFIER\n"
			                        + "          assignment_operator\n"
			                        + "           EQUAL\n"
			                        + "          assignment_expression\n"
			                        + "           variable_identifier\n"
			                        + "            IDENTIFIER\n";
			
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (SIMPLE_VAR_ASSIGN);
			Assert.AreEqual (expected, actual);

		}
	}
}

