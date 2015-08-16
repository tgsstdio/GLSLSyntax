using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomFuncVoidParam
	{
		const string FUNC_VOID_PARAM = "void main() { float value = sin(void); }";

		[Test ()]
		public void ExtractFuncVoidParam ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (FUNC_VOID_PARAM);
		}

		[Test ()]
		public void ExpressFuncVoidParam ()
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
				+ "             void (Keyword)\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (FUNC_VOID_PARAM);
			Assert.AreEqual (expected, actual);
		}
	}
}
