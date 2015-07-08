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
		public void Initialize01 ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();			
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();		
		}
	}
}

