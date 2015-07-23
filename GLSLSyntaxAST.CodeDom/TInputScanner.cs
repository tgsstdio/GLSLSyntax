using System;

namespace GLSLSyntaxAST.CodeDom
{
	public class TInputScanner
	{
		public TInputScanner(int n, string[] s, int[] L, int b = 0, int f = 0)
		{
			numSources = n;
			sources = s;
			lengths = L;
			currentSource = 0;
			currentChar = 0;
			stringBias = b;
			finale = f;

			// loc[0]
			loc = new TSourceLoc[numSources];
			loc[currentSource].stringBias = -stringBias;
			loc[currentSource].line = 1;
			loc[currentSource].column = 0;
		}

		public int get()
		{
			return 0;
		}

		public void unget()
		{
			
		}

		public int peek()
		{
			if (currentSource >= numSources)
				return -1;

			return sources[currentSource][currentChar];
		}

		public void setLine(int newLine)
		{
			loc[currentSource].line = newLine; 
		}

		public void setString(int newString)
		{
			loc[currentSource].stringBias = newString; 
		}

		int numSources;             // number of strings in source
		string [] sources; // array of strings
		int[] lengths;      // length of each string
		int currentSource;
		int currentChar;

		// This is for reporting what string/line an error occurred on, and can be overridden by #line.
		// It remembers the last state of each source string as it is left for the next one, so unget() 
		// can restore that state.
		TSourceLoc[] loc;  // an array

		int stringBias;   // the first string that is the user's string number 0
		int finale;       // number of internal strings after user's last string

		public TSourceLoc getSourceLoc() { 
			return loc[Math.Max(0, Math.Min(currentSource, numSources - finale - 1))]; 
		}
	}
}

