using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture]
	public class ArgParserMissingValueTests
	{
		[TestCase]
		public void CodeValueMissing1()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-c' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-c", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void CodeValueMissing2()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-c' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-C", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void AssemblyValueMissing1()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-a' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-a", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void AssemblyValueMissing2()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-a' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-A", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void AssemblyValueMissing3()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-a' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-a", "-c", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void NamespaceValueMissing1()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-n' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-n", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}

		[TestCase]
		public void NamespaceValueMissing2()
		{
			bool isThrown = false;
			Type expectedType = typeof(ArgumentParser.ValueNotSuppliedException);
			Type actualType = null;
			const string expectedErrorMsg = "Value for '-n' switch not supplied";
			string actualErrorMsg = "";

			var parser = new ArgumentParser ();

			try
			{
				parser.Parse (new string[]{ "-N", "-F", "Sample.vert" });
			}
			catch(Exception ex)
			{
				actualType = ex.GetType ();
				isThrown = true;
				actualErrorMsg = ex.Message;
			}

			Assert.IsTrue (isThrown);
			Assert.AreEqual (expectedType, actualType);
			Assert.AreEqual (expectedErrorMsg, actualErrorMsg);
		}
	}
}

