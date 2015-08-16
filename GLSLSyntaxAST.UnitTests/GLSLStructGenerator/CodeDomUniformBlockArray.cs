using System;
using GLSLSyntaxAST.CodeDom;
using NUnit.Framework;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomUniformBlockArray
	{
		const string UNIFORMS_TEST_CASE = 
				@"
				layout(std140) uniform CameraBlock
				{
					mat4 projectionMatrix;
					mat4 inverseProjectionMatrix;
					mat4 viewMatrix;
					mat4 inverseViewMatrix;
					float x;
					float y;
					float z;
					float w;
					vec3 eye;
					float fieldOfView;
				} in_cameras[1];";

		[Test ()]
		public void ExtractUniformBlockArray ()
		{
			const int expected = 2;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Blocks.Count);
			Assert.AreEqual (1, test.Uniforms.Count);
			Assert.AreEqual (0, test.Attributes.Count);
		}

		[Test ()]
		public void ExpressUniformBlockArray ()
		{
			const string expected	= "translation_unit\n"
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
									+ "    CameraBlock (IDENTIFIER)\n"
									+ "    struct_declaration_list\n"
									+ "     struct_declaration\n"
									+ "      mat4 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       projectionMatrix (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      mat4 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       inverseProjectionMatrix (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      mat4 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       viewMatrix (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      mat4 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       inverseViewMatrix (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       x (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       y (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       z (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       w (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      vec3 (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       eye (IDENTIFIER)\n"
									+ "     struct_declaration\n"
									+ "      float (Keyword)\n"
									+ "      struct_declarator\n"
									+ "       fieldOfView (IDENTIFIER)\n"
									+ "   in_cameras (IDENTIFIER)\n"
									+ "   array_specifier\n"
									+ "    constant_expression\n"
									+ "     1 (INTCONSTANT)\n";
			
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}		

	}
}

