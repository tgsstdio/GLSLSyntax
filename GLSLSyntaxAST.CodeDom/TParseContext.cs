using System;
using System.Text;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	[Flags]
	public enum EShMessages : int
	{
		RelaxedErrors = 1
	}

	public class TParseContext
	{
		public TParseContext (TInputScanner scanner)
		{
			currentScanner = scanner;
		}

		public bool tokensBeforeEOF;
		public EShMessages messages;        // errors/warnings

		public Action<int, string, string> ExtensionCallback {get;set;}

		public Action<int, bool, int> LineCallback {get;set;}
		public void notifyLineDirective(int line, bool has_source, int source)
		{
			if (LineCallback != null) {
				LineCallback(line, has_source, source);
			}
		}

		public Action<int, int, string> VersionCallback {get;set;}
		public void notifyVersion(int line, int version, string type_string)
		{
			if (VersionCallback != null) {
				VersionCallback(line, version, type_string);
			}
		}



		public Action<int, string> ErrorCallback {get;set;}
		public void notifyErrorDirective(int line, string error_message)
		{
			if (ErrorCallback != null) {
				ErrorCallback(line, error_message);
			}
		}

		TInputScanner currentScanner;
		public TSourceLoc getCurrentLoc() {
			return currentScanner.getSourceLoc(); 
		}

		public void setCurrentLine(int line)
		{ 
			currentScanner.setLine(line); 
		}

		public void setCurrentString(int stringBias)
		{
			currentScanner.setString(stringBias); 
		}

		public void error(TSourceLoc location, string reason, string token,
			string extraInfoFormat, params string[] args)
		{

		}

		public void warn(TSourceLoc location, string reason, string token,
			string extraInfoFormat, params string[] args)
		{

		}

		public void profileRequires(TSourceLoc loc, Profile profileMask, int minVersion, int numExtensions, string[] extensions, string featureDesc)
		{
			if (profile & profileMask) {
				bool okay = false;
				if (minVersion > 0 && version >= minVersion)
					okay = true;
				for (int i = 0; i < numExtensions; ++i) {
					switch (getExtensionBehavior(extensions[i])) {
					case ExtensionBehavior.Warn:
						infoSink.info.message(EPrefixWarning, ("extension " + TString(extensions[i]) + " is being used for " + featureDesc).c_str(), loc);
						// fall through
					case ExtensionBehavior.Require:
					case ExtensionBehavior.Enable:
						okay = true;
						break;
					default: break; // some compilers want this
					}
				}

				if (! okay)
					error(loc, "not supported for this version or the enabled extensions", featureDesc, "");
			}
		}


		public Dictionary<string, ExtensionBehavior> extensionBehavior;    // for each extension string, what its current behavior is set to
		ExtensionBehavior getExtensionBehavior(string extension)
		{
			ExtensionBehavior result;
			if (extensionBehavior.TryGetValue (extension, out result))
			{
				return result;			
			} else
			{
				return ExtensionBehavior.Missing;
			}
		}

		public void profileRequires(TSourceLoc loc, Profile profileMask, int minVersion, string extension, string featureDesc)
		{
			if (extension != null)
			{
				profileRequires (loc, profileMask, minVersion, 1, new []{extension}, featureDesc);
			} 
			else
			{
				profileRequires (loc, profileMask, minVersion, 0, null, featureDesc);
			}
		}

		public int version;                 // version, updated by #version in the shader
		public Profile profile;            // the declared profile in the shader (core by default)

		bool extensionsTurnedOn(int numExtensions, string extensions)
		{
			throw new NotImplementedException ();
		}

		public bool lineContinuationCheck(TSourceLoc loc, bool endOfComment)
		{
			const string GL_ARB_SHADING_LANGUAGE_420PACK = "GL_ARB_shading_language_420pack";

			string message = "line continuation";

			bool lineContinuationAllowed = (profile == Profile.EsProfile && version >= 300) ||
				(profile != Profile.EsProfile && (version >= 420 || extensionsTurnedOn(1, GL_ARB_SHADING_LANGUAGE_420PACK)));

			if (endOfComment) {
				if (lineContinuationAllowed)
					warn(loc, "used at end of comment; the following line is still part of the comment", message, "");
				else
					warn(loc, "used at end of comment, but this version does not provide line continuation", message, "");

				return lineContinuationAllowed;
			}

			if ((messages & EShMessages.RelaxedErrors) > 0) {
				if (! lineContinuationAllowed)
					warn(loc, "not allowed in this version", message, "");
				return true;
			} else {
				profileRequires(loc, (int) Profile.EsProfile, 300, null, message);
				profileRequires(loc, ~ (int) Profile.EsProfile, 420, GL_ARB_SHADING_LANGUAGE_420PACK, message);
			}

			return lineContinuationAllowed;
		}

		//
		// Reserved errors for the preprocessor.
		//
		public void reservedPpErrorCheck(TSourceLoc loc, string identifier, string op)
		{
			// "All macro names containing two consecutive underscores ( __ ) are reserved;
			// defining such a name does not itself result in an error, but may result in
			// undefined behavior.  All macro names prefixed with "GL_" ("GL" followed by a
			// single underscore) are also reserved, and defining such a name results in a
			// compile-time error."
			if (identifier.StartsWith("GL_"))
				error(loc, "names beginning with \"GL_\" can't be (un)defined:", op,  identifier);
			else if (identifier.Contains("__")) {
				if (profile == Profile.EsProfile && version >= 300 &&
					(identifier == "__LINE__" || identifier == "__FILE__" || identifier == "__VERSION__"))
				{
					error(loc, "predefined names can't be (un)defined:", op,  identifier);
				}
				else
					warn(loc, "names containing consecutive underscores are reserved:", op, identifier);
			}
		}

		TPragma contextPragma;
		public Action<int, List<string>> PragmaCallback { get; set;}
		public void handlePragma(TSourceLoc loc, List<string> tokens)
		{
			if (PragmaCallback != null)
				PragmaCallback(loc.line, tokens);

			if (tokens.Count == 0)
				return;

			if (tokens[0] == "optimize") {
				if (tokens.Count != 4) {
					error(loc, "optimize pragma syntax is incorrect", "#pragma", "");
					return;
				}

				if (tokens[1] != "(") {
					error(loc, "\"(\" expected after 'optimize' keyword", "#pragma", "");
					return;
				}

				if (tokens[2] == "on")
					contextPragma.optimize = true;
				else if (tokens[2] == "off")
					contextPragma.optimize = false;
				else {
					error(loc, "\"on\" or \"off\" expected after '(' for 'optimize' pragma", "#pragma", "");
					return;
				}

				if (tokens[3] == ")") {
					error(loc, "\")\" expected to end 'optimize' pragma", "#pragma", "");
					return;
				}
			} 
			else if (tokens[0] == "debug") {
				if (tokens.Count != 4) {
					error(loc, "debug pragma syntax is incorrect", "#pragma", "");
					return;
				}

				if (tokens[1] != "(") {
					error(loc, "\"(\" expected after 'debug' keyword", "#pragma", "");
					return;
				}

				if (tokens[2] == "on")
					contextPragma.debug = true;
				else if (tokens[2] == "off")
					contextPragma.debug = false;
				else {
					error(loc, "\"on\" or \"off\" expected after '(' for 'debug' pragma", "#pragma", "");
					return;
				}

				if (tokens[3] == ")") {
					error(loc, "\")\" expected to end 'debug' pragma", "#pragma", "");
					return;
				}
			}
		}

		//
		// When to use requireProfile():
		//
		//     Use if only some profiles support a feature.  However, if within a profile the feature 
		//     is version or extension specific, follow this call with calls to profileRequires().
		//
		// Operation:  If the current profile is not one of the profileMask,
		// give an error message.
		//
		public void requireProfile(TSourceLoc loc, Profile profileMask, string featureDesc)
		{
			if (! (profile & profileMask))
				error(loc, "not supported with this profile:", featureDesc, ProfileName(profile));
		}

		//
		// Map from profile enum to externally readable text name.
		//
		string ProfileName(Profile profile)
		{
			switch (profile) {
				case Profile.NoProfile:             return "none";
				case Profile.CoreProfile:           return "core";
				case Profile.CompatibilityProfile:  return "compatibility";
				case Profile.EsProfile:             return "es";
				default:                     return "unknown profile";
			}
		}


		//
		// Change the current state of an extension's behavior.
		//
		public void updateExtensionBehavior(int line, string extension, string behaviorString)
		{
			if (ExtensionCallback != null)
				ExtensionCallback(line, extension, behaviorString);

			// Translate from text string of extension's behavior to an enum.
			ExtensionBehavior behavior = ExtensionBehavior.Disable;
			if (behaviorString == "require")
				behavior = ExtensionBehavior.Require;
			else if (behaviorString == "enable")
				behavior = ExtensionBehavior.Enable;
			else if (behaviorString == "disable")
				behavior = ExtensionBehavior.Disable;
			else if (behaviorString == "warn")
				behavior = ExtensionBehavior.Warn;
			else {
				error(getCurrentLoc(), "behavior not supported:", "#extension", behaviorString);
				return;
			}

			// update the requested extension
			updateExtensionBehavior(extension, behavior);

			// see if need to propagate to implicitly modified things
			if (extension == "GL_ANDROID_extension_pack_es31a") {
				// to everything in AEP
				updateExtensionBehavior(line, "GL_KHR_blend_equation_advanced", behaviorString);
				updateExtensionBehavior(line, "GL_OES_sample_variables", behaviorString);
				updateExtensionBehavior(line, "GL_OES_shader_image_atomic", behaviorString);
				updateExtensionBehavior(line, "GL_OES_shader_multisample_interpolation", behaviorString);
				updateExtensionBehavior(line, "GL_OES_texture_storage_multisample_2d_array", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_geometry_shader", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_gpu_shader5", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_primitive_bounding_box", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_shader_io_blocks", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_tessellation_shader", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_texture_buffer", behaviorString);
				updateExtensionBehavior(line, "GL_EXT_texture_cube_map_array", behaviorString);
			}
			// geometry to io_blocks
			else if (extension == "GL_EXT_geometry_shader")
				updateExtensionBehavior(line, "GL_EXT_shader_io_blocks", behaviorString);
			else if (extension == "GL_OES_geometry_shader")
				updateExtensionBehavior(line, "GL_OES_shader_io_blocks", behaviorString);
			// tessellation to io_blocks
			else if (extension == "GL_EXT_tessellation_shader")
				updateExtensionBehavior(line, "GL_EXT_shader_io_blocks", behaviorString);
			else if (extension == "GL_OES_tessellation_shader")
				updateExtensionBehavior(line, "GL_OES_shader_io_blocks", behaviorString);
		}
	}
}

