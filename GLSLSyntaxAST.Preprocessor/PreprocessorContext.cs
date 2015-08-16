using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class PreprocessorContext
	{
		internal void setInput (InputScanner input, bool versionWillBeError)
		{
			if (inputStack.Count != 0)
			{
				throw new Exception ("Stack is use");
			}

			pushInput(new StringInput(this, input));

			errorOnVersion = versionWillBeError;
			versionSeen = false;
		}

		const int maxMacroArgs = 64;
		const int MAXIFNESTING = 64;

		int ifdepth;  // current #if-#else-#endif nesting in the cpp.c file (pre-processor)
		bool[] elseSeen; 			 // Keep a track of whether an else has been seen at a particular depth
		int elsetracker; // #if-#else and #endif constructs...Counter.

		// Scanner data:
		int previous_token;
		public ParseContext parseContext;
		internal bool inComment;

		public StringInputBuffer buffer;
		public SymbolLookup Symbols { get; private set; }
		public PreprocessorContext (ParseContext pc, SymbolLookup symbols)
		{
			Symbols = symbols;
			parseContext = pc;
			inComment = false;

			InitAtomTable();
			InitScanner();

			ifdepth = 0;
			elseSeen = new bool[MAXIFNESTING];
			for (elsetracker = 0; elsetracker < MAXIFNESTING; elsetracker++)
				elseSeen[elsetracker] = false;
			elsetracker = 0;

			inputStack = new Stack<BasePreprocessorInput> ();

			buffer = new StringInputBuffer{ name = new char[StringInputBuffer.MAX_TOKEN_LENGTH], tokenText = new char[StringInputBuffer.MAX_TOKEN_LENGTH]};
		}

		private class BinaryEvalOperation
		{
			public int token;
			public EvalPrecedence precedence;
			public Func<int, int, int> op;
		}

		//private Dictionary<string, int> atomMap;
		//private Dictionary<int, string> stringMap;
		private List<BinaryEvalOperation> binop;
		//private int nextAtom;
		private void InitAtomTable()
		{
			binop = new List<BinaryEvalOperation> ();
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.OR_OP, precedence = EvalPrecedence.LOGOR, op = op_logor });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.AND_OP, precedence = EvalPrecedence.LOGAND, op = op_logand });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("|"), precedence = EvalPrecedence.OR, op = op_logand });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("^"), precedence = EvalPrecedence.XOR, op = op_xor });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("&"), precedence = EvalPrecedence.AND, op = op_and });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.EQ_OP, precedence = EvalPrecedence.EQUAL, op = op_eq });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.NE_OP, precedence = EvalPrecedence.EQUAL, op = op_ne });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey(">"), precedence = EvalPrecedence.RELATION, op = op_gt });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.GE_OP, precedence = EvalPrecedence.RELATION, op = op_ge });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("<"), precedence = EvalPrecedence.RELATION, op = op_lt });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.LE_OP, precedence = EvalPrecedence.RELATION, op = op_le });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.LEFT_OP, precedence = EvalPrecedence.SHIFT, op = op_shl });
			binop.Add (new BinaryEvalOperation{ token = (int)CppEnums.RIGHT_OP, precedence = EvalPrecedence.SHIFT, op = op_shr });

			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("+"), precedence = EvalPrecedence.ADD, op = op_add });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("-"), precedence = EvalPrecedence.ADD, op = op_sub });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("*"), precedence = EvalPrecedence.MUL, op = op_mul });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("/"), precedence = EvalPrecedence.MUL, op = op_div });
			binop.Add (new BinaryEvalOperation{ token = Symbols.Atoms.GetAtomKey("%"), precedence = EvalPrecedence.MUL, op = op_mod });
		}

		internal int InitScanner()
		{
			// Add various atoms needed by the CPP line scanner:
			if (!InitCPP())
				return 0;

			previous_token = '\n';

			return 1;
		}

		//
		// from Pp.cpp
		//
		int bindAtom;
		int constAtom;
		int defaultAtom;
		int defineAtom;
		int definedAtom;
		int elseAtom;
		int elifAtom;
		int endifAtom;
		int ifAtom;
		int ifdefAtom;
		int ifndefAtom;
		int includeAtom;
		int lineAtom;
		int pragmaAtom;
		int texunitAtom;
		int undefAtom;
		int errorAtom;
		int __LINE__Atom;
		int __FILE__Atom;
		int __VERSION__Atom;
		int versionAtom;
		int coreAtom;
		int compatibilityAtom;
		int esAtom;
		int extensionAtom;

		private bool InitCPP()
		{
			// Add various atoms needed by the CPP line scanner:
			bindAtom = Symbols.Atoms.LookUpAddString("bind");
			constAtom = Symbols.Atoms.LookUpAddString("const");
			defaultAtom = Symbols.Atoms.LookUpAddString("default");
			defineAtom = Symbols.Atoms.LookUpAddString("define");
			definedAtom = Symbols.Atoms.LookUpAddString("defined");
			elifAtom = Symbols.Atoms.LookUpAddString("elif");
			elseAtom = Symbols.Atoms.LookUpAddString("else");
			endifAtom = Symbols.Atoms.LookUpAddString("endif");
			ifAtom = Symbols.Atoms.LookUpAddString("if");
			ifdefAtom = Symbols.Atoms.LookUpAddString("ifdef");
			ifndefAtom = Symbols.Atoms.LookUpAddString("ifndef");
			includeAtom = Symbols.Atoms.LookUpAddString("include");
			lineAtom = Symbols.Atoms.LookUpAddString("line");
			pragmaAtom = Symbols.Atoms.LookUpAddString("pragma");
			texunitAtom = Symbols.Atoms.LookUpAddString("texunit");
			undefAtom = Symbols.Atoms.LookUpAddString("undef");
			errorAtom = Symbols.Atoms.LookUpAddString("error");
			__LINE__Atom = Symbols.Atoms.LookUpAddString("__LINE__");
			__FILE__Atom = Symbols.Atoms.LookUpAddString("__FILE__");
			__VERSION__Atom = Symbols.Atoms.LookUpAddString("__VERSION__");
			versionAtom = Symbols.Atoms.LookUpAddString("version");
			coreAtom = Symbols.Atoms.LookUpAddString("core");
			compatibilityAtom = Symbols.Atoms.LookUpAddString("compatibility");
			esAtom = Symbols.Atoms.LookUpAddString("es");
			extensionAtom = Symbols.Atoms.LookUpAddString("extension");
			//pool = mem_CreatePool(0, 0);

			return true;
		}

		private Stack<BasePreprocessorInput> inputStack;
		private bool errorOnVersion;
		private bool versionSeen;
		internal void SetInput(InputScanner input, bool versionWillBeError)
		{
			Debug.Assert(inputStack.Count == 0);

			pushInput(new StringInput(this, input));

			errorOnVersion = versionWillBeError;
			versionSeen = false;
		}

		void pushInput(BasePreprocessorInput item)
		{
			inputStack.Push(item);
		}

		internal int getChar()
		{
			return inputStack.Peek().getch();
		}

		internal void ungetChar()
		{
			inputStack.Peek().ungetch();
		}

		internal void popInput()
		{
			inputStack.Pop ();
		}

		// Get the next token from *stack* of input sources, popping input sources
		// that are out of tokens, down until an input sources is found that has a token.
		// Return EOF when there are no more tokens to be found by doing this.
		internal int scanToken(ref PreprocessorToken ppToken)
		{
			int token = BasePreprocessorInput.EOF;

			while (inputStack.Count > 0) {
				token = inputStack.Peek().scan(ref ppToken);
				if (token != BasePreprocessorInput.END_OF_INPUT)
					break;
				popInput();
			}

			if (token == BasePreprocessorInput.END_OF_INPUT)
				return BasePreprocessorInput.EOF;

			return token;
		}

		internal string tokenize(ref PreprocessorToken ppToken)
		{    
			int token = '\n';

			for(;;) {
				string tokenString = null;
				token = scanToken(ref ppToken);
				ppToken.token = token;
				if (token == BasePreprocessorInput.EOF) {
					missingEndifCheck();
					return null;
				}
				if (token == '#') {
					if (previous_token == '\n') {
						token = readCPPline(ref ppToken);
						if (token == BasePreprocessorInput.EOF) {
							missingEndifCheck();
							return null;
						}
						continue;
					} else {
						parseContext.Error(ppToken.loc, "preprocessor directive cannot be preceded by another token", "#", "");
						return null;
					}
				}
				previous_token = token;

				if (token == '\n')
					continue;

				// expand macros
				if (token == (int) CppEnums.IDENTIFIER && MacroExpand(ppToken.atom, ppToken, false, true) != 0)
					continue;

				if (token == (int) CppEnums.IDENTIFIER)
					tokenString = Symbols.Atoms.GetAtomString(ppToken.atom);
				else if (token == (int)  CppEnums.INTCONSTANT || token == (int) CppEnums.UINTCONSTANT ||
					token == (int) CppEnums.FLOATCONSTANT || token == (int) CppEnums.DOUBLECONSTANT)
					tokenString = ppToken.name;
				else if (token == (int) CppEnums.STRCONSTANT) {
					parseContext.Error(ppToken.loc, "string literals not supported", "\"\"", "");
					tokenString = null;
				} else if (token == '\'') {
					parseContext.Error(ppToken.loc, "character literals not supported", "\'", "");
					tokenString = null;
				} else
					tokenString = Symbols.Atoms.GetAtomString(token);

				if (tokenString != null) {
					if (tokenString[0] != 0)
						parseContext.tokensBeforeEOF = true;

					return tokenString;
				}
			}
		}

		internal int readCPPline(ref PreprocessorToken ppToken)
		{
			int token = scanToken(ref ppToken);
			bool isVersion = false;

			if (token == (int) CppEnums.IDENTIFIER) {
				if (ppToken.atom == defineAtom) {
					token = CPPdefine(ref ppToken);
				} else if (ppToken.atom == elseAtom) {
					// ORIGINALLY if (elsetracker[elseSeen])
					if (elseSeen[elsetracker])
						parseContext.Error(ppToken.loc, "#else after #else", "#else", "");
					// ORIGINALLY elsetracker[elseSeen] = true;
					elseSeen[elsetracker] = true;
					//if (! ifdepth)
					if (ifdepth <= 0)
						parseContext.Error(ppToken.loc, "mismatched statements", "#else", "");
					token = extraTokenCheck(elseAtom, ref ppToken, scanToken(ref ppToken));
					token = CPPelse(false, ref ppToken);
				} else if (ppToken.atom == elifAtom) {
					// (! ifdepth) <==> (ifdepth <= 0)
					if  (ifdepth <= 0)
						parseContext.Error(ppToken.loc, "mismatched statements", "#elif", "");
					if (elseSeen[elsetracker])
						parseContext.Error(ppToken.loc, "#elif after #else", "#elif", "");
					// this token is really a dont care, but we still need to eat the tokens
					token = scanToken(ref ppToken); 
					while (token != '\n')
						token = scanToken(ref ppToken);
					token = CPPelse(false, ref ppToken);
				} else if (ppToken.atom == endifAtom) {
					elseSeen[elsetracker] = false;
					--elsetracker;
					if (ifdepth <= 0)
						parseContext.Error(ppToken.loc, "mismatched statements", "#endif", "");
					else
						--ifdepth;
					token = extraTokenCheck(endifAtom, ref ppToken, scanToken(ref ppToken));
				} else if (ppToken.atom == ifAtom) {
					token = CPPif (ref ppToken);
				} else if (ppToken.atom == ifdefAtom) {
					token = CPPifdef(true, ref ppToken);
				} else if (ppToken.atom == ifndefAtom) {
					token = CPPifdef(false, ref ppToken);
				} else if (ppToken.atom == lineAtom) {
					token = CPPline(ref ppToken);
				} else if (ppToken.atom == pragmaAtom) {
					token = CPPpragma(ref ppToken);
				} else if (ppToken.atom == undefAtom) {
					token = CPPundef(ref ppToken);
				} else if (ppToken.atom == errorAtom) {
					token = CPPerror(ref ppToken);
				} else if (ppToken.atom == versionAtom) {
					token = CPPversion(ref ppToken);
					isVersion = true;
				} else if (ppToken.atom == extensionAtom) {
					token = CPPextension(ref ppToken);
				} else {
					parseContext.Error(ppToken.loc, "invalid directive:", "#", Symbols.Atoms.GetAtomString(ppToken.atom));
				}
			} else if (token != '\n' && token != BasePreprocessorInput.EOF)
				parseContext.Error(ppToken.loc, "invalid directive", "#", "");

			while (token != '\n' && token != 0 && token != BasePreprocessorInput.EOF)
				token = scanToken(ref ppToken);

			return token;
		}

		// Handle #define
		int CPPdefine(ref PreprocessorToken ppToken)
		{
			MacroSymbol mac = new MacroSymbol ();
			Symbol symb;

			// get macro name
			int token = scanToken(ref ppToken);
			if (token != (int) CppEnums.IDENTIFIER) {
				parseContext.Error(ppToken.loc, "must be followed by macro name", "#define", "");
				return token;
			}
			int atom = ppToken.atom;
			string definedName = Symbols.Atoms.GetAtomString(atom);
			if (ppToken.loc.stringBias >= 0) {
				// We are in user code; check for reserved name use:
				parseContext.ReservedPpErrorCheck(ppToken.loc, definedName, "#define");
			}

			// gather parameters to the macro, between (...)
			token = scanToken(ref ppToken);
			if (token == '(' && ! ppToken.space) {
				int argc = 0;
				int[] args = new int[maxMacroArgs];
				do {
					token = scanToken(ref ppToken);
					if (argc == 0 && token == ')') 
						break;
					if (token != (int) CppEnums.IDENTIFIER) {
						parseContext.Error(ppToken.loc, "bad argument", "#define", "");

						return token;
					}
					// check for duplication of parameter name
					bool duplicate = false;
					for (int a = 0; a < argc; ++a) {
						if (args[a] == ppToken.atom) {
							parseContext.Error(ppToken.loc, "duplicate macro parameter", "#define", "");
							duplicate = true;
							break;
						}
					}
					if (! duplicate) {
						if (argc < maxMacroArgs)
							args[argc++] = ppToken.atom;
						else
							parseContext.Error(ppToken.loc, "too many macro parameters", "#define", "");                    
					}
					token = scanToken(ref ppToken);
				} while (token == ',');
				if (token != ')') {            
					parseContext.Error(ppToken.loc, "missing parenthesis", "#define", "");

					return token;
				}
				mac.argc = argc;
				mac.args = args;
				token = scanToken(ref ppToken);
			}

			// record the definition of the macro
			SourceLocation defineLoc = ppToken.loc; // because ppToken is going to go to the next line before we report errors
			mac.body = new TokenStream();
			while (token != '\n') {
				Symbols.Atoms.RecordToken(mac.body, token, ppToken);
				token = scanToken(ref ppToken);
				if (token != '\n' && ppToken.space)
					Symbols.Atoms.RecordToken(mac.body, ' ', ppToken);
			}

			// check for duplicate definition
			symb = Symbols.LookUp(atom);
			if (symb != null) {
				if (! symb.mac.undef) {
					// Already defined -- need to make sure they are identical:
					// "Two replacement lists are identical if and only if the preprocessing tokens in both have the same number,
					// ordering, spelling, and white-space separation, where all white-space separations are considered identical."
					if (symb.mac.argc != mac.argc)
						parseContext.Error(defineLoc, "Macro redefined; different number of arguments:", "#define", Symbols.Atoms.GetAtomString(atom));
					else {
						for (int argc = 0; argc < mac.argc; argc++) {
							if (symb.mac.args[argc] != mac.args[argc])
								parseContext.Error(defineLoc, "Macro redefined; different argument names:", "#define", Symbols.Atoms.GetAtomString(atom));
						}
						symb.mac.body.Rewind();
						mac.body.Rewind();
						int newToken;
						do {
							int oldToken;
							PreprocessorToken oldPpToken = new PreprocessorToken();
							PreprocessorToken newPpToken = new PreprocessorToken();                    
							oldToken = ReadToken(symb.mac.body, oldPpToken);
							newToken = ReadToken(mac.body, newPpToken);
							if (oldToken != newToken || oldPpToken != newPpToken) {
								parseContext.Error(defineLoc, "Macro redefined; different substitutions:", "#define", Symbols.Atoms.GetAtomString(atom));
								break; 
							}
						} while (newToken > 0);
					}
				}
			} else
				symb = Symbols.Add(atom);

			symb.mac.body = null;
			symb.mac = mac;

			return '\n';
		}

		SourceLocation ifloc; /* outermost #if */
		// Handle #if
		int CPPif(ref PreprocessorToken ppToken) 
		{
			int token = scanToken(ref ppToken);
			elsetracker++;

			//if (! ifdepth++)
			if (ifdepth++ <= 0)
				ifloc = ppToken.loc;
			if (ifdepth > MAXIFNESTING) {
				parseContext.Error(ppToken.loc, "maximum nesting depth exceeded", "#if", "");
				return 0;
			}
			int res = 0;
			bool err = false;
			token = eval(token, (int) EvalPrecedence.MIN_PRECEDENCE, false, ref res, ref err, ref ppToken);
			token = extraTokenCheck(ifAtom, ref ppToken, token);
			if (res == 0 && !err)
				token = CPPelse(true, ref ppToken);

			return token;
		}

		// Handle #ifdef
		int CPPifdef(bool defined, ref PreprocessorToken ppToken)
		{
			int token = scanToken(ref ppToken);
			int name = ppToken.atom;
			if (++ifdepth > MAXIFNESTING) {
				parseContext.Error(ppToken.loc, "maximum nesting depth exceeded", "#ifdef", "");
				return 0;
			}
			elsetracker++;
			if (token != (int) CppEnums.IDENTIFIER) {
				if (defined)
					parseContext.Error(ppToken.loc, "must be followed by macro name", "#ifdef", "");
				else 
					parseContext.Error(ppToken.loc, "must be followed by macro name", "#ifndef", "");
			} else {
				Symbol s = Symbols.LookUp(name);
				token = scanToken(ref ppToken);
				if (token != '\n') {
					parseContext.Error(ppToken.loc, "unexpected tokens following #ifdef directive - expected a newline", "#ifdef", "");
					while (token != '\n')
						token = scanToken(ref ppToken);
				}
				if (((s != null && !s.mac.undef)) != defined)
					token = CPPelse(true, ref ppToken);
			}

			return token;
		}

		// Handle #error
		int CPPerror(ref PreprocessorToken ppToken) 
		{
			int token = scanToken(ref ppToken);
			StringBuilder message = new StringBuilder();
			SourceLocation loc = ppToken.loc;

			while (token != '\n') {
				if (token == (int) CppEnums.INTCONSTANT || token == (int) CppEnums.UINTCONSTANT ||
					token == (int) CppEnums.FLOATCONSTANT || token == (int) CppEnums.DOUBLECONSTANT) {
					message.Append(ppToken.name);
				} else if (token == (int) CppEnums.IDENTIFIER || token == (int) CppEnums.STRCONSTANT) {
					message.Append(Symbols.Atoms.GetAtomString(ppToken.atom));
				} else {
					message.Append(Symbols.Atoms.GetAtomString(token));
				}
				message.Append(" ");
				token = scanToken(ref ppToken);
			}
			parseContext.notifyErrorDirective(loc.line, message.ToString());
			//store this msg into the shader's information log..set the Compile Error flag!!!!
			parseContext.Error(loc, message.ToString(), "#error", "");

			return '\n';
		}

		// Handle #else
		//* Skip forward to appropriate spot.  This is used both
		//** to skip to a #endif after seeing an #else, AND to skip to a #else,
		//** #elif, or #endif after a #if/#ifdef/#ifndef/#elif test was false.
		//
		int CPPelse(bool matchelse,ref PreprocessorToken ppToken)
		{
			int atom;
			int depth = 0;
			int token = scanToken(ref ppToken);

			while (token != BasePreprocessorInput.EOF) {
				if (token != '#') {
					while (token != '\n' && token != BasePreprocessorInput.EOF)
						token = scanToken(ref ppToken);

					if (token == BasePreprocessorInput.EOF)
						return BasePreprocessorInput.EOF;

					token = scanToken(ref ppToken);
					continue;
				}

				if ((token = scanToken(ref ppToken)) != (int) CppEnums.IDENTIFIER)
					continue;

				atom = ppToken.atom;
				if (atom == ifAtom || atom == ifdefAtom || atom == ifndefAtom) {
					depth++; 
					ifdepth++; 
					elsetracker++;
				} else if (atom == endifAtom) {
					token = extraTokenCheck(atom, ref ppToken, scanToken(ref ppToken));
					elseSeen[elsetracker] = false;
					--elsetracker;
					if (depth == 0) {
						// found the #endif we are looking for
						//  ORIGINALLY if (ifdepth)
						if (ifdepth > 0) 
							--ifdepth;
						break;
					}
					--depth;
					--ifdepth;
				} else if (matchelse && depth == 0) {
					if (atom == elseAtom) {
						elseSeen[elsetracker] = true;
						token = extraTokenCheck(atom, ref ppToken, scanToken(ref ppToken));
						// found the #else we are looking for
						break;
					} else if (atom == elifAtom) {
						if (elseSeen[elsetracker])
							parseContext.Error(ppToken.loc, "#elif after #else", "#elif", "");
						/* we decrement ifdepth here, because CPPif will increment
                * it and we really want to leave it alone */
						//if (ifdepth) {
						if (ifdepth > 0)
						{
							--ifdepth;
							elseSeen[elsetracker] = false;
							--elsetracker;
						}

						return CPPif(ref ppToken);
					}
				} else if (atom == elseAtom) {
					if (elseSeen[elsetracker])
						parseContext.Error(ppToken.loc, "#else after #else", "#else", "");
					else
						elseSeen[elsetracker] = true;
					token = extraTokenCheck(atom, ref ppToken, scanToken(ref ppToken));
				} else if (atom == elifAtom) {
					if (elseSeen[elsetracker])
						parseContext.Error(ppToken.loc, "#elif after #else", "#elif", "");
				}
			}

			return token;
		}

		// Handle #extension
		int CPPextension(ref PreprocessorToken ppToken)
		{
			int line = ppToken.loc.line;
			int token = scanToken(ref ppToken);

			if (token=='\n') {
				parseContext.Error(ppToken.loc, "extension name not specified", "#extension", "");
				return token;
			}

			if (token != (int) CppEnums.IDENTIFIER)
				parseContext.Error(ppToken.loc, "extension name expected", "#extension", "");

			string extensionName = Symbols.Atoms.GetAtomString(ppToken.atom);

			token = scanToken(ref ppToken);
			if (token != ':') {
				parseContext.Error(ppToken.loc, "':' missing after extension name", "#extension", "");
				return token;
			}

			token = scanToken(ref ppToken);
			if (token != (int) CppEnums.IDENTIFIER) {
				parseContext.Error(ppToken.loc, "behavior for extension not specified", "#extension", "");
				return token;
			}

			parseContext.updateExtensionBehavior(line, extensionName, Symbols.Atoms.GetAtomString(ppToken.atom));

			token = scanToken(ref ppToken);
			if (token == '\n')
				return token;
			else
				parseContext.Error(ppToken.loc,  "extra tokens -- expected newline", "#extension","");

			return token;
		}

		// Handle #line
		int CPPline(ref PreprocessorToken ppToken) 
		{
			// "#line must have, after macro substitution, one of the following forms:
			// "#line line
			// "#line line source-string-number"

			int token = scanToken(ref ppToken);
			if (token == '\n') {
				parseContext.Error(ppToken.loc, "must by followed by an integral literal", "#line", "");
				return token;
			}

			int lineRes = 0; // Line number after macro expansion.
			int lineToken = 0;
			int fileRes = 0; // Source file number after macro expansion.
			bool hasFile = false;
			bool lineErr = false;
			bool fileErr = false;
			token = eval(token, EvalPrecedence.MIN_PRECEDENCE, false, ref lineRes, ref lineErr, ref ppToken);
			if (! lineErr) {
				lineToken = lineRes;
				if (token == '\n')
					++lineRes;

				// Desktop, pre-version 3.30:  "After processing this directive
				// (including its new-line), the implementation will behave as if it is compiling at line number line+1 and
				// source string number source-string-number."
				//
				// Desktop, version 3.30 and later, and ES:  "After processing this directive
				// (including its new-line), the implementation will behave as if it is compiling at line number line and
				// source string number source-string-number.
				if (parseContext.mProfile == Profile.EsProfile || parseContext.mVersion >= 330)
					--lineRes;
				parseContext.setCurrentLine(lineRes);

				if (token != '\n') {
					token = eval(token, EvalPrecedence.MIN_PRECEDENCE, false, ref fileRes, ref fileErr, ref ppToken);
					if (! fileErr)
						parseContext.setCurrentString(fileRes);
					hasFile = true;
				}
			}
			if (!fileErr && !lineErr) {
				parseContext.notifyLineDirective(lineToken, hasFile, fileRes);
			}
			token = extraTokenCheck(lineAtom,ref ppToken, token);

			return token;
		}

		// Handle #pragma
		int CPPpragma(ref PreprocessorToken ppToken)
		{
			var tokens = new List<string>();

			SourceLocation loc = ppToken.loc;  // because we go to the next line before processing
			int token = scanToken(ref ppToken);
			while (token != '\n' && token != BasePreprocessorInput.EOF) {
				switch (token) {
				case (int) CppEnums.IDENTIFIER:
					tokens.Add(Symbols.Atoms.GetAtomString(ppToken.atom));
					break;
				case (int) CppEnums.INTCONSTANT:
				case (int) CppEnums.UINTCONSTANT:
				case (int) CppEnums.FLOATCONSTANT:
				case (int) CppEnums.DOUBLECONSTANT:
					tokens.Add(ppToken.name);
					break;
				default:
					tokens.Add (Char.ConvertFromUtf32 (token).ToString ());
					break;
				}
				token = scanToken(ref ppToken);
			}

			if (token == BasePreprocessorInput.EOF)
				parseContext.Error(loc, "directive must end with a newline", "#pragma", "");
			else
				parseContext.handlePragma(loc, tokens);

			return token;    
		}

		// Handle #undef
		int CPPundef(ref PreprocessorToken ppToken)
		{
			int token = scanToken(ref ppToken);
			Symbol symb;
			if (token != (int) CppEnums.IDENTIFIER) {
				parseContext.Error(ppToken.loc, "must be followed by macro name", "#undef", "");

				return token;
			}

			string name = Symbols.Atoms.GetAtomString(ppToken.atom); 
			// TODO preprocessor simplification: the token text should have been built into the ppToken during scanToken()
			parseContext.ReservedPpErrorCheck(ppToken.loc, name, "#undef");

			symb = Symbols.LookUp(ppToken.atom);
			if (symb != null) {
				symb.mac.undef = true;
			}
			token = scanToken(ref ppToken);
			if (token != '\n')
				parseContext.Error(ppToken.loc, "can only be followed by a single macro name", "#undef", "");

			return token;
		}

		// Call when there should be no more tokens left on a line.
		int extraTokenCheck(int atom, ref PreprocessorToken ppToken, int token)
		{
			if (token != '\n') {
				const string message = "unexpected tokens following directive";

				string label;
				if (atom == elseAtom)
					label = "#else";
				else if (atom == elifAtom)
					label = "#elif";
				else if (atom == endifAtom)
					label = "#endif";
				else if (atom == ifAtom)
					label = "#if";
				else if (atom == lineAtom)
					label = "#line";
				else
					label = "";

				if ((parseContext.messages & MessageType.RelaxedErrors) > 0)
					parseContext.Warn(ppToken.loc, message, label, "");
				else
					parseContext.Error(ppToken.loc, message, label, "");

				while (token != '\n')
					token = scanToken(ref ppToken);
			}

			return token;
		}

		// #version: This is just for error checking: the version and profile are decided before preprocessing starts
		int CPPversion(ref PreprocessorToken ppToken)
		{
			int token = scanToken(ref ppToken);

			if (errorOnVersion || versionSeen)
				parseContext.Error(ppToken.loc, "must occur first in shader", "#version", "");
			versionSeen = true;

			if (token == '\n') {
				parseContext.Error(ppToken.loc, "must be followed by version number", "#version", "");

				return token;
			}

			if (token != (int) CppEnums.INTCONSTANT)
				parseContext.Error(ppToken.loc, "must be followed by version number", "#version", "");

			ppToken.ival = int.Parse(ppToken.name);
			int versionNumber = ppToken.ival;
			int line = ppToken.loc.line;
			token = scanToken(ref ppToken);

			if (token == '\n') {
				parseContext.notifyVersion(line, versionNumber, null);
				return token;
			} else {
				if (ppToken.atom != coreAtom &&
					ppToken.atom != compatibilityAtom &&
					ppToken.atom != esAtom)
					parseContext.Error(ppToken.loc, "bad profile name; use es, core, or compatibility", "#version", "");
				parseContext.notifyVersion(line, versionNumber, Symbols.Atoms.GetAtomString(ppToken.atom));
				token = scanToken(ref ppToken);

				if (token == '\n')
					return token;
				else
					parseContext.Error(ppToken.loc, "bad tokens following profile -- expected newline", "#version", "");
			}

			return token;
		}

		int op_logor(int a, int b) { return (a > 0 || b > 0) ? 1 : 0; }
		int op_logand(int a, int b) { return (a > 0 && b > 0) ? 1 : 0; }
		int op_or(int a, int b) { return a | b; }
		int op_xor(int a, int b) { return a ^ b; }
		int op_and(int a, int b) { return a & b; }
		int op_eq(int a, int b) { return a == b ? 1 : 0; }
		int op_ne(int a, int b) { return a != b ? 1 : 0; }
		int op_ge(int a, int b) { return a >= b  ? 1 : 0; }
		int op_le(int a, int b) { return a <= b  ? 1 : 0; }
		int op_gt(int a, int b) { return a > b  ? 1 : 0; }
		int op_lt(int a, int b) { return a < b  ? 1 : 0; }
		int op_shl(int a, int b) { return a << b; }
		int op_shr(int a, int b) { return a >> b; }
		int op_add(int a, int b) { return a + b; }
		int op_sub(int a, int b) { return a - b; }
		int op_mul(int a, int b) { return a * b; }
		int op_div(int a, int b) { return a / b; }
		int op_mod(int a, int b) { return a % b; }
		int op_pos(int a) { return a; }
		int op_neg(int a) { return -a; }
		int op_cmpl(int a) { return ~a; }
		int op_not(int a) { return (a == 0) ? 1 : 0; }

		private class UnaryEvalOperation
		{
			public char token;
			public Func<int, int> op;
		}



		int eval(int token, EvalPrecedence precedence, bool shortCircuit, ref int res, ref bool err, ref PreprocessorToken ppToken)
		{
			UnaryEvalOperation[] unop =   {
				new UnaryEvalOperation{ token='+', op= op_pos },
				new UnaryEvalOperation{ token='-', op= op_neg },
				new UnaryEvalOperation{ token='~', op= op_cmpl},
				new UnaryEvalOperation{ token='!', op= op_not },
			};


			SourceLocation loc = ppToken.loc;  // because we sometimes read the newline before reporting the error
			if (token == (int) CppEnums.IDENTIFIER) {
				if (ppToken.atom == definedAtom) {
					bool needclose = false;
					token = scanToken(ref ppToken);
					if (token == '(') {
						needclose = true;
						token = scanToken(ref ppToken);
					}
					if (token != (int) CppEnums.IDENTIFIER) {
						parseContext.Error(loc, "incorrect directive, expected identifier", "preprocessor evaluation", "");
						err = true;
						res = 0;

						return token;
					}
					Symbol s = Symbols.LookUp(ppToken.atom);
										// !s.mac.undef
					res = (s != null) ? (s.mac.undef ? 0 : 1) : 0;
					token = scanToken(ref ppToken);
					if (needclose) {
						if (token != ')') {
							parseContext.Error(loc, "expected ')'", "preprocessor evaluation", "");
							err = true;
							res = 0;

							return token;
						}
						token = scanToken(ref ppToken);
					}
				} else {
					token = evalToToken(token, shortCircuit, ref res, ref err, ref ppToken);
					return eval(token, precedence, shortCircuit, ref res, ref err, ref ppToken);
				}
			} else if (token == (int) CppEnums.INTCONSTANT) {
				res = ppToken.ival;
				token = scanToken(ref ppToken);
			} else if (token == '(') {
				token = scanToken(ref ppToken);
				token = eval(token, EvalPrecedence.MIN_PRECEDENCE, shortCircuit, ref res, ref err, ref ppToken);
				if (! err) {
					if (token != ')') {
						parseContext.Error(loc, "expected ')'", "preprocessor evaluation", "");
						err = true;
						res = 0;

						return token;
					}
					token = scanToken(ref ppToken);
				}
			} else {
				int op;
				for (op = unop.Length - 1; op >= 0; op--) {
					if (unop[op].token == token)
						break;
				}
				if (op >= 0) {
					token = scanToken(ref ppToken);
					token = eval(token, EvalPrecedence.UNARY, shortCircuit, ref res, ref err, ref ppToken);
					res = unop[op].op(res);
				} else {
					parseContext.Error(loc, "bad expression", "preprocessor evaluation", "");
					err = true;
					res = 0;

					return token;
				}
			}

			token = evalToToken(token, shortCircuit, ref res, ref err, ref ppToken);

			// Perform evaluation of binary operation, if there is one, otherwise we are done.
			while (! err) {
				if (token == ')' || token == '\n') 
					break;
				int op;
				for (op = binop.Count - 1; op >= 0; op--) {
					if (binop[op].token == token)
						break;
				}
				if (op < 0 || binop[op].precedence <= precedence)
					break;
				int leftSide = res;

				// Setup short-circuiting, needed for ES, unless already in a short circuit.
				// (Once in a short-circuit, can't turn off again, until that whole subexpression is done.
				if (! shortCircuit) {
					if ((token == (int) CppEnums.OR_OP  && leftSide == 1) ||
						(token == (int) CppEnums.AND_OP && leftSide == 0))
						shortCircuit = true;
				}

				token = scanToken(ref ppToken);
				token = eval(token, binop[op].precedence, shortCircuit, ref res, ref err, ref ppToken);
				res = binop[op].op(leftSide, res);
			}

			return token;
		}

		// Expand macros, skipping empty expansions, to get to the first real token in those expansions.
		int evalToToken(int token, bool shortCircuit, ref int res, ref bool err, ref PreprocessorToken ppToken)
		{
			bool escapedLoop = false;
			while (token == (int) CppEnums.IDENTIFIER && ppToken.atom != definedAtom) {
				int macroReturn = MacroExpand(ppToken.atom, ppToken, true, false);
				if (macroReturn == 0) {
					parseContext.Error(ppToken.loc, "can't evaluate expression", "preprocessor evaluation", "");
					err = true;
					res = 0;
					token = scanToken(ref ppToken);
					break;
				}
				if (macroReturn == -1) {
					if (! shortCircuit && parseContext.mProfile == Profile.EsProfile) {
						const string message = "undefined macro in expression not allowed in es profile";
						string name = Symbols.Atoms.GetAtomString(ppToken.atom);
						if ((parseContext.messages & MessageType.RelaxedErrors) > 0)
							parseContext.Warn(ppToken.loc, message, "preprocessor evaluation", name);
						else
							parseContext.Error(ppToken.loc, message, "preprocessor evaluation", name);
					}
				}
				token = scanToken(ref ppToken);
			}

			return token;
		}

		// Checks if we've seen balanced #if...#endif
		internal void missingEndifCheck()
		{
			if (ifdepth > 0)
				parseContext.Error(parseContext.getCurrentLoc(), "missing #endif", "", "");
		}

		internal void pushTokenStreamInput(TokenStream ts)
		{
			pushInput(new TokenInput(this, ts));
			ts.Rewind();
		}

		void UngetToken(int token, PreprocessorToken ppToken)
		{
			pushInput(new UngotTokenInput(this, token, ppToken));
		}

		///*
		//* Read the next token from a token stream (not the source stream, but stream used to hold a tokenized macro).
		//*/
		internal int ReadToken(TokenStream pTok, PreprocessorToken ppToken)
		{
			char[] tokenText = buffer.tokenText;
			int ltoken = 0;
			int len = 0;
			int ch;

			ltoken = pTok.lReadByte();
			ppToken.loc = parseContext.getCurrentLoc();
			if (ltoken > 127)
				ltoken += 128;
			switch (ltoken) 
			{
				case '#':        
					if (pTok.lReadByte() == '#') {
						parseContext.requireProfile(ppToken.loc, ~Profile.EsProfile, "token pasting (##)");
						parseContext.ProfileRequires(ppToken.loc, ~Profile.EsProfile, 130, null, "token pasting (##)");
						parseContext.Error(ppToken.loc, "token pasting not implemented (internal error)", "##", "");
						//return CPP_TOKEN_PASTE;
						return ReadToken(pTok, ppToken);
					} else
						pTok.lUnreadByte();
					break;
				case (int) CppEnums.STRCONSTANT:
				case (int) CppEnums.IDENTIFIER:
				case (int) CppEnums.FLOATCONSTANT:
				case (int) CppEnums.DOUBLECONSTANT:
				case (int) CppEnums.INTCONSTANT:
				case (int) CppEnums.UINTCONSTANT:
					len = 0;
				ch = pTok.lReadByte ();
					while (ch != 0)
					{
						if (len < PreprocessorToken.maxTokenLength)
						{
							tokenText [len] = (char)ch;
							len++;
						ch = pTok.lReadByte();
						} else
						{
							parseContext.Error (ppToken.loc, "token too long", "", "");
							break;
						}
					}
					break;
				default:
					break;
			}

			// DY RESTRUCTURED CODE 
			//tokenText[len] = 0;

			string text = new string (tokenText, 0, len);
			switch (ltoken) 
			{
				case (int) CppEnums.IDENTIFIER:
				case (int) CppEnums.STRCONSTANT:
					ppToken.atom = Symbols.Atoms.LookUpAddString(text);
					break;
				case (int) CppEnums.FLOATCONSTANT:
				case (int) CppEnums.DOUBLECONSTANT:
					ppToken.name = text;
					ppToken.dval = float.Parse(ppToken.name);
					break;
				case (int) CppEnums.INTCONSTANT:
				case (int) CppEnums.UINTCONSTANT:
					ppToken.name = text;
					len = text.Length;
					if (len > 0 && tokenText[0] == '0') {
						if (len > 1 && (tokenText[1] == 'x' || tokenText[1] == 'X'))
							ppToken.ival = Convert.ToInt32(ppToken.name, 16);
						else
							ppToken.ival = Convert.ToInt32(ppToken.name, 8);
					} else
						ppToken.ival = int.Parse(ppToken.name);
					break;
				default:
					break;
			}

			return ltoken;
		}

		private int MacroExpand(int atom, PreprocessorToken ppToken, bool expandUndef, bool newLineOkay)
		{
			Symbol sym = Symbols.LookUp(atom);
			int token;
			int depth = 0;

			ppToken.space = false;
			if (atom == __LINE__Atom) {
				ppToken.ival = parseContext.getCurrentLoc().line;
				ppToken.name = ppToken.ival.ToString();
				UngetToken ((int)CppEnums.INTCONSTANT, ppToken);

				return 1;
			}

			if (atom == __FILE__Atom) {
				ppToken.ival = parseContext.getCurrentLoc().stringBias;
				ppToken.name = ppToken.ival.ToString ();
				UngetToken ((int)CppEnums.INTCONSTANT, ppToken);

				return 1;
			}

			if (atom == __VERSION__Atom) {
				ppToken.ival = parseContext.mVersion;
				ppToken.name = ppToken.ival.ToString ();
				UngetToken ((int)CppEnums.INTCONSTANT, ppToken);

				return 1;
			}

			// no recursive expansions
			if (sym != null && sym.mac.busy)
				return 0;

			// not expanding undefined macros
			if ((sym == null || sym.mac.undef) && ! expandUndef)
				return 0;

			// 0 is the value of an undefined macro
			if ((sym == null || sym.mac.undef) && expandUndef) {
				pushInput(new ZeroInput(this));
				return -1;
			}

			MacroInput inp = new MacroInput(this);

			SourceLocation loc = ppToken.loc;  // in case we go to the next line before discovering the error
			inp.mac = sym.mac;
			if (sym.mac.args != null && sym.mac.args.Length > 0) {
				token = scanToken(ref ppToken);
				if (newLineOkay) {
					while (token == '\n')                
						token = scanToken(ref ppToken);
				}
				if (token != '(') {
					parseContext.Error(loc, "expected '(' following", "macro expansion", Symbols.Atoms.GetAtomString(atom));
					UngetToken(token, ppToken);
					ppToken.atom = atom;

					inp = null;
					return 0;
				}
				inp.args.Clear ();
				for (int i = 0; i < inp.mac.argc; i++)
					inp.args.Add (new TokenStream ());
				int arg = 0;
				bool tokenRecorded = false;
				do {
					depth = 0;
					while (true) {
						token = scanToken(ref ppToken);
						if (token == BasePreprocessorInput.EOF) {
							parseContext.Error(loc, "EOF in macro", "macro expansion", Symbols.Atoms.GetAtomString(atom));
							inp = null;
							return 0;
						}
						if (token == '\n') {
							if (! newLineOkay) {
								parseContext.Error(loc, "end of line in macro substitution:", "macro expansion", Symbols.Atoms.GetAtomString(atom));
								inp = null;
								return 0;
							}
							continue;
						}
						if (token == '#') {
							parseContext.Error(ppToken.loc, "unexpected '#'", "macro expansion", Symbols.Atoms.GetAtomString(atom));
							inp = null;
							return 0;
						}
						if (inp.mac.argc == 0 && token != ')')
							break;
						if (depth == 0 && (token == ',' || token == ')'))
							break;
						if (token == '(')
							depth++;
						if (token == ')')
							depth--;
						Symbols.Atoms.RecordToken(inp.args[arg], token, ppToken);
						tokenRecorded = true;
					}
					if (token == ')') {
						if (inp.mac.argc == 1 && !tokenRecorded)
							break;
						arg++;
						break;
					}
					arg++;
				} while (arg < inp.mac.argc);

				if (arg < inp.mac.argc)
					parseContext.Error(loc, "Too few args in Macro", "macro expansion", Symbols.Atoms.GetAtomString(atom));
				else if (token != ')') {
					depth=0;
					while (token != BasePreprocessorInput.EOF && (depth > 0 || token != ')')) {
						if (token == ')')
							depth--;
						token = scanToken(ref ppToken);
						if (token == '(')
							depth++;
					}

					if (token == BasePreprocessorInput.EOF) {
						parseContext.Error(loc, "EOF in macro", "macro expansion", Symbols.Atoms.GetAtomString(atom));
						inp = null;
						return 0;
					}
					parseContext.Error(loc, "Too many args in macro", "macro expansion", Symbols.Atoms.GetAtomString(atom));
				}
				for (int i = 0; i < inp.mac.argc; i++)
				{
					inp.args [i] = PrescanMacroArg (inp.args[i], ppToken, newLineOkay);
				}
			}

			pushInput(inp);
			sym.mac.busy = true;
			sym.mac.body.Rewind();

			return 1;
		}

		internal TokenStream PrescanMacroArg(TokenStream a, PreprocessorToken ppToken, bool newLineOkay)
		{
			int token;
			a.Rewind();
			do {
				token = ReadToken(a, ppToken);
				if (token == (int) CppEnums.IDENTIFIER && Symbols.LookUp(ppToken.atom) != null)
					break;
			} while (token != BasePreprocessorInput.END_OF_INPUT);

			if (token == BasePreprocessorInput.END_OF_INPUT)
				return a;

			TokenStream n = new TokenStream ();
			pushInput(new MarkerInput(this));
			pushTokenStreamInput(a);
			while ((token = scanToken(ref ppToken)) != MarkerInput.marker) {
				if (token == (int) CppEnums.IDENTIFIER && MacroExpand(ppToken.atom, ppToken, false, newLineOkay) != 0)
					continue;
				Symbols.Atoms.RecordToken(n, token, ppToken);
			}
			popInput();

			return n;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////////// Floating point constants: /////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////////

		/*

		*/

		/// <summary>
		///  Scan a single- or double-precision floating point constant.  Assumes that the scanner
		/// has seen at least one digit, followed by either a decimal '.' or the
		/// letter 'e', or a precision ending (e.g., F or LF).  
		/// </summary>
		/// <returns>The float const.</returns>
		/// <param name="len">Length.</param>
		/// <param name="ch">Ch.</param>
		/// <param name="ppToken">Pp token.</param>
		internal int lFloatConst(StringInputBuffer buffer, int len, int ch, PreprocessorToken ppToken)
		{
			bool HasDecimalOrExponent = false;
			int declen, exp, ExpSign;
			int str_len;
			bool isDouble = false;

			declen = 0;
			exp = 0;

			const int MAX_TOKEN_LENGTH = 1024;

			str_len=len;
			char[] str =  buffer.name;
			if (ch == '.') {
				HasDecimalOrExponent = true;
				str[len++] = (char)ch;
				ch = getChar();
				while (ch >= '0' && ch <= '9')
				{
					if (len < MAX_TOKEN_LENGTH)
					{
						declen++;
						if (len > 0 || ch != '0') {
							str[len] = (char)ch;
							len++;
							str_len++;
						}
						ch = getChar();
					} 
					else 
					{
						parseContext.Error( ppToken.loc, "float literal too long", "", "");
						len = 1;
						str_len = 1;
					}
				}
			}

			// Exponent:

			if (ch == 'e' || ch == 'E')
			{
				HasDecimalOrExponent = true;
				if (len >= MAX_TOKEN_LENGTH) 
				{
					parseContext.Error( ppToken.loc, "float literal too long", "", "");
					len = 1;
					str_len=1;
				} 
				else 
				{
					ExpSign = 1;
					str[len++] = (char)ch;
					ch = getChar();
					if (ch == '+')
					{
						str[len++] = (char)ch;
						ch = getChar();
					} 
					else if (ch == '-')
					{
						ExpSign = -1;
						str[len++] = (char)ch;
						ch = getChar();
					}
					if (ch >= '0' && ch <= '9')
					{
						while (ch >= '0' && ch <= '9')
						{
							if (len < MAX_TOKEN_LENGTH)
							{
								exp = exp*10 + ch - '0';
								str[len++] = (char)ch;
								ch = getChar();
							} 
							else
							{
								parseContext.Error( ppToken.loc, "float literal too long", "", "");
								len = 1;
								str_len=1;
							}
						}
					}
					else
					{
						parseContext.Error( ppToken.loc, "bad character in float exponent", "", "");
					}
					exp *= ExpSign;
				}
			}

			if (len == 0)
			{
				ppToken.dval = 0.0;
				str = "0.0".ToCharArray();
			}
			else
			{
				if (ch == 'l' || ch == 'L')
				{
					parseContext.doubleCheck( ppToken.loc, "double floating-point suffix");
					if (! HasDecimalOrExponent)
						parseContext.Error( ppToken.loc, "float literal needs a decimal point or exponent", "", "");
					int ch2 = getChar();
					if (ch2 != 'f' && ch2 != 'F')
					{
						ungetChar();
						ungetChar();
					}
					else
					{
						if (len < MAX_TOKEN_LENGTH)
						{
							str[len++] = (char)ch;
							str[len++] = (char)ch2;
							isDouble = true;
						}
						else
						{
							parseContext.Error( ppToken.loc, "float literal too long", "", "");
							len = 1;
							str_len=1;
						}
					}
				}
				else if (ch == 'f' || ch == 'F')
				{
					parseContext.ProfileRequires( ppToken.loc,  Profile.EsProfile, 300, null, "floating-point suffix");
					if ((parseContext.messages &  MessageType.RelaxedErrors) == 0)
						parseContext.ProfileRequires (ppToken.loc, (Profile)~Profile.EsProfile, 120, null, "floating-point suffix");
					if (! HasDecimalOrExponent)
						parseContext.Error( ppToken.loc, "float literal needs a decimal point or exponent", "", "");
					if (len < MAX_TOKEN_LENGTH)
						str[len++] = (char)ch;
					else
					{
						parseContext.Error( ppToken.loc, "float literal too long", "", "");
						len = 1;
						str_len=1;
					}
				} else 
					ungetChar();

				//str[len]='\0';

				ppToken.dval = Double.Parse(new string(str, 0, len));
			}

			if (isDouble)
				return (int) CppEnums.DOUBLECONSTANT;
			else
				return (int) CppEnums.FLOATCONSTANT;
		}
	}
}

