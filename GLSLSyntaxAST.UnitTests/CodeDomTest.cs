using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using GLSLOutput;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomTest
	{
		[Test ()]
		public void TestCase ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract ("layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};");
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Blocks.Count);
		}

		[Test ()]
		public void TestStruct ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract ("layout (std140, location = 1) struct LightProperties\n{\n\tvec3 direction;\n\tvec4 ambientColor;\n\tvec4 diffuseColor;\n\tvec4 specularColor;\n};");
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Blocks.Count);
		}

		[Test ()]
		public void TestAttributes ()
		{
			int expected = 1;
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract ("layout (location = 1) in vec3 v_normal;");
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (1, test.Attributes.Count);
		}

		[Test ()]
		public void Initialize01 ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();			
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();		
		}
	}
}

