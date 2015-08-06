using System;
using System.IO;

namespace GLSLSyntaxAST.Preprocessor
{
	public class Standalone
	{
		private readonly InfoSink mInfoSink;
		private GLSLIntermediate mIntermediate;
		private SymbolLookup mSymbols;
		public Standalone (InfoSink infoSink, GLSLIntermediate intermediate, SymbolLookup symbols)
		{
			mInfoSink = infoSink;
			mIntermediate = intermediate;
			mSymbols = symbols;
		}

		/// <summary>
		/// Finds the language.
		/// Deduce the language from the filename.  Files must end in one of the
		/// following extensions:
		/// 
		///  .vert = vertex
		///  .tesc = tessellation control
		///   .tese = tessellation evaluation
		///   .geom = geometry
		///   .frag = fragment
		///   .comp = compute
		/// </summary>
		/// <returns>The language.</returns>
		/// <param name="fileName">File name.</param>
		public static ShaderLanguage FindLanguage(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return ShaderLanguage.Vertex;
			}

			string suffix = Path.GetExtension(fileName);
			if (suffix == ".vert")
				return ShaderLanguage.Vertex;
			else if (suffix == ".tesc")
				return ShaderLanguage.TessControl;
			else if (suffix == ".tese")
				return ShaderLanguage.TessEvaluation;
			else if (suffix == ".geom")
				return ShaderLanguage.Geometry;
			else if (suffix == ".frag")
				return ShaderLanguage.Fragment;
			else if (suffix == ".comp")
				return ShaderLanguage.Compute;

			return ShaderLanguage.Vertex;
		}

		[Flags]
		public enum TOptions : int
		{
			None               = 0x0000,
			Intermediate       = 0x0001,
			SuppressInfolog    = 0x0002,
			MemoryLeakMode     = 0x0004,
			RelaxedErrors      = 0x0008,
			GiveWarnings       = 0x0010,
			LinkProgram        = 0x0020,
			MultiThreaded      = 0x0040,
			DumpConfig         = 0x0080,
			DumpReflection     = 0x0100,
			SuppressWarnings   = 0x0200,
			DumpVersions       = 0x0400,
			Spirv                = 0x0800,
			HumanReadableSpirv   = 0x1000,
			VulkanRules        = 0x2000,
			DefaultDesktop     = 0x4000,
			OutputPreprocessed = 0x8000,
		};

		public int Options = 0;

		/// <summary>
		/// Translate the meaningful subset of command-line options to parser-behavior options.
		/// </summary>
		/// <returns>The message options.</returns>
		/// <param name="messages">Messages.</param>
		private MessageType SetMessageOptions(MessageType messages)
		{
			if ((Options & (int) TOptions.RelaxedErrors) > 0)
				messages = messages | MessageType.RelaxedErrors;
			if ((Options & (int) TOptions.Intermediate) > 0)
				messages = messages | MessageType.AST;
			if ((Options & (int) TOptions.SuppressWarnings) > 0)
				messages = messages | MessageType.SuppressWarnings;
			if ((Options & (int) TOptions.Spirv) > 0)
				messages = messages | MessageType.SPIRVRules;
			if ((Options & (int) TOptions.VulkanRules) > 0)
				messages = messages | MessageType.VulkanRules;
			if ((Options & (int) TOptions.OutputPreprocessed) > 0)
				messages = messages | MessageType.OnlyPreprocessor;
			return messages;
		}

		public bool Preprocess (ShaderLanguage stage, string[] shaderStrings, out string result)
		{
			var messageType = SetMessageOptions(MessageType.Default);
			var shader = new TShader(mInfoSink, stage, mIntermediate, mSymbols);
			shader.setStrings (shaderStrings);
			int defaultVersion = (Options & (int) TOptions.DefaultDesktop) > 0 ? 110: 100;
			return shader.preprocess (defaultVersion, Profile.NoProfile, false, false, messageType, out result);
		}

		public bool Run(string fileName, out string result)
		{
			ShaderLanguage stage = FindLanguage(fileName);

			using(var fs = File.OpenRead(fileName))
			{
				return Run (fs, stage, out result);
			}
		}

		public bool Run(Stream fs, ShaderLanguage stage, out string result)
		{
			string shaderStrings = null;
			using(var sr = new StreamReader(fs))
			{
				//	shaderStrings = fs
				shaderStrings = sr.ReadToEnd();
			}

			return Preprocess (stage, new string[]{shaderStrings}, out result);			
		}

	}
}

