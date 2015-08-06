namespace GLSLSyntaxAST.Preprocessor
{
	public class InfoSink
	{
		public InfoSink (IInfoSinkComponent info, IInfoSinkComponent debug)
		{
			Info = info;
			Debug = debug;
		}

		public IInfoSinkComponent Info {
			get;
			private set;
		}

		public IInfoSinkComponent Debug {
			get;
			private set;
		}
	}
}

