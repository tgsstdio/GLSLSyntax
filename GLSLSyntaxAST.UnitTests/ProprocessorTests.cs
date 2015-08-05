using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using System.Text;
using System.Collections.Generic;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]	
	public class ProprocessorTests
	{
		[Test ()]
		public void TestCase ()
		{
			var preprocessor = new Standalone ();
			string result = null;
			Assert.IsTrue(preprocessor.Run("Sample.vert", out result));
			Assert.IsNotNull (result);
		}
	}
}

