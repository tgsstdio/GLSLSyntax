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
									+ "      LAYOUT\n"
									+ "      layout_qualifier_id_list\n"
									+ "       layout_qualifier_id\n"
									+ "        IDENTIFIER\n"
									+ "     storage_qualifier\n"
									+ "      UNIFORM\n"
									+ "    IDENTIFIER\n"
									+ "    struct_declaration_list\n"
									+ "     struct_declaration\n"
									+ "      MAT4\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      MAT4\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      MAT4\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      MAT4\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      VEC3\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "     struct_declaration\n"
									+ "      FLOAT\n"
									+ "      struct_declarator\n"
									+ "       IDENTIFIER\n"
									+ "   IDENTIFIER\n"
									+ "   array_specifier\n"
									+ "    constant_expression\n"
									+ "     INTCONSTANT\n";
			
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (UNIFORMS_TEST_CASE);
			Assert.AreEqual (expected, actual);
		}		

	}
}

