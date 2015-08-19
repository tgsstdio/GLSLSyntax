using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using System.Collections.Generic;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomBufferDeclaration
	{
		const string BUFFER_STRING = @"layout(binding = 4, std430) buffer LinkedList
{
	NodeType nodes[];
	// Padding[]
};";
		[Test ()]
		public void ExtractBufferDeclaration ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (BUFFER_STRING);
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
			Assert.AreEqual (GLSLStructType.Buffer, first.StructType);
			Assert.IsNotNull (first.Layout);
			Assert.IsTrue (first.Layout.Binding.HasValue);
			Assert.AreEqual (4, first.Layout.Binding.Value);
			Assert.AreEqual ("std430", first.Layout.Format);
		}


		[Test ()]
		public void ExpressBufferDeclaration ()
		{
			const string expected = "translation_unit\n"
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
				+ "        layout_qualifier_id\n"
				+ "         IDENTIFIER\n"
				+ "     struct_specifier\n"
				+ "      BUFFER\n"
				+ "      IDENTIFIER\n"
				+ "      struct_declaration_list\n"
				+ "       struct_declaration\n"
				+ "        IDENTIFIER\n"
				+ "        struct_declarator\n"
				+ "         IDENTIFIER\n"
				+ "         array_specifier\n"
				+ "          array_empty_bracket\n"
				+ "           []\n"; 

			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (BUFFER_STRING);
			Assert.AreEqual (expected, actual);
		}
	}
}

