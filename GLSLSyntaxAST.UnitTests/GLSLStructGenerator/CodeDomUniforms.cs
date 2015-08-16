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
									+ "      layout (Keyword)\n"
									+ "      layout_qualifier_id_list\n"
									+ "       layout_qualifier_id\n"
									+ "        std140 (IDENTIFIER)\n"
									+ "     storage_qualifier\n"
									+ "      uniform (Keyword)\n"
									+ "    UBOData (IDENTIFIER)\n"
									+ "    struct_declaration_list\n"
									+ "     struct_declaration\n"
									+ "      vec3 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       firstValue (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       thirdValue (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      vec4 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       secondValue (IDENTIFIER)\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}
	}
}
