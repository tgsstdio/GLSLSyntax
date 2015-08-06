using System.Collections.Generic;
using System;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class AtomLookup
	{
		private readonly Dictionary<string, int> mAtomMap;
		private readonly Dictionary<int, string> mStringMap;
		internal AtomLookup ()
		{
			mAtomMap = new Dictionary<string, int> ();
			mStringMap = new Dictionary<int, string> ();
			NextAtom = (int) CppEnums.FIRST_USER_TOKEN_SY;
		}

		/// <summary>
		/// Add forced mapping of string to atom.
		/// </summary>
		/// <returns>The fixed atom.</returns>
		/// <param name="s">S.</param>
		/// <param name="atom">Atom.</param>
		internal int AddFixedAtom(string s, int atom)
		{
			mAtomMap.Add (s, atom);
			mStringMap.Add (atom, s);
			return atom;
		}

		internal int NextAtom { get; private set; }

		/// <summary>
		/// Map a new or existing string to an atom, inventing a new atom if necessary.
		/// </summary>
		/// <returns>The up add string.</returns>
		/// <param name="s">S.</param>
		internal int LookUpAddString(string s)
		{
			int result;
			if (mAtomMap.TryGetValue (s, out result))
			{
				return result;
			} 
			else
			{
				return AddFixedAtom (s, NextAtom++);
			}
		}

		internal int GetAtomKey(string key)
		{
			return mAtomMap [key];
		}

		/// <summary>
		/// Map an already created atom to its string.
		/// </summary>
		/// <returns>The atom string.</returns>
		/// <param name="atom">Atom.</param>
		internal string GetAtomString(int atom)
		{
			if (atom == 0)
				return "<null atom>";
			if (atom < 0)
				return "<EOF>";

			string result;
			if (mStringMap.TryGetValue (atom, out result))
			{
				return result;
			}
			else
			{
				return "<invalid atom>";
			}
		}

		/// <summary>
		/// Add a token to the end of a list for later playback.
		/// </summary>
		/// <param name="pTok">P tok.</param>
		/// <param name="token">Token.</param>
		/// <param name="ppToken">Pp token.</param>
		internal void RecordToken(TokenStream pTok, int token, PreprocessorToken ppToken)
		{
			if (token > 256)
				pTok.lAddByte( (UInt16)((token & 0x7f) + 0x80));
			else
				pTok.lAddByte( (UInt16)(token & 0x7f));

			switch (token) {
			case (int) CppEnums.IDENTIFIER:
			case (int) CppEnums.STRCONSTANT:
				string s = GetAtomString(ppToken.atom);
				foreach(var letter in s)
				{
					pTok.lAddByte((UInt16) letter);
				}
				pTok.lAddByte(0);
				break;
			case (int) CppEnums.INTCONSTANT:
			case (int) CppEnums.UINTCONSTANT:
			case (int) CppEnums.FLOATCONSTANT:
			case (int) CppEnums.DOUBLECONSTANT:
				string str = ppToken.name;
				foreach (var letter in str)
				{
					pTok.lAddByte((UInt16) letter);
				}
				pTok.lAddByte(0);
				break;
			default:
				break;
			}
		}
	}
}

