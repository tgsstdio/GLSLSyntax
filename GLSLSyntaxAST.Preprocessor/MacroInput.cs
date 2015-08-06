using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class MacroInput : BasePreprocessorInput
	{
		internal MacroInput (PreprocessorContext pp) : base(pp)
		{
		}

		internal override int scan(ref PreprocessorToken ppToken)
		{
			int token;
			do {
				token = pp.ReadToken(mac.body, ppToken);
			} while (token == ' ');  // handle white space in macro
			// TODO : maybe fixed this original GL issue
			// TODO: preprocessor:  properly handle whitespace (or lack of it) between tokens when expanding
			if (token == (int) CppEnums.IDENTIFIER) {
				int i;
				for (i = mac.argc - 1; i >= 0; i--)
					if (mac.args[i] == ppToken.atom) 
						break;
				if (i >= 0) {
					pp.pushTokenStreamInput(args[i]);

					return pp.scanToken(ref ppToken);
				}
			}

			if (token == BasePreprocessorInput.END_OF_INPUT)
				mac.busy = false;

			return token;
		}

		internal override int getch() 
		{ 
			throw new NotSupportedException ();
		}

		internal override void ungetch() 
		{ 
			throw new NotSupportedException ();
		}

		internal MacroSymbol mac;
		internal List<TokenStream> args;
	}
}


