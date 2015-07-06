using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomTest
	{
		[Test ()]
		public void TestCase ()
		{
			int expected = 1;
			IGLSLStructGenerator test = new GLSLStructBuilder ();
			test.Initialize ();
			int actual = test.Extract ("layout(std140) uniform UBOData {\n\tvec3 firstValue;\n\tfloat thirdValue;\n\tvec4 secondValue;\n};");
			Assert.AreEqual (expected, actual);
			Assert.AreEqual (
		}

		[Test ()]
		public void Initialize01 ()
		{
			IGLSLStructGenerator test = new GLSLStructBuilder ();
			test.Initialize ();
		}
	}
}

