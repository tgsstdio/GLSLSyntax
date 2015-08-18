using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomFuncNoParams
	{
		const string FUNC_NO_PARAM = "void main() { float value = sin(); }";

		[TestCase]
		public void ExtractFuncNoParams ()
		{
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FUNC_NO_PARAM);			
		}

		[TestCase]
		public void  ExpressFuncNoParams ()
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
				+ "             function_call_header_with_parameters\n";

			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FUNC_NO_PARAM);
			Assert.AreEqual (expected, actual);			
		}
	}
}

