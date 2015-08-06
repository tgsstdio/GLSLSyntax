using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.Preprocessor
{
	internal class ParseContext
	{
		internal ParseContext(
			int version,
			Profile profile,
			InfoSink infoSink,
			bool forwardCompatible,
			SymbolLookup symbols,
			GLSLIntermediate intermediate)
		{
			mIntermediate = intermediate;
			mVersion = version;
			mProfile = profile;
			mInfoSink = infoSink;
			mExtensionBehaviors = new Dictionary<string, ExtensionBehavior> ();
			contextPragma = new Pragma (true, false);
			mSymbols = symbols;
		}
		private readonly SymbolLookup mSymbols;

		private PreprocessorContext ppContext;
		internal void setPpContext(PreprocessorContext pp)
		{
			ppContext = pp;	
		}

		public int numErrors; 
		public void addError() {
			++numErrors; 
		}

		internal void setScanner(InputScanner scanner)
		{
			currentScanner = scanner;
		}

		internal bool tokensBeforeEOF;
		internal MessageType messages;        // errors/warnings

		internal Action<int, string, string> ExtensionCallback {get;set;}

		internal Action<int, bool, int> LineCallback {get;set;}
		internal void notifyLineDirective(int line, bool hasSource, int source)
		{
			if (LineCallback != null) {
				LineCallback(line, hasSource, source);
			}
		}

		internal Action<int, int, string> VersionCallback {get;set;}
		internal void notifyVersion(int line, int version, string typeString)
		{
			if (VersionCallback != null) {
				VersionCallback(line, version, typeString);
			}
		}



		internal Action<int, string> ErrorCallback {get;set;}
		internal void notifyErrorDirective(int line, string errorMessage)
		{
			if (ErrorCallback != null) {
				ErrorCallback(line, errorMessage);
			}
		}

		InputScanner currentScanner;
		internal SourceLocation getCurrentLoc() {
			return currentScanner.getSourceLoc(); 
		}

		internal void setCurrentLine(int line)
		{ 
			currentScanner.setLine(line); 
		}

		internal void setCurrentString(int stringBias)
		{
			currentScanner.setString(stringBias); 
		}

		internal void Error(SourceLocation location, string reason, string token,
			string extraInfoFormat, params object[] args)
		{
			mInfoSink.Info.AppendPrefix(PrefixType.Error);
			mInfoSink.Info.AppendLocation(location);
			mInfoSink.Info
				.Append ("'")
				.Append (token)
				.Append ("' : ")
				.Append (reason)
				.Append (" ")
				.Append (string.Format (extraInfoFormat, args))
				.Append ("\n");
		}

		internal void Warn(SourceLocation location, string reason, string token,
			string extraInfoFormat, params object[] args)
		{
			mInfoSink.Info.AppendPrefix(PrefixType.Warning);
			mInfoSink.Info.AppendLocation(location);
			mInfoSink.Info
				.Append ("'")
				.Append (token)
				.Append ("' : ")
				.Append (reason)
				.Append (" ")
				.Append (string.Format (extraInfoFormat, args))
				.Append ("\n");
		}

		private InfoSink mInfoSink;
		internal void ProfileRequires(SourceLocation loc, Profile profileMask, int minVersion, int numExtensions, string[] extensions, string featureDesc)
		{
			if ((mProfile & profileMask) > 0) {
				bool okay = false;
				if (minVersion > 0 && mVersion >= minVersion)
					okay = true;
				for (int i = 0; i < numExtensions; ++i) {
					switch (GetExtensionBehavior(extensions[i])) {
					case ExtensionBehavior.Warn:
						mInfoSink.Info.WriteMessage (PrefixType.Warning, "extension " + extensions [i] + " is being used for " + featureDesc, loc);
						okay = true;
						break;
					case ExtensionBehavior.Require:
					case ExtensionBehavior.Enable:
						okay = true;
						break;
					default: break; // some compilers want this
					}
				}

				if (! okay)
					Error(loc, "not supported for this version or the enabled extensions", featureDesc, "");
			}
		}

		internal Dictionary<string, ExtensionBehavior> mExtensionBehaviors;    // for each extension string, what its current behavior is set to
		ExtensionBehavior GetExtensionBehavior(string extension)
		{
			ExtensionBehavior result;
			if (mExtensionBehaviors.TryGetValue (extension, out result))
			{
				return result;			
			} else
			{
				return ExtensionBehavior.Missing;
			}
		}

		internal void ProfileRequires(SourceLocation loc, Profile profileMask, int minVersion, string extension, string featureDesc)
		{
			if (extension != null)
			{
				ProfileRequires (loc, profileMask, minVersion, 1, new []{extension}, featureDesc);
			} 
			else
			{
				ProfileRequires (loc, profileMask, minVersion, 0, null, featureDesc);
			}
		}

		internal int mVersion;                 // version, updated by #version in the shader
		internal Profile mProfile;            // the declared profile in the shader (core by default)

		bool extensionsTurnedOn(int numExtensions, string extensions)
		{
			throw new NotImplementedException ();
		}

		internal bool lineContinuationCheck(SourceLocation loc, bool endOfComment)
		{
			const string message = "line continuation";

			bool lineContinuationAllowed = (mProfile == Profile.EsProfile && mVersion >= 300) ||
				(mProfile != Profile.EsProfile && (mVersion >= 420 || extensionsTurnedOn(1, GL_ARB_shading_language_420pack)));

			if (endOfComment) {
				if (lineContinuationAllowed)
					Warn(loc, "used at end of comment; the following line is still part of the comment", message, "");
				else
					Warn(loc, "used at end of comment, but this version does not provide line continuation", message, "");

				return lineContinuationAllowed;
			}

			if ((messages & MessageType.RelaxedErrors) > 0) {
				if (! lineContinuationAllowed)
					Warn(loc, "not allowed in this version", message, "");
				return true;
			} else {
				ProfileRequires(loc, Profile.EsProfile, 300, null, message);
				ProfileRequires(loc, ~ Profile.EsProfile, 420, GL_ARB_shading_language_420pack, message);
			}

			return lineContinuationAllowed;
		}

		//
		// Reserved errors for the preprocessor.
		//
		public void ReservedPpErrorCheck(SourceLocation loc, string identifier, string op)
		{
			// "All macro names containing two consecutive underscores ( __ ) are reserved;
			// defining such a name does not itself result in an error, but may result in
			// undefined behavior.  All macro names prefixed with "GL_" ("GL" followed by a
			// single underscore) are also reserved, and defining such a name results in a
			// compile-time error."
			if (identifier.StartsWith("GL_"))
				Error(loc, "names beginning with \"GL_\" can't be (un)defined:", op,  identifier);
			else if (identifier.Contains("__")) {
				if (mProfile == Profile.EsProfile && mVersion >= 300 &&
					(identifier == "__LINE__" || identifier == "__FILE__" || identifier == "__VERSION__"))
				{
					Error(loc, "predefined names can't be (un)defined:", op,  identifier);
				}
				else
					Warn(loc, "names containing consecutive underscores are reserved:", op, identifier);
			}
		}

		Pragma contextPragma;
		internal Action<int, List<string>> PragmaCallback { get; set;}
		internal void handlePragma(SourceLocation loc, List<string> tokens)
		{
			if (PragmaCallback != null)
				PragmaCallback(loc.line, tokens);

			if (tokens.Count == 0)
				return;

			if (tokens[0] == "optimize") {
				if (tokens.Count != 4) {
					Error(loc, "optimize pragma syntax is incorrect", "#pragma", "");
					return;
				}

				if (tokens[1] != "(") {
					Error(loc, "\"(\" expected after 'optimize' keyword", "#pragma", "");
					return;
				}

				if (tokens[2] == "on")
					contextPragma.optimize = true;
				else if (tokens[2] == "off")
					contextPragma.optimize = false;
				else {
					Error(loc, "\"on\" or \"off\" expected after '(' for 'optimize' pragma", "#pragma", "");
					return;
				}

				if (tokens[3] == ")") {
					Error(loc, "\")\" expected to end 'optimize' pragma", "#pragma", "");
					return;
				}
			} 
			else if (tokens[0] == "debug") {
				if (tokens.Count != 4) {
					Error(loc, "debug pragma syntax is incorrect", "#pragma", "");
					return;
				}

				if (tokens[1] != "(") {
					Error(loc, "\"(\" expected after 'debug' keyword", "#pragma", "");
					return;
				}

				if (tokens[2] == "on")
					contextPragma.debug = true;
				else if (tokens[2] == "off")
					contextPragma.debug = false;
				else {
					Error(loc, "\"on\" or \"off\" expected after '(' for 'debug' pragma", "#pragma", "");
					return;
				}

				if (tokens[3] == ")") {
					Error(loc, "\")\" expected to end 'debug' pragma", "#pragma", "");
					return;
				}
			}
		}

		/// <summary>
		/// Call for any operation needing GLSL double data-type support.
		/// </summary>
		internal void doubleCheck(SourceLocation loc, string op)
		{
			requireProfile(loc, Profile.CoreProfile | Profile.CompatibilityProfile, op);
			ProfileRequires(loc, Profile.CoreProfile, 400, null, op);
			ProfileRequires(loc, Profile.CompatibilityProfile, 400, null, op);
		}

		/// <summary>
		/// When to use requireProfile():
		//
		//     Use if only some profiles support a feature.  However, if within a profile the feature 
		//     is version or extension specific, follow this call with calls to profileRequires().
		//
		// Operation:  If the current profile is not one of the profileMask,
		// give an error message.
		/// </summary>
		/// <param name="loc">Location.</param>
		/// <param name="profileMask">Profile mask.</param>
		/// <param name="featureDesc">Feature desc.</param>
		internal void requireProfile(SourceLocation loc, Profile profileMask, string featureDesc)
		{
			if ((mProfile & profileMask) == 0)
				Error(loc, "not supported with this profile:", featureDesc, ProfileName(mProfile));
		}

		//
		// Map from profile enum to externally readable text name.
		//
		static string ProfileName(Profile profile)
		{
			switch (profile) {
				case Profile.NoProfile:             return "none";
				case Profile.CoreProfile:           return "core";
				case Profile.CompatibilityProfile:  return "compatibility";
				case Profile.EsProfile:             return "es";
				default:                     return "unknown profile";
			}
		}

		private GLSLIntermediate mIntermediate;
		void updateExtensionBehavior(string extension, ExtensionBehavior behavior)
		{
			// Update the current behavior
			if (extension == "all")
			{
				// special case for the 'all' extension; apply it to every extension present
				if (behavior == ExtensionBehavior.Require || behavior == ExtensionBehavior.Enable) {
					Error(getCurrentLoc(), "extension 'all' cannot have 'require' or 'enable' behavior", "#extension", "");
					return;
				} 
				else 
				{					
					string[] keys = new string[mExtensionBehaviors.Keys.Count];
					mExtensionBehaviors.Keys.CopyTo (keys, 0);
					foreach (string key in keys)
					{
						mExtensionBehaviors[key] = behavior;
					}
				}
			} 
			else
			{
				// Do the update for this single extension
				ExtensionBehavior found;
				if (mExtensionBehaviors.TryGetValue (extension, out found))
				{
					if (found == ExtensionBehavior.DisablePartial)
						Warn (getCurrentLoc (), "extension is only partially supported:", "#extension", extension);
					if (behavior == ExtensionBehavior.Enable || behavior == ExtensionBehavior.Require)
						mIntermediate.addRequestedExtension (extension);
					mExtensionBehaviors[extension] = behavior;
				} 
				else
				{
					switch (behavior) {
					case ExtensionBehavior.Require:
						Error(getCurrentLoc(), "extension not supported:", "#extension", extension);
						break;
					case ExtensionBehavior.Enable:
					case ExtensionBehavior.Warn:
					case ExtensionBehavior.Disable:
						Warn(getCurrentLoc(), "extension not supported:", "#extension", extension);
						break;
					default:
						throw new InvalidOperationException("unexpected behavior");
					}

					return;
				} 
			}
		}

		const string GL_OES_texture_3D                   = "GL_OES_texture_3D";
		const string GL_OES_standard_derivatives         = "GL_OES_standard_derivatives";
		const string GL_EXT_frag_depth                   = "GL_EXT_frag_depth";
		const string GL_OES_EGL_image_external           = "GL_OES_EGL_image_external";
		const string GL_EXT_shader_texture_lod           = "GL_EXT_shader_texture_lod";

		const string GL_ARB_texture_rectangle            = "GL_ARB_texture_rectangle";
		const string GL_3DL_array_objects                = "GL_3DL_array_objects";
		const string GL_ARB_shading_language_420pack     = "GL_ARB_shading_language_420pack";
		const string GL_ARB_texture_gather               = "GL_ARB_texture_gather";
		const string GL_ARB_gpu_shader5                  = "GL_ARB_gpu_shader5";
		const string GL_ARB_separate_shader_objects      = "GL_ARB_separate_shader_objects";
		const string GL_ARB_compute_shader               = "GL_ARB_compute_shader";
		const string GL_ARB_tessellation_shader          = "GL_ARB_tessellation_shader";
		const string GL_ARB_enhanced_layouts             = "GL_ARB_enhanced_layouts";
		const string GL_ARB_texture_cube_map_array       = "GL_ARB_texture_cube_map_array";
		const string GL_ARB_shader_texture_lod           = "GL_ARB_shader_texture_lod";
		const string GL_ARB_explicit_attrib_location     = "GL_ARB_explicit_attrib_location";
		const string GL_ARB_shader_image_load_store      = "GL_ARB_shader_image_load_store";
		const string GL_ARB_shader_atomic_counters       = "GL_ARB_shader_atomic_counters";
		const string GL_ARB_derivative_control           = "GL_ARB_derivative_control";
		const string GL_ARB_shader_texture_image_samples = "GL_ARB_shader_texture_image_samples";
		const string GL_ARB_viewport_array               = "GL_ARB_viewport_array";
		//const string GL_ARB_cull_distance            = "GL_ARB_cull_distance";  // present for 4.5, but need extension control over block members

		// AEP
		const string GL_ANDROID_extension_pack_es31a             = "GL_ANDROID_extension_pack_es31a";
		const string GL_KHR_blend_equation_advanced              = "GL_KHR_blend_equation_advanced";
		const string GL_OES_sample_variables                     = "GL_OES_sample_variables";
		const string GL_OES_shader_image_atomic                  = "GL_OES_shader_image_atomic";
		const string GL_OES_shader_multisample_interpolation     = "GL_OES_shader_multisample_interpolation";
		const string GL_OES_texture_storage_multisample_2d_array = "GL_OES_texture_storage_multisample_2d_array";
		const string GL_EXT_geometry_shader                      = "GL_EXT_geometry_shader";
		const string GL_EXT_geometry_point_size                  = "GL_EXT_geometry_point_size";
		const string GL_EXT_gpu_shader5                          = "GL_EXT_gpu_shader5";
		const string GL_EXT_primitive_bounding_box               = "GL_EXT_primitive_bounding_box";
		const string GL_EXT_shader_io_blocks                     = "GL_EXT_shader_io_blocks";
		const string GL_EXT_tessellation_shader                  = "GL_EXT_tessellation_shader";
		const string GL_EXT_tessellation_point_size              = "GL_EXT_tessellation_point_size";
		const string GL_EXT_texture_buffer                       = "GL_EXT_texture_buffer";
		const string GL_EXT_texture_cube_map_array               = "GL_EXT_texture_cube_map_array";

		// OES matching AEP
		const string GL_OES_geometry_shader                      = "GL_OES_geometry_shader";
		const string GL_OES_geometry_point_size                  = "GL_OES_geometry_point_size";
		const string GL_OES_gpu_shader5                          = "GL_OES_gpu_shader5";
		const string GL_OES_primitive_bounding_box               = "GL_OES_primitive_bounding_box";
		const string GL_OES_shader_io_blocks                     = "GL_OES_shader_io_blocks";
		const string GL_OES_tessellation_shader                  = "GL_OES_tessellation_shader";
		const string GL_OES_tessellation_point_size              = "GL_OES_tessellation_point_size";
		const string GL_OES_texture_buffer                       = "GL_OES_texture_buffer";
		const string GL_OES_texture_cube_map_array               = "GL_OES_texture_cube_map_array";

		//
		// Initialize all extensions, almost always to 'disable', as once their features
		// are incorporated into a core version, their features are supported through allowing that
		// core version, not through a pseudo-enablement of the extension.
		//
		internal void initializeExtensionBehavior()
		{
			mExtensionBehaviors[GL_OES_texture_3D]                   = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_standard_derivatives]         = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_frag_depth]                   = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_EGL_image_external]           = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_shader_texture_lod]           = ExtensionBehavior.Disable;

			mExtensionBehaviors[GL_ARB_texture_rectangle]            = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_3DL_array_objects]                = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_shading_language_420pack]     = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_texture_gather]               = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_gpu_shader5]                  = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_ARB_separate_shader_objects]      = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_compute_shader]               = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_ARB_tessellation_shader]          = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_enhanced_layouts]             = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_texture_cube_map_array]       = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_shader_texture_lod]           = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_explicit_attrib_location]     = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_shader_image_load_store]      = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_shader_atomic_counters]       = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_derivative_control]           = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_shader_texture_image_samples] = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_ARB_viewport_array]               = ExtensionBehavior.Disable;
			//    extensionBehavior[GL_ARB_cull_distance]                = ExtensionBehavior.Disable;    // present for 4.5, but need extension control over block members

			// AEP
			mExtensionBehaviors[GL_ANDROID_extension_pack_es31a]             = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_KHR_blend_equation_advanced]              = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_sample_variables]                     = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_shader_image_atomic]                  = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_shader_multisample_interpolation]     = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_texture_storage_multisample_2d_array] = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_EXT_geometry_shader]                      = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_geometry_point_size]                  = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_gpu_shader5]                          = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_EXT_primitive_bounding_box]               = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_EXT_shader_io_blocks]                     = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_tessellation_shader]                  = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_tessellation_point_size]              = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_EXT_texture_buffer]                       = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_EXT_texture_cube_map_array]               = ExtensionBehavior.DisablePartial;

			// OES matching AEP
			mExtensionBehaviors[GL_OES_geometry_shader]          = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_geometry_point_size]      = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_gpu_shader5]              = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_primitive_bounding_box]   = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_shader_io_blocks]         = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_tessellation_shader]      = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_tessellation_point_size]  = ExtensionBehavior.Disable;
			mExtensionBehaviors[GL_OES_texture_buffer]           = ExtensionBehavior.DisablePartial;
			mExtensionBehaviors[GL_OES_texture_cube_map_array]   = ExtensionBehavior.DisablePartial;
		}


		//
		// Change the current state of an extension's behavior.
		//
		internal void updateExtensionBehavior(int line, string extension, string behaviorString)
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
				Error(getCurrentLoc(), "behavior not supported:", "#extension", behaviorString);
				return;
			}

			// update the requested extension
			updateExtensionBehavior(extension, behavior);

			// see if need to propagate to implicitly modified things
			if (extension == GL_ANDROID_extension_pack_es31a) {
				// to everything in AEP
				updateExtensionBehavior(line, GL_KHR_blend_equation_advanced, behaviorString);
				updateExtensionBehavior(line, GL_OES_sample_variables, behaviorString);
				updateExtensionBehavior(line, GL_OES_shader_image_atomic, behaviorString);
				updateExtensionBehavior(line, GL_OES_shader_multisample_interpolation, behaviorString);
				updateExtensionBehavior(line, GL_OES_texture_storage_multisample_2d_array, behaviorString);
				updateExtensionBehavior(line, GL_EXT_geometry_shader, behaviorString);
				updateExtensionBehavior(line, GL_EXT_gpu_shader5, behaviorString);
				updateExtensionBehavior(line, GL_EXT_primitive_bounding_box, behaviorString);
				updateExtensionBehavior(line, GL_EXT_shader_io_blocks, behaviorString);
				updateExtensionBehavior(line, GL_EXT_tessellation_shader, behaviorString);
				updateExtensionBehavior(line, GL_EXT_texture_buffer, behaviorString);
				updateExtensionBehavior(line, GL_EXT_texture_cube_map_array, behaviorString);
			}
			// geometry to io_blocks
			else if (extension == GL_EXT_geometry_shader)
				updateExtensionBehavior(line, GL_EXT_shader_io_blocks, behaviorString);
			else if (extension == GL_OES_geometry_shader)
				updateExtensionBehavior(line, GL_OES_shader_io_blocks, behaviorString);
			// tessellation to io_blocks
			else if (extension == GL_EXT_tessellation_shader)
				updateExtensionBehavior(line, GL_EXT_shader_io_blocks, behaviorString);
			else if (extension == GL_OES_tessellation_shader)
				updateExtensionBehavior(line, GL_OES_shader_io_blocks, behaviorString);
		}
	}
}

