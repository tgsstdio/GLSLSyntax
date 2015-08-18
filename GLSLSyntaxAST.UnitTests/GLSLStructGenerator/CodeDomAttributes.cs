using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomAttributes
	{
		const string ATTRIBUTES_TEST_CASE = "layout (location = 1) in vec3 v_normal;";

		[Test ()]
		public void ExtractAttributes ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (ATTRIBUTES_TEST_CASE);
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (0, test.Blocks.Count);
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (1, test.Attributes.Count);
		}

		[Test ()]
		public void ExpressAttributes ()
		{
			const string expected 	= "translation_unit\n"
				+ " external_declaration\n"
				+ "  declaration\n"
				+ "   single_declaration\n"
				+ "    fully_specified_type\n"
				+ "     type_qualifier\n"
				+ "      layout_qualifier\n"
				+ "       LAYOUT\n"
				+ "       layout_qualifier_id_list\n"
				+ "        layout_qualifier_id\n"
				+ "         IDENTIFIER\n"
				+ "         EQUAL\n"
				+ "         constant_expression\n"
				+ "          INTCONSTANT\n"
				+ "      storage_qualifier\n"
				+ "       IN\n"
				+ "     VEC3\n"
				+ "    IDENTIFIER\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (ATTRIBUTES_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}
	}
}

