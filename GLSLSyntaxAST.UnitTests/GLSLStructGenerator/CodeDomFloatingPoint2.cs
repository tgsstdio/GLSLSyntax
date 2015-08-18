﻿using System;
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
			Assert.AreEqual (0, actual);
			Assert.AreEqual (0, test.Blocks.Count);
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);
		}

		[Test ()]
		public void ExpressFloatingPoint2 ()
		{
			const string expected	 = "translation_unit\n"
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
			                         + "       expression_statement\n"
			                         + "        expression\n"
			                         + "         assignment_expression\n"
			                         + "          variable_identifier\n"
			                         + "           IDENTIFIER\n"
			                         + "          assignment_operator\n"
			                         + "           EQUAL\n"
			                         + "          assignment_expression\n"
			                         + "           function_call\n"
			                         + "            function_call_header_with_parameters\n"
			                         + "             function_call_parameter\n"
			                         + "              IDENTIFIER\n"
			                         + "             assignment_expression\n"
			                         + "              floating_number_value\n"
			                         + "               INTCONSTANT\n"
			                         + "               .\n"
			                         + "               REMAINDER\n";
						
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FLOAT_POINT_2_PARAMS);
			Assert.AreEqual (expected, actual);
		}
	}
}
