using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using System.Collections.Generic;

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
			Assert.AreEqual (0, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);
			var blocks = new List<StructInfo> ();
			foreach (var block in test.Blocks)
			{
				blocks.Add (block);
			}
			var first = blocks [0];
			Assert.AreEqual (GLSLStructType.Struct, first.StructType);
			Assert.IsNotNull (first.Layout);
			Assert.IsTrue (first.Layout.Location.HasValue);
			Assert.AreEqual (1, first.Layout.Location.Value);
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
									+ "       LAYOUT\n"
									+ "       layout_qualifier_id_list\n"
									+ "        layout_qualifier_id\n"
									+ "         IDENTIFIER\n"
									+ "        layout_qualifier_id\n"
									+ "         IDENTIFIER\n"
									+ "         EQUAL\n"
									+ "         constant_expression\n"
									+ "          INTCONSTANT\n"
									+ "     struct_specifier\n"
									+ "      STRUCT\n"
									+ "      IDENTIFIER\n"
									+ "      struct_declaration_list\n"
									+ "       struct_declaration\n"
									+ "        VEC3\n"
									+ "        struct_declarator\n"
									+ "         IDENTIFIER\n"
									+ "       struct_declaration\n"
									+ "        VEC4\n"
									+ "        struct_declarator\n"
									+ "         IDENTIFIER\n"
									+ "       struct_declaration\n"
									+ "        VEC4\n"
									+ "        struct_declarator\n"
									+ "         IDENTIFIER\n"
									+ "       struct_declaration\n"
									+ "        VEC4\n"
									+ "        struct_declarator\n"
									+ "         IDENTIFIER\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (STRUCT_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}
	}
}
