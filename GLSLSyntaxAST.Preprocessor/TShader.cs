using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class TShader
	{
		private readonly InfoSink mInfoSink;
		private ShaderLanguage mLanguage;
		private GLSLIntermediate mIntermediate;
		public TShader (InfoSink sink, ShaderLanguage language, GLSLIntermediate intermediate)
		{
			mInfoSink = sink;
			mLanguage = language;
			mIntermediate = intermediate;
		}

		public string[] mStrings;
		public void setStrings(string[] s)
		{
			mStrings = s;
		}

		public bool preprocess (
			int defaultVersion,
			Profile defaultProfile,
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,
			ShaderMessages message,
			out string outputString)
		{
			return PreprocessDeferred(
				mStrings,
				defaultVersion,
				defaultProfile,
				forceDefaultVersionAndProfile,
				forwardCompatible,
				message,
				out outputString);
		}

		// Take a single compilation unit, and run the preprocessor on it.
		// Return: True if there were no issues found in preprocessing,
		//         False if during preprocessing any unknown version, pragmas or
		//         extensions were found.
		bool PreprocessDeferred(
			string[] shaderStrings,
			int defaultVersion,         // use 100 for ES environment, 110 for desktop
			Profile defaultProfile,
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,     // give errors for use of deprecated features
			ShaderMessages messages,       // warnings/errors/AST; things to print out
			out string outputString)
		{
			var parser = new DoPreprocessing();
			bool result = ProcessDeferred(shaderStrings, defaultVersion, defaultProfile, forceDefaultVersionAndProfile,
				forwardCompatible, messages, parser.DoStuff);
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
		/// Processes the deferred.
		/// </summary>
		/// <returns><c>true</c>, if deferred was processed, <c>false</c> otherwise.</returns>
		/// <param name="shaderStrings">Shader strings.</param>
		/// <param name="defaultVersion">use 100 for ES environment, 110 for desktop</param>
		/// <param name="defaultProfile">Default profile.</param>
		/// <param name="forceDefaultVersionAndProfile">set version/profile to defaultVersion/defaultProfile regardless of the #version directive in the source code </param>
		/// <param name="forwardCompatible">give errors for use of deprecated features</param>
		/// <param name="messages">warnings/errors/AST; things to print out</param>
		/// <param name="processingContext">Processing context.</param>
		public bool ProcessDeferred(
			string[] shaderStrings,
			int defaultVersion,
			Profile defaultProfile,
			bool forceDefaultVersionAndProfile,
			bool forwardCompatible,
			ShaderMessages messages,
			Func<TParseContext,
				PreprocessorContext,
				InputScanner,
				bool,
				bool> processingContext)
		{
			if (shaderStrings.Length == 0)
			{				
				return true;
			}

			// First, without using the preprocessor or parser, find the #version, so we know what
			// symbol tables, processing rules, etc. to set up.  This does not need the extra strings
			// outlined above, just the user shader.
			int version;
			Profile profile;
			var userInput = new InputScanner(shaderStrings, 0 , 0);  // no preamble
			bool versionNotFirstToken;
			bool versionNotFirst = userInput.scanVersion(out version, out profile, out versionNotFirstToken);
			bool versionNotFound = version == 0;
			if (forceDefaultVersionAndProfile) {
				if (((messages & ShaderMessages.SuppressWarnings) == 0) && !versionNotFound &&
					(version != defaultVersion || profile != defaultProfile)) {
					mInfoSink.info
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
				mInfoSink,
				mLanguage, 
				versionNotFirst,
				defaultVersion, 
				ref version,
				ref profile);
			bool versionWillBeError = (versionNotFound || (profile == Profile.EsProfile && version >= 300 && versionNotFirst));
			bool warnVersionNotFirst = false;
			if (! versionWillBeError && versionNotFirstToken) {
				if ((messages & ShaderMessages.RelaxedErrors) > 0)
					warnVersionNotFirst = true;
				else
					versionWillBeError = true;
			}
			mIntermediate.setVersion(version);
			mIntermediate.setProfile(profile);
			//
			// Now we can process the full shader under proper symbols and rules.
			//

			var parseContext = new TParseContext(version, profile, mInfoSink, forwardCompatible);
			var ppContext = new PreprocessorContext(parseContext);
			parseContext.setPpContext(ppContext);
			if (! goodVersion)
				parseContext.addError();
			if (warnVersionNotFirst) {
				SourceLocation loc = new SourceLocation ();
				parseContext.warn(loc, "Illegal to have non-comment, non-whitespace tokens before #version", "#version", "");
			}

			parseContext.initializeExtensionBehavior();
			// not recommended 

			ppContext.SetProgramDefineAsInt ("GL_ARB_shader_storage_buffer_object", 1);

			parseContext.SetPreambleManually ();
			//var fullInput = new TInputScanner(strings, numPre, numPost);
			var fullInput = new InputScanner(shaderStrings, 0 , 0);

			bool success = processingContext(parseContext, ppContext, fullInput, versionWillBeError);

			return success;
		}

		static bool DeduceVersionProfile(InfoSink infoSink, ShaderLanguage stage, bool versionNotFirst, int defaultVersion, ref int version, ref Profile profile)
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
					infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: versions 300 and 310 require specifying the 'es' profile");
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
					infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: versions before 150 do not allow a profile token");
					if (version == 100)
						profile = Profile.EsProfile;
					else
						profile = Profile.NoProfile;
				} else if (version == 300 || version == 310) {
					if (profile != Profile.EsProfile) {
						correct = false;
						infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: versions 300 and 310 support only the es profile");
					}
					profile = Profile.EsProfile;
				} else {
					if (profile == Profile.EsProfile) {
						correct = false;
						infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: only version 300 and 310 support the es profile");
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
			case ShaderLanguage.Geometry:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 150)) {
					correct = false;
					infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: geometry shaders require es profile with version 310 or non-es profile with version 150 or above");
					version = (profile == Profile.EsProfile) ? 310 : 150;
					if (profile == Profile.EsProfile || profile == Profile.NoProfile)
						profile = Profile.CoreProfile;
				}
				break;
			case ShaderLanguage.TessControl:
			case ShaderLanguage.TessEvaluation:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 150)) {
					correct = false;
					infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: tessellation shaders require es profile with version 310 or non-es profile with version 150 or above");
					version = (profile == Profile.EsProfile) ? 310 : 400; // 150 supports the extension, correction is to 400 which does not
					if (profile == Profile.EsProfile || profile == Profile.NoProfile)
						profile = Profile.CoreProfile;
				}
				break;
			case ShaderLanguage.Compute:
				if ((profile == Profile.EsProfile && version < 310) ||
					(profile != Profile.EsProfile && version < 420)) {
					correct = false;
					infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: compute shaders require es profile with version 310 or above, or non-es profile with version 420 or above");
					version = profile == Profile.EsProfile ? 310 : 430; // 420 supports the extension, correction is to 430 which does not
					profile = Profile.CoreProfile;
				}
				break;
			default:
				break;
			}

			if (profile == Profile.EsProfile && version >= 300 && versionNotFirst) {
				correct = false;
				infoSink.info.message(InfoSinkBase.TPrefixType.EPrefixError, "#version: statement must appear first in es-profile shader; before comments or newlines");
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
	}
}

