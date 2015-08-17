using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class ArgParserDefaultValueTests
	{
		[TestCase]
		public void DefaultValues()
		{
			var parser = new ArgumentParser ();
			Assert.IsFalse (parser.GenerateAssembly);
			Assert.IsFalse (parser.GenerateCode);
			Assert.AreEqual (string.Empty, parser.AssemblyFileName);
			Assert.AreEqual (string.Empty, parser.SourceFileName);
			Assert.AreEqual (string.Empty, parser.Namespace);
		}
	}
}

