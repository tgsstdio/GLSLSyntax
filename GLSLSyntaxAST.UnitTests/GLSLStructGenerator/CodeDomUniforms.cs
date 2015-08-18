using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomUniforms
	{
		const string UNIFORMS_TEST_CASE = "layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};";

		[Test ()]
		public void ExtractUniforms ()
		{
			const int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Blocks.Count);
		}

		[Test ()]
		public void ExpressUniforms ()
		{
			const string expected 	= "translation_unit\n"
								  	+ " external_declaration\n"
									+ "  declaration\n"
									+ "   block_structure\n"
									+ "    type_qualifier\n"
									+ "     layout_qualifier\n"
									+ "      LAYOUT\n"
									+ "      layout_qualifier_id_list\n"
									+ "       layout_qualifier_id\n"
									+ "        IDENTIFIER\n"
									+ "     storage_qualifier\n"
									+ "      UNIFORM\n"
									+ "    IDENTIFIER\n"
									+ "    struct_declaration_list\n"
									+ "     struct_declaration\n"
									+ "      VEC3\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      VEC4\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}
	}
}
