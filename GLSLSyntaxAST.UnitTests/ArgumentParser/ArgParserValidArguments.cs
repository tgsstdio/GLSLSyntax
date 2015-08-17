using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class ArgParserValidArguments
	{
		[TestCase]
		public void TestNamespace()
		{
			var parser = new ArgumentParser ();
			Assert.AreEqual (string.Empty, parser.Namespace);
			const string expected = "BirdNest.Shaders";
			parser.Parse(new string[]{"-n", expected,"-a", "Sample.dll", "-F", "Sample.frag"});
			Assert.AreEqual(expected, parser.Namespace);
		}

		[TestCase]
		public void TestAssemblyOption()
		{
			var parser = new ArgumentParser ();
			Assert.AreEqual (string.Empty, parser.AssemblyFileName);
			Assert.IsFalse (parser.GenerateAssembly);
			const string expected = "Sample.dll";
			parser.Parse(new string[]{"-n", "BirdNest.Shaders","-a", expected, "-F", "Sample.frag"});
			Assert.AreEqual(expected, parser.AssemblyFileName);
			Assert.IsTrue (parser.GenerateAssembly);
		}

		[TestCase]
		public void TestCodeOption()
		{
			var parser = new ArgumentParser ();
			Assert.AreEqual (string.Empty, parser.SourceFileName);
			Assert.IsFalse (parser.GenerateCode);
			const string expected = "Sample.cs";
			parser.Parse(new string[]{"-n", "BirdNest.Shaders","-c", expected, "-F", "Sample.frag"});
			Assert.AreEqual(expected, parser.SourceFileName);
			Assert.IsTrue (parser.GenerateCode);
		}

		[TestCase]
		public void TestBothCodeAndAssemblyOption()
		{
			var parser = new ArgumentParser ();
			Assert.AreEqual (string.Empty, parser.SourceFileName);
			Assert.IsFalse (parser.GenerateCode);
			Assert.AreEqual (string.Empty, parser.AssemblyFileName);
			Assert.IsFalse (parser.GenerateAssembly);
			const string expectedSourceFile = "Sample.cs";
			const string expectedAssembly = "Sample.dll";
			parser.Parse(new string[]{"-a", expectedAssembly,"-c", expectedSourceFile, "-F", "Sample.frag"});
			Assert.AreEqual(expectedSourceFile, parser.SourceFileName);
			Assert.IsTrue (parser.GenerateCode);
			Assert.AreEqual(expectedAssembly, parser.AssemblyFileName);
			Assert.IsTrue (parser.GenerateAssembly);
		}

		[TestCase]
		public void PassInSingleShader()
		{
			var parser = new ArgumentParser ();
			const string expected = "Sample.frag";
			var result = parser.Parse(new string[]{"-a", "Sample.dll", "-F", expected});
			Assert.AreEqual (1, result.Length);
			Assert.AreEqual (expected, result[0]);
		}

		[TestCase]
		public void PassInTwoShaders()
		{
			var parser = new ArgumentParser ();
			const string expected1 = "Sample.frag";
			const string expected2 = "Sample.vert";
			var result = parser.Parse(new string[]{"-a", "Sample.dll", "-F", expected1, expected2});
			Assert.AreEqual (2, result.Length);
			Assert.AreEqual (expected1, result[0]);
			Assert.AreEqual (expected2, result[1]);
		}
	}
}

