using System;
using NUnit.Framework;
using GLSLSyntaxAST.CodeDom;
using System.Text;

namespace GLSLSyntaxAST.UnitTests
{
	[TestFixture ()]	
	public class ProprocessorTests
	{
		[Test ()]
		public void TestCase ()
		{
			IPreprocessorContext ppContext = new PreprocessorContext ();
			TParseContext parseContext = new TParseContext ();

//			bool operator()(TParseContext& parseContext, TPpContext& ppContext,
//				TInputScanner& input, bool versionWillBeError,
//				TSymbolTable& , TIntermediate& ,
//				EShOptimizationLevel , EShMessages )
//			{
				// This is a list of tokens that do not require a space before or after.

				bool versionWillBeError = true;
				char[] unNeededSpaceTokens = {';','(',')','[',']'};
				char[] noSpaceBeforeTokens = {','};
				TPpToken token;

				StringBuilder outputStream = new StringBuilder();
				int lastLine = -1; // lastLine is the line number of the last token
				// processed. It is tracked in order for new-lines to be inserted when
				// a token appears on a new line.
				int lastToken = -1;
				parseContext.setScanner(&input);
				ppContext.setInput(input, versionWillBeError);

				// Inserts newlines and incremnets lastLine until
				// lastLine >= line.
				var adjustLine = (line) =>
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
						if (str) {
							outputStream.Append(" ").Append(str);
						}
						outputStream.AppendLine();
						++lastLine;
				};

				parseContext.PragmaCallback = (int line, string[] ops) =>
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

				while (var tok = ppContext.tokenize(token)) {
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
						(unNeededSpaceTokens.find((char)token.token) == std::string::npos) &&
						(unNeededSpaceTokens.find((char)lastToken) == std::string::npos) &&
						(noSpaceBeforeTokens.find((char)token.token) == std::string::npos)) {
						outputStream.Append(" ");
					}
					lastToken = token.token;
					outputStream.Append(tok);
				}
				outputStream.AppendLine();
				*outputString = outputStream.str();

				return true;
			}
		}
	}
}

