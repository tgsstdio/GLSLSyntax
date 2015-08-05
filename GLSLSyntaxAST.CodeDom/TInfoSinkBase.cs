using System;
using System.Text;

namespace GLSLSyntaxAST.CodeDom
{
	public class TInfoSinkBase
	{
//		public:
//		TInfoSinkBase() : outputStream(4) {}
//		void erase() { sink.erase(); }
//		TInfoSinkBase& operator<<(const TPersistString& t) { append(t); return *this; }
//		TInfoSinkBase& operator<<(char c)                  { append(1, c); return *this; }
//		TInfoSinkBase& operator<<(const char* s)           { append(s); return *this; }
//		TInfoSinkBase& operator<<(int n)                   { append(String(n)); return *this; }
//		TInfoSinkBase& operator<<(unsigned int n)          { append(String(n)); return *this; }
//		TInfoSinkBase& operator<<(long unsigned int n)     { append(String(n)); return *this; }
//		TInfoSinkBase& operator<<(float n)                 { const int size = 40; char buf[size]; 
//			snprintf(buf, size, (fabs(n) > 1e-8 && fabs(n) < 1e8) || n == 0.0f ? "%f" : "%g", n);
//			append(buf); 
//			return *this; }
//		TInfoSinkBase& operator+(const TPersistString& t)  { append(t); return *this; }
//		TInfoSinkBase& operator+(const TString& t)         { append(t); return *this; }
//		TInfoSinkBase& operator<<(const TString& t)        { append(t); return *this; }
//		TInfoSinkBase& operator+(const char* s)            { append(s); return *this; }
//		const char* c_str() const { return sink.c_str(); }

		private readonly StringBuilder sink;
		public TOutputStream outputStream;
		public TInfoSinkBase ()
		{
			sink = new StringBuilder();
			outputStream = TOutputStream.EString;
		}

		[Flags]
		public enum TPrefixType : int
		{
			EPrefixNone = 0,
			EPrefixWarning,
			EPrefixError,
			EPrefixInternalError,
			EPrefixUnimplemented,
			EPrefixNote
		};

		[Flags]
		public enum TOutputStream : int
		{
			ENull = 0,
			EDebugger = 0x01,
			EStdOut = 0x02,
			EString = 0x04,
		};

		public override string ToString()
		{
			return sink.ToString ();
		}

		public void prefix(TPrefixType message)
		{
			switch(message) {
				case TPrefixType.EPrefixNone:
					break;
				case TPrefixType.EPrefixWarning:
					append("WARNING: ");
					break;
				case TPrefixType.EPrefixError:
					append("ERROR: ");
					break;
				case TPrefixType.EPrefixInternalError:
					append("INTERNAL ERROR: ");
					break;
				case TPrefixType.EPrefixUnimplemented:
					append("UNIMPLEMENTED: ");
					break;
				case TPrefixType.EPrefixNote:
					append("NOTE: ");
					break;
				default:
					append("UNKOWN ERROR: ");
					break;
			}
		}

		public void location(TSourceLoc loc)
		{
			const int maxSize = 24;
			string locText;
			if (loc.name != null) {
				append(loc.name);
				locText = loc.line.ToString();
			} else {
				locText = string.Format("{0}:{1}", loc.stringBias, loc.line);
			}
			append(locText);
			append(": ");
		}

		public void message(TPrefixType message, string s) 
		{
			prefix(message);
			append(s);
			append("\n");
		}

		public void message(TPrefixType message, string s, TSourceLoc loc)
		{
			prefix(message);
			location(loc);
			append(s);
			append("\n");
		}

		public void setOutputStream(TOutputStream output = TOutputStream.EString)
		{
			outputStream = output;
		}

		protected void append(char[] s)           
		{
			if ((outputStream & TOutputStream.EString) > 0) {
				sink.Append(s); 
			}

			//#ifdef _WIN32
			//    if (outputStream & EDebugger)
			//        OutputDebugString(s);
			//#endif

			if ((outputStream & TOutputStream.EStdOut) > 0)
			{
				foreach (var letter in s)
				{
					Console.Write (letter);	
				}
			}
		}

		public TInfoSinkBase append(string s)           
		{
			if ((outputStream & TOutputStream.EString) > 0) {
				sink.Append(s); 
			}

			//#ifdef _WIN32
			//    if (outputStream & EDebugger)
			//        OutputDebugString(s);
			//#endif

			if ((outputStream & TOutputStream.EStdOut) > 0)
				Console.Write(s);
			return this;
		}

		protected void append(int count, char c)
		{ 
			if ((outputStream & TOutputStream.EString) > 0){     
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

			if ((outputStream & TOutputStream.EStdOut) > 0)
				Console.Write(c);
		}
	};
}

