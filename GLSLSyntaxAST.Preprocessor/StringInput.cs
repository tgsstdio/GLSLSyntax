namespace GLSLSyntaxAST.CodeDom
{
	public class StringInput : BasePreprocessorInput 
	{
		//
		// From PpScanner.cpp
		//
		public StringInput(PreprocessorContext pp, InputScanner i) : base(pp)
		{ 
			input = i;
		}

		public override int scan(ref PreprocessorToken ppToken)
		{
		//
		// Scanner used to tokenize source stream.
		//
			bool AlreadyComplained = false;
			int len = 0;
			int ch = 0;
			int ii = 0;
			uint ival = 0;

			ppToken.ival = 0;
			ppToken.space = false;
			ch = pp.getChar();

			for (;;) {
				while (ch == ' ' || ch == '\t') {
					ppToken.space = true;
					ch = pp.getChar();
				}

				ppToken.loc = pp.parseContext.getCurrentLoc();
				len = 0;
				switch (ch) {
				default:
					return ch; // Single character token, including '#' and '\' (escaped newlines are handled at a lower level, so this is just a '\' token)

				// TODO : EOF for stream
//				case EOF:
//					return endOfInput;

				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'G':
				case 'H':
				case 'I':
				case 'J':
				case 'K':
				case 'L':
				case 'M':
				case 'N':
				case 'O':
				case 'P':
				case 'Q':
				case 'R':
				case 'S':
				case 'T':
				case 'U':
				case 'V':
				case 'W':
				case 'X':
				case 'Y':
				case 'Z':
				case '_':
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'g':
				case 'h':
				case 'i':
				case 'j':
				case 'k':
				case 'l':
				case 'm':
				case 'n':
				case 'o':
				case 'p':
				case 'q':
				case 'r':
				case 's':
				case 't':
				case 'u':
				case 'v':
				case 'w':
				case 'x':
				case 'y':
				case 'z':
					do
					{
						if (len < StringInputBuffer.MAX_TOKEN_LENGTH)
						{
							pp.buffer.tokenText [len++] = (char)ch;
							ch = pp.getChar ();					
						} else
						{
							if (!AlreadyComplained)
							{
								pp.parseContext.error (ppToken.loc, "name too long", "", "");
								AlreadyComplained = true;
							}
							ch = pp.getChar ();
						}
					} while ((ch >= 'a' && ch <= 'z') ||
					         (ch >= 'A' && ch <= 'Z') ||
					         (ch >= '0' && ch <= '9') ||
					         ch == '_');

					// line continuation with no token before or after makes len == 0, and need to start over skipping white space, etc.
					if (len == 0)
						continue;

					pp.buffer.tokenText [len] = '\0';
					pp.ungetChar ();
					ppToken.atom = pp.LookUpAddString (new string (pp.buffer.tokenText, 0, len));
					return (int) CppEnums.IDENTIFIER;
				case '0':
					pp.buffer.name[len++] = (char)ch;
					ch = pp.getChar();
					if (ch == 'x' || ch == 'X') {
						// must be hexidecimal

						bool isUnsigned = false;
						pp.buffer.name[len++] = (char)ch;
						ch = pp.getChar();
						if ((ch >= '0' && ch <= '9') ||
							(ch >= 'A' && ch <= 'F') ||
							(ch >= 'a' && ch <= 'f')) {

							ival = 0;
							do {
								if (ival <= 0x0fffffff) {
									pp.buffer.name[len++] = (char)ch;
									if (ch >= '0' && ch <= '9') {
										ii = ch - '0';
									} else if (ch >= 'A' && ch <= 'F') {
										ii = ch - 'A' + 10;
									} else if (ch >= 'a' && ch <= 'f') {
										ii = ch - 'a' + 10;
									} else
										pp.parseContext.error(ppToken.loc, "bad digit in hexidecimal literal", "", "");
									ival = (uint)((ival << 4) | ii);
								} else {
									if (!AlreadyComplained) {
										pp.parseContext.error(ppToken.loc, "hexidecimal literal too big", "", "");
										AlreadyComplained = true;
									}
									ival = 0xffffffff;
								}
								ch = pp.getChar();
							} while ((ch >= '0' && ch <= '9') ||
								(ch >= 'A' && ch <= 'F') ||
								(ch >= 'a' && ch <= 'f'));
						} else {
							pp.parseContext.error(ppToken.loc, "bad digit in hexidecimal literal", "", "");
						}
						if (ch == 'u' || ch == 'U') {
							if (len < StringInputBuffer.MAX_TOKEN_LENGTH)
								pp.buffer.name[len++] = (char)ch;
							isUnsigned = true;
						} else
							pp.ungetChar();
						//NOT NEEDED
						//ppToken.name[len] = '\0';
						ppToken.ival = (int)ival;

						if (isUnsigned)
						{
							return (int)CppEnums.UINTCONSTANT;
						}
						else
						{	
							return (int)CppEnums.INTCONSTANT;
						}
					} else {
						// could be octal integer or floating point, speculative pursue octal until it must be floating point

						bool isUnsigned = false;
						bool octalOverflow = false;
						bool nonOctal = false;
						ival = 0;

						// see how much octal-like stuff we can read
						while (ch >= '0' && ch <= '7') {
							if (len < StringInputBuffer.MAX_TOKEN_LENGTH)
								pp.buffer.name[len++] = (char)ch;
							else if (! AlreadyComplained) {
								pp.parseContext.error(ppToken.loc, "numeric literal too long", "", "");
								AlreadyComplained = true;
							}
							if (ival <= 0x1fffffff) {
								ii = ch - '0';
								ival = (uint)((ival << 3) | ii);
							} else
								octalOverflow = true;
							ch = pp.getChar();
						}

						// could be part of a float...
						if (ch == '8' || ch == '9') {
							nonOctal = true;
							do {
								if (len < StringInputBuffer.MAX_TOKEN_LENGTH)
									pp.buffer.name[len++] = (char)ch;
								else if (! AlreadyComplained) {
									pp.parseContext.error(ppToken.loc, "numeric literal too long", "", "");
									AlreadyComplained = true;
								}
								ch = pp.getChar();
							} while (ch >= '0' && ch <= '9');
						}
						if (ch == '.' || ch == 'e' || ch == 'f' || ch == 'E' || ch == 'F' || ch == 'l' || ch == 'L')
						{
							ppToken.name = new string(pp.buffer.name, 0, len);	
							return pp.lFloatConst (len, ch, ppToken);
						}

						// wasn't a float, so must be octal...
						if (nonOctal)
							pp.parseContext.error(ppToken.loc, "octal literal digit too large", "", "");

						if (ch == 'u' || ch == 'U') {
							if (len < StringInputBuffer.MAX_TOKEN_LENGTH)
								pp.buffer.name[len++] = (char)ch;
							isUnsigned = true;
						} else
							pp.ungetChar();
						// NOT NEEDED
						//ppToken.name[len] = '\0';

						if (octalOverflow)
							pp.parseContext.error(ppToken.loc, "octal literal too big", "", "");

						ppToken.ival = (int)ival;
						ppToken.name = new string(pp.buffer.name, 0, len);	
						if (isUnsigned)
						{							
							return (int)CppEnums.UINTCONSTANT;
						}
						else
						{
							return (int)CppEnums.INTCONSTANT;
						}
					}
					break;
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return DoHexidecimal (pp.buffer, ppToken, ch, ref AlreadyComplained, ref len, StringInputBuffer.MAX_TOKEN_LENGTH);
					break;
				case '-':
					ch = pp.getChar();
					if (ch == '-') {
						return (int) CppEnums.DEC_OP;
					} else if (ch == '=') {
						return (int) CppEnums.SUB_ASSIGN;
					} else {
						pp.ungetChar();
						return '-';
					}
				case '+':
					ch = pp.getChar();
					if (ch == '+') {
						return (int) CppEnums.INC_OP;
					} else if (ch == '=') {
						return (int) CppEnums.ADD_ASSIGN;
					} else {
						pp.ungetChar();
						return '+';
					}
				case '*':
					ch = pp.getChar();
					if (ch == '=') {
						return (int) CppEnums.MUL_ASSIGN;
					} else {
						pp.ungetChar();
						return '*';
					}
				case '%':
					ch = pp.getChar();
					if (ch == '=') {
						return (int) CppEnums.MOD_ASSIGN;
					} else if (ch == '>'){
						return (int) CppEnums.RIGHT_BRACE;
					} else {
						pp.ungetChar();
						return '%';
					}
				case ':':
					ch = pp.getChar();
					if (ch == '>') {
						return (int) CppEnums.RIGHT_BRACKET;
					} else {
						pp.ungetChar();
						return ':';
					}
				case '^':
					ch = pp.getChar();
					if (ch == '^') {
						return (int) CppEnums.XOR_OP;
					} else {
						if (ch == '=')
						{
							return (int)CppEnums.XOR_ASSIGN;
						}
						else
						{
							pp.ungetChar();
							return '^';
						}
					}

				case '=':
					ch = pp.getChar();
					if (ch == '=') {
						return (int) CppEnums.EQ_OP;
					} else {						
						pp.ungetChar();
						return '=';
					}
				case '!':
					ch = pp.getChar();
					if (ch == '=') {
						return (int) CppEnums.NE_OP;
					} else {
						pp.ungetChar();
						return '!';
					}
				case '|':
					ch = pp.getChar();
					if (ch == '|') {
						return (int) CppEnums.OR_OP;
					} else {
						if (ch == '=')
						{	
							return (int)CppEnums.OR_ASSIGN;
						}
						else{
							pp.ungetChar();
							return '|';
						}
					}
				case '&':
					ch = pp.getChar();
					if (ch == '&') {
						return (int) CppEnums.AND_OP;
					} else {
						if (ch == '=')
						{
							return (int)CppEnums.AND_ASSIGN;
						}
						else{
							pp.ungetChar();
							return '&';
						}
					}
				case '<':
					ch = pp.getChar();
					if (ch == '<') {
						ch = pp.getChar();
						if (ch == '=')
						{
							return (int)CppEnums.LEFT_ASSIGN;
						}
						else
						{
							pp.ungetChar();
							return (int) CppEnums.LEFT_OP;
						}
					} else {
						if (ch == '=')
						{
							return (int) CppEnums.LE_OP;
						} 
						else 
						{
							if (ch == '%')
							{
								return (int)CppEnums.LEFT_BRACE;
							} else if (ch == ':')
							{
								return (int)CppEnums.LEFT_BRACKET;
							}
							else
							{
								pp.ungetChar();
								return '<';
							}
						}
					}
				case '>':
					ch = pp.getChar();
					if (ch == '>') {
						ch = pp.getChar();
						if (ch == '=')
						{
							return (int)CppEnums.RIGHT_ASSIGN;
						}
						else{
							pp.ungetChar();
							return (int) CppEnums.RIGHT_OP;
						}
					} else {
						if (ch == '=') {
							return (int) CppEnums.GE_OP;
						} else {
							pp.ungetChar();
							return '>';
						}
					}
				case '.':
					ch = pp.getChar();
					if (ch >= '0' && ch <= '9')
					{
						pp.ungetChar();
						return pp.lFloatConst(0, '.', ppToken);
					}
					else
					{
						pp.ungetChar();
						return '.';
					}
				case '/':
					ch = pp.getChar();
					if (ch == '/') {
						pp.inComment = true;
						do {
							ch = pp.getChar();
						} while (ch != '\n' && ! this.IsEOF(ch));
						ppToken.space = true;
						pp.inComment = false;

						if (this.IsEOF (ch))
						{	
							return END_OF_INPUT;
						}

						return ch;
					} else if (ch == '*') {
						ch = pp.getChar();
						do {
							while (ch != '*') {
								if (this.IsEOF(ch))
								{
									pp.parseContext.error(ppToken.loc, "EOF in comment", "comment", "");
									return END_OF_INPUT;
								}
								ch = pp.getChar();
							}
							ch = pp.getChar();
							if (this.IsEOF(ch))
							{
								pp.parseContext.error(ppToken.loc, "EOF in comment", "comment", "");
								return END_OF_INPUT;
							}
						} while (ch != '/');
						ppToken.space = true;
						// loop again to get the next token...
						break;
					} else if (ch == '=') {
						return (int) CppEnums.DIV_ASSIGN;
					} else {
						pp.ungetChar();
						return '/';
					}
					break;
				case '"':
					ch = pp.getChar();
					while (ch != '"' && ch != '\n' && this.IsEOF(ch)) {
						if (len < StringInputBuffer.MAX_TOKEN_LENGTH) {
							pp.buffer.tokenText[len] = (char)ch;
							len++;
							ch = pp.getChar();
						} else
							break;
					};
					//pp.buffer.tokenText[len] = '\0';
					if (ch == '"') {
						ppToken.atom = pp.LookUpAddString(new string(pp.buffer.tokenText, 0, len));
						ppToken.name = new string (pp.buffer.name, 0, len);
						return (int) CppEnums.STRCONSTANT;
					} else {
						pp.parseContext.error(ppToken.loc, "end of line in string", "string", "");
						ppToken.name = new string (pp.buffer.name, 0, len);
						return (int) CppEnums.ERROR_SY;
					}
				}

				ch = pp.getChar();
			}
		}

		private int DoHexidecimal(StringInputBuffer buffer, PreprocessorToken ppToken, int ch, ref bool alreadyComplained, ref int len, int maxTokenLength)
		{
			// can't be hexidecimal or octal, is either decimal or floating point

			do {
				if (len < maxTokenLength)
					buffer.name[len++] = (char)ch;
				else if (! alreadyComplained) {
					pp.parseContext.error(ppToken.loc, "numeric literal too long", "", "");
					alreadyComplained = true;
				}
				ch = pp.getChar();
			} while (ch >= '0' && ch <= '9');
			if (ch == '.' || ch == 'e' || ch == 'f' || ch == 'E' || ch == 'F' || ch == 'l' || ch == 'L')
			{
				ppToken.name = new string (buffer.name, 0, len);				
				return pp.lFloatConst(len, ch, ppToken);
			} 
			else
			{
				// Finish handling signed and unsigned integers
				int numericLen = len;
				bool uintSign = false;
				if (ch == 'u' || ch == 'U') 
				{
					if (len < maxTokenLength)
						buffer.name[len++] = (char)ch;
					uintSign = true;
				} else
					pp.ungetChar();

				// NOT NEEDED
				//ppToken.name[len] = '\0';
				uint ival = 0;
				const uint ONETENTHMAXINT = 0xFFFFFFFFu / 10;
				const uint REMAINDERMAXINT = 0xFFFFFFFFu - 10 * ONETENTHMAXINT;
				for (int i = 0; i < numericLen; i++) 
				{
					ch = buffer.name[i] - '0';
					if ((ival > ONETENTHMAXINT) || (ival == ONETENTHMAXINT && ch > REMAINDERMAXINT)) 
					{
						pp.parseContext.error(ppToken.loc, "numeric literal too big", "", "");
						ival = 0xFFFFFFFFu;
						break;
					} 
					else
						ival = (uint)(ival * 10 + ch);
				}
				ppToken.ival = (int)ival;
				ppToken.name = new string (buffer.name, 0, len);
				return uintSign ? (int)CppEnums.UINTCONSTANT : (int)CppEnums.INTCONSTANT;
			}
		}

		private bool IsEOF(int c)
		{
			return false;
		}

		// Scanner used to get source stream characters.
		//  - Escaped newlines are handled here, invisibly to the caller.
		//  - All forms of newline are handled, and turned into just a '\n'.
		public override int getch()
		{
			int ch = input.get();

			if (ch == '\\') {
				// Move past escaped newlines, as many as sequentially exist
				do {
					if (input.peek() == '\r' || input.peek() == '\n') {
						bool allowed = pp.parseContext.lineContinuationCheck(input.getSourceLoc(), pp.inComment);
						if (! allowed && pp.inComment)
							return '\\';

						// escape one newline now
						ch = input.get();
						int nextch = input.get();
						if (ch == '\r' && nextch == '\n')
							ch = input.get();
						else
							ch = nextch;
					} else
						return '\\';
				} while (ch == '\\');
			}

			// handle any non-escaped newline
			if (ch == '\r' || ch == '\n') {
				if (ch == '\r' && input.peek() == '\n')
					ch = input.get();
				return '\n';
			}

			return ch;
		}

		// Scanner used to backup the source stream characters.  Newlines are
		// handled here, invisibly to the caller, meaning have to undo exactly
		// what getch() above does (e.g., don't leave things in the middle of a
		// sequence of escaped newlines).
		public override void ungetch()
		{
			input.unget();

			do {
				int ch = input.peek();
				if (ch == '\r' || ch == '\n') {
					if (ch == '\n') {
						// correct for two-character newline
						input.unget();
						if (input.peek() != '\r')
							input.get();
					}
					// now in front of a complete newline, move past an escape character
					input.unget();
					if (input.peek() == '\\')
						input.unget();
					else {
						input.get();
						break;
					}
				} else
					break;
			} while (true);
		}

		protected InputScanner input;
	}
}

