﻿using System;
using System.Diagnostics;

namespace GLSLSyntaxAST.CodeDom
{
	public class tTokenInput : tInput {
		public tTokenInput(PreprocessorContext pp, TokenStream t) : base(pp)
		{
			tokens = t;
		}

		public override int scan(ref TPpToken ppToken)
		{
			return pp.ReadToken(tokens, ppToken);
		}

		public override int getch() 
		{
			throw new NotSupportedException ();
		}

		public override void ungetch()
		{
			throw new NotSupportedException ();
		}

		protected TokenStream tokens;
	}
}

