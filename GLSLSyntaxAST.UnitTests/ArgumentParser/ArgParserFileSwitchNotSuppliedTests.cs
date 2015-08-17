using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class ArgParserFileSwitchNotSuppliedTests
	{
		[TestCase]
		[ExpectedException(typeof(ArgumentParser.FileSwitchNotSuppliedException))]
		public void NoArguments()
		{
			var parser = new ArgumentParser ();
			parser.Parse (new string[]{ });
		}

		[TestCase]
		[ExpectedException(typeof(ArgumentParser.FileSwitchNotSuppliedException))]
		public void AssemblySwitchOnly()
		{
			var parser = new ArgumentParser ();
			parser.Parse (new string[]{ "-a", "Sample.dll"});
		}

		[TestCase]
		[ExpectedException(typeof(ArgumentParser.FileSwitchNotSuppliedException))]
		public void CodeSwitchOnly()
		{
			var parser = new ArgumentParser ();
			parser.Parse (new string[]{ "-c", "Sample.dll"});
		}
	}
}

