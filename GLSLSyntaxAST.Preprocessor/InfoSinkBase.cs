using System;
using System.Text;

namespace GLSLSyntaxAST.Preprocessor
{
	public class InfoSinkBase : IInfoSinkComponent
	{
		private readonly StringBuilder sink;
		private SinkType mSinkType;
		public InfoSinkBase (SinkType sinkType)
		{
			sink = new StringBuilder();
			mSinkType = sinkType;
		}

		public override string ToString()
		{
			return sink.ToString ();
		}

		public IInfoSinkComponent AppendPrefix(PrefixType message)
		{
			switch(message) {
				case PrefixType.None:
					break;
				case PrefixType.Warning:
					Append("WARNING: ");
					break;
				case PrefixType.Error:
					Append("ERROR: ");
					break;
				case PrefixType.InternalError:
					Append("INTERNAL ERROR: ");
					break;
				case PrefixType.Unimplemented:
					Append("UNIMPLEMENTED: ");
					break;
				case PrefixType.Note:
					Append("NOTE: ");
					break;
				default:
					Append("UNKOWN ERROR: ");
					break;
			}
			return this;
		}

		public IInfoSinkComponent AppendLocation(SourceLocation loc)
		{
			string locText;
			if (loc.name != null) {
				Append(loc.name);
				locText = loc.line.ToString();
			} else {
				locText = string.Format("{0}:{1}", loc.stringBias, loc.line);
			}
			Append(locText);
			Append(": ");
			return this;
		}

		public void WriteMessage(PrefixType message, string s) 
		{
			AppendPrefix(message);
			Append(s);
			Append("\n");
		}

		public void WriteMessage(PrefixType message, string s, SourceLocation loc)
		{
			AppendPrefix(message);
			AppendLocation(loc);
			Append(s);
			Append("\n");
		}

		public IInfoSinkComponent Append(char[] s)           
		{
			if ((mSinkType & SinkType.String) > 0) {
				sink.Append(s); 
			}

			//#ifdef _WIN32
			//    if (outputStream & EDebugger)
			//        OutputDebugString(s);
			//#endif

			if ((mSinkType & SinkType.StdOut) > 0)
			{
				foreach (var letter in s)
				{
					Console.Write (letter);	
				}
			}
			return this;
		}

		public IInfoSinkComponent Append(string s)           
		{
			if ((mSinkType & SinkType.String) > 0) {
				sink.Append(s); 
			}

			//#ifdef _WIN32
			//    if (outputStream & EDebugger)
			//        OutputDebugString(s);
			//#endif

			if ((mSinkType & SinkType.StdOut) > 0)
				Console.Write(s);
			return this;
		}

		public IInfoSinkComponent Append(int count, char c)
		{ 
			if ((mSinkType & SinkType.String) > 0){     
				sink.Append(new String(c, count)); 
			}

			//#ifdef _WIN32
			//    if (outputStream & EDebugger) {
			//        char str[2];
			//        str[0] = c;
			//        str[1] = '\0';
			//        OutputDebugString(str);
			//    }
			//#endif

			if ((mSinkType & SinkType.StdOut) > 0)
				Console.Write(c);
			return this;
		}
	};
}

