﻿using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFloatingPoint1
	{
		const string FLOAT_POINT_1 = "void main() { float in_position = 1.0; }";

		[Test ()]
		public void ExtractFloatingPoint1 ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FLOAT_POINT_1);
		}

		[Test ()]
		public void ExpressFloatingPoint1 ()
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
									+ "          in_position (IDENTIFIER)\n"
									+ "          = (Key symbol)\n"
									+ "          initializer\n"
									+ "           assignment_expression\n"
									+ "            floating_number_value\n"
									+ "             1 (INTCONSTANT)\n"
									+ "             . (Key symbol)\n"
									+ "             0 (REMAINDER)\n";
			
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FLOAT_POINT_1);
			Assert.AreEqual (expected, actual);
		}
	}
}
