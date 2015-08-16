using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomStruct
	{
		const string STRUCT_TEST_CASE = "layout (std140, location = 1) struct LightProperties\n{\n\tvec3 direction;\n\tvec4 ambientColor;\n\tvec4 diffuseColor;\n\tvec4 specularColor;\n};";

		[Test ()]
		public void ExtractStruct ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (STRUCT_TEST_CASE);
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Blocks.Count);
		}

		[Test ()]
		public void ExpressStruct ()
		{
			const string expected 	= "translation_unit\n"
									+ " external_declaration\n"
									+ "  declaration\n"
									+ "   single_declaration\n"
									+ "    fully_specified_type\n"
									+ "     type_qualifier\n"
									+ "      layout_qualifier\n"
									+ "       layout (Keyword)\n"
									+ "       layout_qualifier_id_list\n"
									+ "        layout_qualifier_id\n"
									+ "         std140 (IDENTIFIER)\n"
									+ "        layout_qualifier_id\n"
									+ "         location (IDENTIFIER)\n"
									+ "         = (Key symbol)\n"
									+ "         constant_expression\n"
									+ "          1 (INTCONSTANT)\n"
									+ "     struct_specifier\n"
									+ "      struct (Keyword)\n"
									+ "      LightProperties (IDENTIFIER)\n"
									+ "      struct_declaration_list\n"
									+ "       struct_declaration\n"
									+ "        vec3 (Keyword)\n"
									+ "        struct_declarator\n"
									+ "         direction (IDENTIFIER)\n"
									+ "       struct_declaration\n"
									+ "        vec4 (Keyword)\n"
									+ "        struct_declarator\n"
									+ "         ambientColor (IDENTIFIER)\n"
									+ "       struct_declaration\n"
									+ "        vec4 (Keyword)\n"
									+ "        struct_declarator\n"
									+ "         diffuseColor (IDENTIFIER)\n"
									+ "       struct_declaration\n"
									+ "        vec4 (Keyword)\n"
									+ "        struct_declarator\n"
									+ "         specularColor (IDENTIFIER)\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (STRUCT_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}
	}
}
