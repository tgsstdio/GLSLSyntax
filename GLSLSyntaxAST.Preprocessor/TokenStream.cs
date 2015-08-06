using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.Preprocessor
{
	public class TokenStream 
	{
		public List<UInt16> data;
		public int current;

		public TokenStream ()
		{
			data = new List<ushort> ();
		}

		/// <summary>
		/// Reset a token stream in preperation for reading.
		/// </summary>
		internal void Rewind()
		{
			current = 0;
		}		

		/// <summary>
		/// Get the next byte from a stream.
		/// </summary>
		/// <returns>The read byte.</returns>
		internal int lReadByte()
		{
			if (current < data.Count)
				return data[current++];
			else
				return BasePreprocessorInput.END_OF_INPUT;
		}

		internal void lAddByte(UInt16 fVal)
		{
			data.Add(fVal);
		}

		internal void lUnreadByte()
		{
			if (current > 0)
				--current;
		}
	};
}

