using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class TShader
	{
		private readonly TInfoSink infoSink;
		private readonly TDeferredCompiler compiler;
		private EShLanguage stage;
		private TIntermediate intermediate;
		public TShader (EShLanguage s, TInfoSink sink, TDeferredCompiler c, TIntermediate i)
		{
			stage = s;
			infoSink = sink;
			compiler = c;
			intermediate = i;
		}

		public string[] strings;
		public int numStrings;
		public void setStrings(string s, int n)
		{
			strings = new string[]{s};
			numStrings = n;
			//lengths = nullptr;
		}

		public string preamble = null;
		public bool preprocess (
			TBuiltInResource builtInResources,
			int defaultVersion,
			Profile defaultProfile,
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,
			EShMessages message, out string output_string)
		{
			if (preamble == null)
				preamble = "";

			return PreprocessDeferred(compiler, strings, numStrings,
				null, preamble, EShOptimizationLevel.EShOptNone, builtInResources,
				defaultVersion, defaultProfile, forceDefaultVersionAndProfile, forwardCompatible, message,
				out intermediate, out output_string);
		}

		// Take a single compilation unit, and run the preprocessor on it.
		// Return: True if there were no issues found in preprocessing,
		//         False if during preprocessing any unknown version, pragmas or
		//         extensions were found.
		bool PreprocessDeferred(
			TDeferredCompiler compiler,
			string[] shaderStrings,
			int numStrings,
			int[] inputLengths,
			string preamble,
			EShOptimizationLevel optLevel,
			TBuiltInResource resources,
			int defaultVersion,         // use 100 for ES environment, 110 for desktop
			Profile defaultProfile,
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,     // give errors for use of deprecated features
			EShMessages messages,       // warnings/errors/AST; things to print out
			out TIntermediate intermediate, // returned tree, etc.
			out string outputString)
		{
			var parser = new DoPreprocessing();
			bool result = ProcessDeferred(compiler, shaderStrings, numStrings, inputLengths,
				preamble, optLevel, resources, defaultVersion, defaultProfile, forceDefaultVersionAndProfile,
				forwardCompatible, messages, out intermediate, parser.DoStuff, false);
			outputString = parser.Output;
			return result;
		}

		static string ProfileName(Profile profile)
		{
			switch (profile) {
			case Profile.NoProfile:            
				return "none";
			case Profile.CoreProfile:         
				return "core";
			case Profile.CompatibilityProfile:  
				return "compatibility";
			case Profile.EsProfile:             
				return "es";
			default:                     
				return "unknown profile";
			}
		}

		/// <summary>
		/// This is the common setup and cleanup code for PreprocessDeferred and
		/// CompileDeferred.
		/// It takes any callable with a signature of
		/// bool (TParseContext& parseContext, TPpContext& ppContext,
		///               TInputScanner& input, bool versionWillBeError,
		///               TSymbolTable& , TIntermediate& ,
		///               EShOptimizationLevel , EShMessages );
		/// Which returns false if a failure was detected and true otherwise.		
		/// </summary>
		/// <returns><c>true</c>, if deferred was processed, <c>false</c> otherwise.</returns>
		public bool ProcessDeferred(
			TDeferredCompiler compiler,
			string[] shaderStrings,
			int numStrings,
			int[] inputLengths,
			string customPreamble,
			EShOptimizationLevel optLevel,
			TBuiltInResource resources,
			int defaultVersion,         // use 100 for ES environment, 110 for desktop
			Profile defaultProfile,
			// set version/profile to defaultVersion/defaultProfile regardless of the #version
			// directive in the source code
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,     // give errors for use of deprecated features
			EShMessages messages,       // warnings/errors/AST; things to print out
			out TIntermediate intermediate, // returned tree, etc.
			Func<TParseContext,
				PreprocessorContext,
				TInputScanner,
				bool,
				bool> processingContext,
			bool requireNonempty)
		{
			intermediate = null;

			if (numStrings == 0)
			{				
				return true;
			}

			// Move to length-based strings, rather than null-terminated strings.
			// Also, add strings to include the preamble and to ensure the shader is not null,
			// which lets the grammar accept what was a null (post preprocessing) shader.
			//
			// Shader will look like
			//   string 0:                system preamble
			//   string 1:                custom preamble
			//   string 2...numStrings+1: user's shader
			//   string numStrings+2:     "int;"
			int numPre = 2;
			int numPost = requireNonempty? 1 : 0;
			var strings = new string[numStrings + numPre + numPost];
			for (int s = 0; s < numStrings; ++s) {
				strings[s + numPre] = shaderStrings[s];
			}

			// First, without using the preprocessor or parser, find the #version, so we know what
			// symbol tables, processing rules, etc. to set up.  This does not need the extra strings
			// outlined above, just the user shader.
			int version;
			Profile profile;
			var userInput = new TInputScanner(shaderStrings, 0 , 0);  // no preamble
			bool versionNotFirstToken;
			bool versionNotFirst = userInput.scanVersion(out version, out profile, out versionNotFirstToken);
			bool versionNotFound = version == 0;
			if (forceDefaultVersionAndProfile) {
				if (((messages & EShMessages.EShMsgSuppressWarnings) == 0) && !versionNotFound &&
					(version != defaultVersion || profile != defaultProfile)) {
					compiler.infoSink.info
						.append ("Warning, (version, profile) forced to be (")
						.append (defaultVersion.ToString ())
						.append (", ")
						.append (ProfileName (defaultProfile))
						.append ("), while in source code it is (")
						.append (version.ToString())
						.append (", ")
						.append (ProfileName(profile))
						.append (")\n");
				}

				if (versionNotFound) {
					versionNotFirstToken = false;
					versionNotFirst = false;
					versionNotFound = false;
				}
				version = defaultVersion;
				profile = defaultProfile;
			}
			bool goodVersion = DeduceVersionProfile(
				compiler.infoSink,
				compiler.getLanguage(), 
				versionNotFirst,
				defaultVersion, 
				ref version,
				ref profile);
			bool versionWillBeError = (versionNotFound || (profile == Profile.EsProfile && version >= 300 && versionNotFirst));
			bool warnVersionNotFirst = false;
			if (! versionWillBeError && versionNotFirstToken) {
				if ((messages & EShMessages.RelaxedErrors) > 0)
					warnVersionNotFirst = true;
				else
					versionWillBeError = true;
			}

			//intermediate.setVersion(version);
			//intermediate.setProfile(profile);


			// Add built-in symbols that are potentially context dependent;
			// they get popped again further down.
			//AddContextSpecificSymbols(resources, compiler.infoSink, symbolTable, version, profile, compiler.getLanguage());

			//
			// Now we can process the full shader under proper symbols and rules.
			//

			var parseContext = new TParseContext(false, version, profile, compiler.getLanguage(), compiler.infoSink, forwardCompatible);
			var scanContext = new TScanContext(parseContext);
			var ppContext = new PreprocessorContext(parseContext);
			parseContext.setScanContext(scanContext);
			parseContext.setPpContext(ppContext);
			parseContext.setLimits(resources);
			if (! goodVersion)
				parseContext.addError();
			if (warnVersionNotFirst) {
				TSourceLoc loc = new TSourceLoc ();
				parseContext.warn(loc, "Illegal to have non-comment, non-whitespace tokens before #version", "#version", "");
			}

			parseContext.initializeExtensionBehavior();


			// Fill in the strings as outlined above.
			strings[0] = ""; //parseContext.getPreamble();
			strings[1] = customPreamble;
			if (2 != numPre)
			{
				throw new Exception ("Preamble check");
			}

			if (requireNonempty) {
				strings[numStrings + numPre] = "\n int;";
			}
			//var fullInput = new TInputScanner(strings, numPre, numPost);
			var fullInput = new TInputScanner(shaderStrings, 0 , 0);

			bool success = processingContext(parseContext, ppContext, fullInput, versionWillBeError);

			return success;
		}

		static bool DeduceVersionProfile(TInfoSink infoSink, EShLanguage stage, bool versionNotFirst, int defaultVersion, ref int version, ref Profile profile)
		{
			const int FirstProfileVersion = 150;
			bool correct = true;

			// Get a good version...
			if (version == 0) {
				version = defaultVersion;
				// infoSink.info.message(EPrefixWarning, "#version: statement missing; use #version on first line of shader");
			}

			// Get a good profile...
			if (profile == Profile.NoProfile) {
				if (version == 300 || version == 310) {
					correct = false;
					infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: versions 300 and 310 require specifying the 'es' profile");
					profile = Profile.EsProfile;
				} else if (version == 100)
					profile = Profile.EsProfile;
				else if (version >= FirstProfileVersion)
					profile = Profile.CoreProfile;
				else
					profile = Profile.NoProfile;
			} else {
				// a profile was provided...
				if (version < 150) {
					correct = false;
					infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: versions before 150 do not allow a profile token");
					if (version == 100)
						profile = Profile.EsProfile;
					else
						profile = Profile.NoProfile;
				} else if (version == 300 || version == 310) {
					if (profile != Profile.EsProfile) {
						correct = false;
						infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: versions 300 and 310 support only the es profile");
					}
					profile = Profile.EsProfile;
				} else {
					if (profile == Profile.EsProfile) {
						correct = false;
						infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: only version 300 and 310 support the es profile");
						if (version >= FirstProfileVersion)
							profile = Profile.CoreProfile;
						else
							profile = Profile.NoProfile;
					} 
					// else: typical desktop case... e.g., "#version 410 core"
				}
			}

			// Correct for stage type...
			switch (stage) {
			case EShLanguage.EShLangGeometry:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 150)) {
					correct = false;
					infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: geometry shaders require es profile with version 310 or non-es profile with version 150 or above");
					version = (profile == Profile.EsProfile) ? 310 : 150;
					if (profile == Profile.EsProfile || profile == Profile.NoProfile)
						profile = Profile.CoreProfile;
				}
				break;
			case EShLanguage.EShLangTessControl:
			case EShLanguage.EShLangTessEvaluation:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 150)) {
					correct = false;
					infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: tessellation shaders require es profile with version 310 or non-es profile with version 150 or above");
					version = (profile == Profile.EsProfile) ? 310 : 400; // 150 supports the extension, correction is to 400 which does not
					if (profile == Profile.EsProfile || profile == Profile.NoProfile)
						profile = Profile.CoreProfile;
				}
				break;
			case EShLanguage.EShLangCompute:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 420)) {
					correct = false;
					infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: compute shaders require es profile with version 310 or above, or non-es profile with version 420 or above");
					version = profile == Profile.EsProfile ? 310 : 430; // 420 supports the extension, correction is to 430 which does not
					profile = Profile.CoreProfile;
				}
				break;
			default:
				break;
			}

			if (profile == Profile.EsProfile && version >= 300 && versionNotFirst) {
				correct = false;
				infoSink.info.message(TInfoSinkBase.TPrefixType.EPrefixError, "#version: statement must appear first in es-profile shader; before comments or newlines");
			}

			// A metacheck on the condition of the compiler itself...
			switch (version) {

			// ES versions
			case 100:
			case 300:
				// versions are complete
				break;

				// Desktop versions
			case 110:
			case 120:
			case 130:
			case 140:
			case 150:
			case 330:
				// versions are complete
				break;

			case 310:
			case 400:
			case 410:
			case 420:
			case 430:
			case 440:
			case 450:
				infoSink.info
					.append("Warning, version ")
					.append(version.ToString())
					.append(" is not yet complete; most version-specific features are present, but some are missing.\n");
				break;

			default:
				infoSink.info
					.append ("Warning, version ")
					.append (version.ToString ())
					.append (" is unknown.\n");
				break;

			}

			return correct;
		}

		// only one of these needed for non-ES; ES needs 2 for different precision defaults of built-ins
		enum EPrecisionClass : int
		{
			EPcGeneral = 0,
			EPcFragment,
			EPcCount
		};

		/// <summary>
		/// Local mapping functions for making arrays of symbol tables....
		/// </summary>
		/// <returns>The version to index.</returns>
		/// <param name="version">Version.</param>
		static int MapVersionToIndex(int version)
		{
			switch (version) {
			case 100: return  0;
			case 110: return  1;
			case 120: return  2;
			case 130: return  3;
			case 140: return  4;
			case 150: return  5;
			case 300: return  6;
			case 330: return  7;
			case 400: return  8;
			case 410: return  9;
			case 420: return 10;
			case 430: return 11;
			case 440: return 12;
			case 310: return 13;
			case 450: return 14;
			default:       // |
				return  0; // |
			}              // |
		}                      // V

		static int MapProfileToIndex(Profile profile)
		{
			switch (profile) {
			case Profile.NoProfile: 
				return 0;
			case Profile.CoreProfile: 
				return 1;
			case Profile.CompatibilityProfile:
				return 2;
			case Profile.EsProfile:
				return 3;
			default:                         // |
				return 0;                    // |
			}                                // |
		}   


	}
}

