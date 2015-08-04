using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class TDeferredCompiler
	{
		private EShLanguage language;
		public TInfoSink infoSink;
		public TDeferredCompiler(EShLanguage s, TInfoSink i)
		{
			language = s;
			infoSink = i;
		}

		public bool compile(object node, int num, Profile profile = Profile.NoProfile) 
		{
			return true; 
		}

		public EShLanguage getLanguage() { return language; }
	};
}

