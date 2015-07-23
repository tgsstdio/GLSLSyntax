using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GLSLSyntaxAST.CodeDom
{
	public class tMacroInput : tInput
	{
		public tMacroInput (IPreprocessorContext pp) : base(pp)
		{
		}

		public override int scan(TPpToken ppToken)
		{
			int token;
			do {
				token = pp.ReadToken(mac.body, ppToken);
			} while (token == ' ');  // handle white space in macro
			// TODO : maybe fixed this original GL issue
			// TODO: preprocessor:  properly handle whitespace (or lack of it) between tokens when expanding
			if (token == CppEnums.IDENTIFIER) {
				int i;
				for (i = mac.argc - 1; i >= 0; i--)
					if (mac.args[i] == ppToken->atom) 
						break;
				if (i >= 0) {
					pp.pushTokenStreamInput(args[i]);

					return pp.scanToken(ppToken);
				}
			}

			if (token == tInput.END_OF_INPUT)
				mac.busy = 0;

			return token;
		}

		public override int getch() 
		{ 
			Debug.Assert(0); 
			return tInput.END_OF_INPUT; 

			throw new NotImplementedException ();
		}

		public override void ungetch() 
		{ 
			Debug.Assert(0); 

			throw new NotImplementedException ();
		}

		MacroSymbol mac;
		public List<TokenStream> args;
	}
}


