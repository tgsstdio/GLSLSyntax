using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class CodeDomVersionString
	{
		const string VERSION_EXAMPLE = "#version 150";
		[Test ()]
		public void ExtractVersionString ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			int actual = test.Extract (VERSION_EXAMPLE);
		}

		[Test ()]
		public void ExpressVersionString ()
		{
			const string expected = "translation_unit\n";
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();
			lookup.Initialize ();
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();
			var actual = test.ExpressTree (VERSION_EXAMPLE);
			Assert.AreEqual (expected, actual);
		}
	}
}

