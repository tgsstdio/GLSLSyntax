using NUnit.Framework;
using System;
using Irony.Parsing;

namespace GLSLSyntaxAST.UnitTests
{
	/// <summary>
	/// Really dodgy tests; NO TESTING AT ALL
	/// ATM just noting the test cases for the desired output
	/// </summary>
	[TestFixture ()]
	public class Test
	{
		[Test ()]
		public void TestCase ()
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser(lang);
			var tree = compiler.Parse ("void main(void) { float v = vec3(1,1,1); }");
			//CheckNodes (tree.Root, 0);			
		}

		[Test ()]
		public void StructTest01 ()
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser(lang);
			var tree = compiler.Parse ("struct Camera { float x;}");
			//CheckNodes (tree.Root, 0);			
		}	

		[Test ()]
		public void StructTest02 ()
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			var tree = compiler.Parse ("struct Camera { float x; int num; }; ");
		}

		[Test ()]
		public void StructTest03 ()
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			var tree = compiler.Parse ("struct Camera { float x; int num; vec3 output; vec2 data[10]; vec4 grid[3][4]; }; ");
		}

		[Test ()]
		public void StructTest04 ()
		{
			GLSLGrammar lang = new GLSLGrammar ();
			var compiler = new Irony.Parsing.Parser (lang);
			var tree = compiler.Parse ("struct Camera { float x; int num; vec3 output; vec2 data[10]; vec4 grid[3][4]; bool samples[]; };");
		}


	}
}

