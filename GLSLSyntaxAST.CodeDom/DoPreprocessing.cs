using System;
using System.Collections.Generic;
using System.Text;

namespace GLSLSyntaxAST.CodeDom
{
	public class DoPreprocessing
	{
		public DoPreprocessing ()
		{
		}

		public bool DoStuff (TParseContext parseContext, PreprocessorContext ppContext,
		               TInputScanner input, bool versionWillBeError)
		{
			//bool versionWillBeError = true;
			var unNeededSpaceTokens = new HashSet<char>(new char[]{';','(',')','[',']'});
			var noSpaceBeforeTokens =  new HashSet<char>(new char[]{','});

			var outputStream = new StringBuilder();
			int lastLine = -1; // lastLine is the line number of the last token
			// processed. It is tracked in order for new-lines to be inserted when
			// a token appears on a new line.
			int lastToken = -1;
			parseContext.setScanner(input);
			ppContext.setInput(input, versionWillBeError);

			// Inserts newlines and incremnets lastLine until
			// lastLine >= line.
			Action<int> adjustLine = (line) =>
			{
				int tokenLine = line - 1;
				while(lastLine < tokenLine) {
					if (lastLine >= 0) {
						outputStream.AppendLine();
					}
					++lastLine;
				}
			};

			parseContext.ExtensionCallback = (int line, string extension, string behavior) => 
			{
				adjustLine(line);
				outputStream.Append("#extension ").Append(extension).Append(" : ").Append(behavior);
			};

			parseContext.LineCallback = (int line, bool hasSource, int sourceNum) => 
			{
				// SourceNum is the number of the source-string that is being parsed.
				if (lastLine != -1) {
					outputStream.AppendLine();
				}
				outputStream.Append("#line ").Append(line);
				if (hasSource) {
					outputStream.Append(" ").Append(sourceNum);
				}
				outputStream.AppendLine();
				lastLine = System.Math.Max(line - 1, 1);
			};


			parseContext.VersionCallback = (int line, int version, string str) =>
			{
				adjustLine(line);
				outputStream.Append("#version ").Append(version);
				if (str != null) {
					outputStream.Append(" ").Append(str);
				}
				outputStream.AppendLine();
				++lastLine;
			};

			parseContext.PragmaCallback = (int line, List<string> ops) =>
			{
				adjustLine(line);
				outputStream.Append("#pragma ");
				foreach(var op in ops) {
					outputStream.Append(op);
				}
			};

			parseContext.ErrorCallback = (int line, string errorMessage) =>
			{
				adjustLine(line);
				outputStream.Append("#error ").Append(errorMessage);
			};

			var token = new TPpToken();
			string tok = ppContext.tokenize (ref token);
			while (tok != null) {
				int tokenLine = token.loc.line - 1;  // start at 0;
				bool newLine = false;
				while (lastLine < tokenLine) {
					if (lastLine > -1) {
						outputStream.AppendLine();
						newLine = true;
					}
					++lastLine;
					if (lastLine == tokenLine) {
						// Don't emit whitespace onto empty lines.
						// Copy any whitespace characters at the start of a line
						// from the input to the output.
						for(int i = 0; i < token.loc.column - 1; ++i) {
							outputStream.Append(" ");
						}
					}
				}

				// Output a space in between tokens, but not at the start of a line,
				// and also not around special tokens. This helps with readability
				// and consistency.
				if (!newLine &&
					lastToken != -1 &&
					(!unNeededSpaceTokens.Contains ((char)token.token)) &&
					(!unNeededSpaceTokens.Contains((char)lastToken)) &&
					(!noSpaceBeforeTokens.Contains ((char)token.token))) 
				{
					outputStream.Append(" ");
				}
				lastToken = token.token;
				outputStream.Append(tok);
				tok = ppContext.tokenize (ref token);
			}

			outputStream.AppendLine();
			//outputString = outputStream.str();

			return true;
		}

		public string Output;
	}
}

