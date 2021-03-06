﻿using System;
using GLSLSyntaxAST.CodeDom;
using System.IO;

namespace GLSLSyntax.Preprocessor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var infoSink = new InfoSink {debug = new InfoSinkBase(), info = new InfoSinkBase()};
			var intermediate = new GLSLIntermediate ();
			var preprocessor = new Standalone (infoSink, intermediate);
			string result = null;

			//const string fileName = "Sample.vert";
			const string fileName = "notex.vert";
			string original = null;
			using (var fs = File.OpenRead (fileName))
			using (var sr = new StreamReader(fs))				
			{
				original = sr.ReadToEnd ();
			}

			Console.WriteLine (original);
			Console.WriteLine (original.Length);

			if (preprocessor.Run (fileName, out result))
			{
				Console.WriteLine (result);
				Console.WriteLine (result.Length);
			}

		}
	}
}
