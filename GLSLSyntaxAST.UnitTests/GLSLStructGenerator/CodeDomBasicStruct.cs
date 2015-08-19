using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using System.Collections.Generic;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomBasicStruct
	{
		const string STATEMENT = @"struct BindlessTextureHandle
				{
					texture2D TextureId;
				};";

		[TestCase]
		public void ExtractBasicStruct ()
		{
			const int expected = 1;
			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (STATEMENT);
			Assert.AreEqual (expected, actual);

			Assert.AreEqual (expected, test.Blocks.Count);
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
			Assert.IsFalse (first.Layout.Binding.HasValue);
			Assert.IsNull (first.Layout.Format);			
		}

		[TestCase]
		public void ExpressBasicStruct ()
		{
			const string expected = "translation_unit\n" 
				+ " external_declaration\n"
				+ "  declaration\n"
				+ "   single_declaration\n"
				+ "    fully_specified_type\n" 
				+ "     struct_specifier\n"
				+ "      STRUCT\n"
				+ "      IDENTIFIER\n"
				+ "      struct_declaration_list\n"
				+ "       struct_declaration\n"
				+ "        IDENTIFIER\n"
				+ "        struct_declarator\n"
				+ "         IDENTIFIER\n";

			var lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			var test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (STATEMENT);
			Assert.AreEqual (expected, actual);			
		}
	}
}

