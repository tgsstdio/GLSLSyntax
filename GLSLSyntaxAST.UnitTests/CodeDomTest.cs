using GLSLSyntaxAST.CodeDom;
using NUnit.Framework;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]
	public class CodeDomTest
	{
		[Test ()]
		public void Initialize ()
		{
			IGLSLTypeLookup lookup = new OpenTKTypeLookup ();			
			IGLSLUniformExtractor test = new GLSLUniformExtractor (lookup);
			test.Initialize ();		
		}
	}
}

