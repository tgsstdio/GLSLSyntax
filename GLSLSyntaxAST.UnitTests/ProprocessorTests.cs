using NUnit.Framework;
using GLSLSyntaxAST.Preprocessor;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]	
	public class ProprocessorTests
	{
		[Test ()]
		public void TestCase ()
		{			
			var debug = new InfoSinkBase (SinkType.String);
			var info = new InfoSinkBase (SinkType.String);
			var infoSink = new InfoSink(info, debug);
			var intermediate = new GLSLIntermediate ();
			var symbols = new SymbolLookup ();
			symbols.SetPreambleManually (Profile.CoreProfile);
			symbols.DefineAs ("GL_ARB_shader_storage_buffer_object", 1);
			var preprocessor = new Standalone (infoSink, intermediate, symbols);
			string result;
			Assert.IsTrue(preprocessor.Run("Sample.vert", out result));
			Assert.IsNotNull (result);
		}
	}
}

