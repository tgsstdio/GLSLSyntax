namespace GLSLSyntaxAST.Preprocessor
{
	public interface IInfoSinkComponent
	{
		IInfoSinkComponent AppendPrefix(PrefixType message);
		IInfoSinkComponent AppendLocation(SourceLocation loc);
		void WriteMessage(PrefixType message, string s);
		void WriteMessage (PrefixType message, string s, SourceLocation loc);
		IInfoSinkComponent Append (char[] s);
		IInfoSinkComponent Append (int count, char c);
		IInfoSinkComponent Append (string s) ;
		string ToString ();
	}
}

