using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class InputScanner
	{
		internal InputScanner(string[] sources, int bias, int finale)
		{
			mSources = sources;
			currentSource = 0;
			currentChar = 0;
			stringBias = bias;
			mFinale = finale;

			// loc[0]
			loc = new SourceLocation[mSources.Length];
			loc[currentSource].stringBias = -stringBias;
			loc[currentSource].line = 1;
			loc[currentSource].column = 0;
		}

		// return of -1 means end of strings,
		// anything else is the next character

		// retrieve the next character and advance one character
		internal int get()
		{
			if (currentSource >= mSources.Length)
				return -1;

			if (mSources [currentSource].Length == 0)
				return -1;

			int ret = mSources[currentSource][currentChar];
			++loc[currentSource].column;
			if (ret == '\n') {
				++loc[currentSource].line;
				loc[currentSource].column = 0;
			}
			advance();

			return ret;
		}

		// advance one character
		void advance()
		{
			++currentChar;
			var length = mSources [currentSource].Length;
			if (currentChar >= length) {
				++currentSource;
				if (currentSource < mSources.Length) {
					loc[currentSource].stringBias = loc[currentSource - 1].stringBias + 1;
					loc[currentSource].line = 1;
					loc[currentSource].column = 0;
				}
				while (currentSource < mSources.Length && length == 0) {
					++currentSource;
					if (currentSource < mSources.Length) {
						loc[currentSource].stringBias = loc[currentSource - 1].stringBias + 1;
						loc[currentSource].line = 1;
						loc[currentSource].column = 0;
					}
				}
				currentChar = 0;
			}
		}

		internal void unget()
		{
			if (currentChar > 0) {
				--currentChar;
				--loc[currentSource].column;
				if (loc[currentSource].column < 0) {
					// We've moved back past a new line. Find the
					// previous newline (or start of the file) to compute
					// the column count on the now current line.
					int ch = currentChar;
					while(ch > 0) {
						if (mSources[currentSource][ch] == '\n') {
							break;
						}
						--ch;
					}
					loc[currentSource].column = currentChar - ch;
				}
			} else {
				var strLength = mSources [currentSource].Length;
				do {
					--currentSource;
				} while (currentSource > 0 && strLength == 0);
				if (strLength == 0) {
					// set to 0 if we've backed up to the start of an empty string
					currentChar = 0;
				} else
					currentChar = strLength - 1;
			}
			if (peek() == '\n')
				--loc[currentSource].line;
		}

		internal int peek()
		{
			if (currentSource >= mSources.Length)
				return -1;

			return mSources[currentSource][currentChar];
		}

		internal void setLine(int newLine)
		{
			loc[currentSource].line = newLine; 
		}

		internal void setString(int newString)
		{
			loc[currentSource].stringBias = newString; 
		}

		string [] mSources; // array of strings
		int currentSource;
		int currentChar;

		// This is for reporting what string/line an error occurred on, and can be overridden by #line.
		// It remembers the last state of each source string as it is left for the next one, so unget() 
		// can restore that state.
		SourceLocation[] loc;  // an array

		int stringBias;   // the first string that is the user's string number 0
		int mFinale;       // number of internal strings after user's last string

		internal SourceLocation getSourceLoc() { 
			return loc[Math.Max(0, Math.Min(currentSource, mSources.Length - mFinale - 1))]; 
		}

		// Returns true if there was non-white space (e.g., a comment, newline) before the #version
		// or no #version was found; otherwise, returns false.  There is no error case, it always
		// succeeds, but will leave version == 0 if no #version was found.
		//
		// Sets notFirstToken based on whether tokens (beyond white space and comments)
		// appeared before the #version.
		//
		// N.B. does not attempt to leave input in any particular known state.  The assumption
		// is that scanning will start anew, following the rules for the chosen version/profile,
		// and with a corresponding parsing context.
		//

		// read past any white space
		void consumeWhiteSpace(ref bool foundNonSpaceTab)
		{
			int c = peek();  // don't accidentally consume anything other than whitespace
			while (c == ' ' || c == '\t' || c == '\r' || c == '\n') {
				if (c == '\r' || c == '\n')
					foundNonSpaceTab = true;
				get();
				c = peek();
			}
		}

		// skip whitespace, then skip a comment, rinse, repeat
		void consumeWhitespaceComment(ref bool foundNonSpaceTab)
		{
			do {
				consumeWhiteSpace(ref foundNonSpaceTab);

				// if not starting a comment now, then done
				int c = peek();
				if (c != '/' || c < 0)
					return;

				// skip potential comment 
				foundNonSpaceTab = true;
				if (! consumeComment())
					return;

			} while (true);
		}

		// return true if a comment was actually consumed
		bool consumeComment()
		{
			if (peek() != '/')
				return false;

			get();  // consume the '/'
			int c = peek();
			if (c == '/') {

				// a '//' style comment
				get();  // consume the second '/'
				c = get();
				do {
					while (c > 0 && c != '\\' && c != '\r' && c != '\n')
						c = get();

					if (c <= 0 || c == '\r' || c == '\n') {
						while (c == '\r' || c == '\n')
							c = get();

						// we reached the end of the comment
						break;
					} else {
						// it's a '\', so we need to keep going, after skipping what's escaped

						// read the skipped character
						c = get();

						// if it's a two-character newline, skip both characters
						if (c == '\r' && peek() == '\n')
							get();
						c = get();
					}
				} while (true);

				// put back the last non-comment character
				if (c > 0)
					unget();

				return true;
			} else if (c == '*') {

				// a '/*' style comment
				get();  // consume the '*'
				c = get();
				do {
					while (c > 0 && c != '*')
						c = get();
					if (c == '*') {
						c = get();
						if (c == '/')
							break;  // end of comment
						// not end of comment
					} else // end of input
						break;
				} while (true);

				return true;
			} else {
				// it's not a comment, put the '/' back
				unget();

				return false;
			}
		}


		internal bool scanVersion(out int version, out Profile profile, out bool notFirstToken)
		{
			// This function doesn't have to get all the semantics correct,
			// just find the #version if there is a correct one present.
			// The preprocessor will have the responsibility of getting all the semantics right.

			bool versionNotFirst = false;  // means not first WRT comments and white space, nothing more
			notFirstToken = false;         // means not first WRT to real tokens
			version = 0;  // means not found
			profile = Profile.NoProfile;

			bool foundNonSpaceTab = false;
			bool lookingInMiddle = false;
			int c;
			do {
				if (lookingInMiddle) {
					notFirstToken = true;
					// make forward progress by finishing off the current line plus extra new lines
					if (peek() == '\n' || peek() == '\r') {
						while (peek() == '\n' || peek() == '\r')
							get();
					} else
						do {
							c = get();
						} while (c > 0 && c != '\n' && c != '\r');
					while (peek() == '\n' || peek() == '\r')
						get();
					if (peek() < 0)
						return true;
				}
				lookingInMiddle = true;

				// Nominal start, skipping the desktop allowed comments and white space, but tracking if 
				// something else was found for ES:
				consumeWhitespaceComment(ref foundNonSpaceTab);
				if (foundNonSpaceTab) 
					versionNotFirst = true;

				// "#"
				if (get() != '#') {
					versionNotFirst = true;
					continue;
				}

				// whitespace
				do {
					c = get();
				} while (c == ' ' || c == '\t');

				// "version"
				if (    c != 'v' ||
					get() != 'e' ||
					get() != 'r' ||
					get() != 's' ||
					get() != 'i' ||
					get() != 'o' ||
					get() != 'n') {
					versionNotFirst = true;
					continue;
				}

				// whitespace
				do {
					c = get();
				} while (c == ' ' || c == '\t');

				// version number
				while (c >= '0' && c <= '9') {
					version = 10 * version + (c - '0');
					c = get();
				}
				if (version == 0) {
					versionNotFirst = true;
					continue;
				}

				// whitespace
				while (c == ' ' || c == '\t')
					c = get();

				// profile
				const int MAX_PROFILE_LENGTH = 13;  // not including any 0
				var profileString = new char[MAX_PROFILE_LENGTH];
				int profileLength;
				for (profileLength = 0; profileLength < MAX_PROFILE_LENGTH; ++profileLength) {
					if (c < 0 || c == ' ' || c == '\t' || c == '\n' || c == '\r')
						break;
					profileString[profileLength] = (char)c;
					c = get();
				}
				if (c > 0 && c != ' ' && c != '\t' && c != '\n' && c != '\r') {
					versionNotFirst = true;
					continue;
				}

				var profileValue = new string(profileString, 0 , profileLength);
				if (profileLength == 2 && profileValue == "es")
					profile = Profile.EsProfile;
				else if (profileLength == 4 && profileValue == "core")
					profile = Profile.CoreProfile;
				else if (profileLength == 13 && profileValue == "compatibility")
					profile = Profile.CompatibilityProfile;

				return versionNotFirst;
			} while (true);
		}
	}
}

