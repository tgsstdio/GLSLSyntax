using System;
using System.Text;
using System.Collections.Generic;

namespace GLSLSyntaxAST.CodeDom
{
	public class TParseContext
	{

		public bool parsingBuiltins; 
		public TParseContext(
			bool pb,
			int version,
			Profile profile,
			EShLanguage language,
			TInfoSink iSink,
			bool forwardCompatible)
		{
			parsingBuiltins = pb;
			infoSink = iSink;
			extensionBehavior = new Dictionary<string, ExtensionBehavior> ();
		}

		public void setScanContext(TScanContext c)
		{
			
		}

		public void setPpContext(PreprocessorContext pp)
		{

		}

		public void setLimits(TBuiltInResource resources)
		{

		}

		public int numErrors; 
		public void addError() {
			++numErrors; 
		}

		public void setScanner(TInputScanner scanner)
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

		private TInfoSink infoSink;
		public void profileRequires(TSourceLoc loc, Profile profileMask, int minVersion, int numExtensions, string[] extensions, string featureDesc)
		{
			if ((profile & profileMask) > 0) {
				bool okay = false;
				if (minVersion > 0 && version >= minVersion)
					okay = true;
				for (int i = 0; i < numExtensions; ++i) {
					switch (getExtensionBehavior(extensions[i])) {
					case ExtensionBehavior.Warn:
						infoSink.info.message (TInfoSinkBase.TPrefixType.EPrefixWarning, "extension " + extensions [i] + " is being used for " + featureDesc, loc);
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
				profileRequires(loc, Profile.EsProfile, 300, null, message);
				profileRequires(loc, ~ Profile.EsProfile, 420, GL_ARB_SHADING_LANGUAGE_420PACK, message);
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
			if ((profile & profileMask) == 0)
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

		private TIntermediate intermediate;
		void updateExtensionBehavior(string extension, ExtensionBehavior behavior)
		{
			// Update the current behavior
			if (extension == "all")
			{
				// special case for the 'all' extension; apply it to every extension present
				if (behavior == ExtensionBehavior.Require || behavior == ExtensionBehavior.Enable) {
					error(getCurrentLoc(), "extension 'all' cannot have 'require' or 'enable' behavior", "#extension", "");
					return;
				} 
				else 
				{					
					string[] keys = new string[extensionBehavior.Keys.Count];
					extensionBehavior.Keys.CopyTo (keys, 0);
					foreach (string key in keys)
					{
						extensionBehavior[key] = behavior;
					}
				}
			} 
			else
			{
				// Do the update for this single extension
				ExtensionBehavior found;
				if (extensionBehavior.TryGetValue (extension, out found))
				{
					if (found == ExtensionBehavior.DisablePartial)
						warn (getCurrentLoc (), "extension is only partially supported:", "#extension", extension);
					if (behavior == ExtensionBehavior.Enable || behavior == ExtensionBehavior.Require)
						intermediate.addRequestedExtension (extension);
					extensionBehavior[extension] = behavior;
				} 
				else
				{
					switch (behavior) {
					case ExtensionBehavior.Require:
						error(getCurrentLoc(), "extension not supported:", "#extension", extension);
						break;
					case ExtensionBehavior.Enable:
					case ExtensionBehavior.Warn:
					case ExtensionBehavior.Disable:
						warn(getCurrentLoc(), "extension not supported:", "#extension", extension);
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
		public void initializeExtensionBehavior()
		{
			extensionBehavior[GL_OES_texture_3D]                   = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_standard_derivatives]         = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_frag_depth]                   = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_EGL_image_external]           = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_shader_texture_lod]           = ExtensionBehavior.Disable;

			extensionBehavior[GL_ARB_texture_rectangle]            = ExtensionBehavior.Disable;
			extensionBehavior[GL_3DL_array_objects]                = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_shading_language_420pack]     = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_texture_gather]               = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_gpu_shader5]                  = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_ARB_separate_shader_objects]      = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_compute_shader]               = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_ARB_tessellation_shader]          = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_enhanced_layouts]             = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_texture_cube_map_array]       = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_shader_texture_lod]           = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_explicit_attrib_location]     = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_shader_image_load_store]      = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_shader_atomic_counters]       = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_derivative_control]           = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_shader_texture_image_samples] = ExtensionBehavior.Disable;
			extensionBehavior[GL_ARB_viewport_array]               = ExtensionBehavior.Disable;
			//    extensionBehavior[GL_ARB_cull_distance]                = ExtensionBehavior.Disable;    // present for 4.5, but need extension control over block members

			// AEP
			extensionBehavior[GL_ANDROID_extension_pack_es31a]             = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_KHR_blend_equation_advanced]              = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_sample_variables]                     = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_shader_image_atomic]                  = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_shader_multisample_interpolation]     = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_texture_storage_multisample_2d_array] = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_EXT_geometry_shader]                      = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_geometry_point_size]                  = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_gpu_shader5]                          = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_EXT_primitive_bounding_box]               = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_EXT_shader_io_blocks]                     = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_tessellation_shader]                  = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_tessellation_point_size]              = ExtensionBehavior.Disable;
			extensionBehavior[GL_EXT_texture_buffer]                       = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_EXT_texture_cube_map_array]               = ExtensionBehavior.DisablePartial;

			// OES matching AEP
			extensionBehavior[GL_OES_geometry_shader]          = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_geometry_point_size]      = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_gpu_shader5]              = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_primitive_bounding_box]   = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_shader_io_blocks]         = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_tessellation_shader]      = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_tessellation_point_size]  = ExtensionBehavior.Disable;
			extensionBehavior[GL_OES_texture_buffer]           = ExtensionBehavior.DisablePartial;
			extensionBehavior[GL_OES_texture_cube_map_array]   = ExtensionBehavior.DisablePartial;
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

		// Get code that is not part of a shared symbol table, is specific to this shader,
		// or needed by the preprocessor (which does not use a shared symbol table).
		public string getPreamble()
		{
			if (profile == Profile.EsProfile) {
					return
@"#define GL_ES 1
#define GL_FRAGMENT_PRECISION_HIGH 1
#define GL_OES_texture_3D 1
#define GL_OES_standard_derivatives 1
#define GL_EXT_frag_depth 1
#define GL_OES_EGL_image_external 1
#define GL_EXT_shader_texture_lod 1
#define GL_ANDROID_extension_pack_es31a 1
#define GL_KHR_blend_equation_advanced 1
#define GL_OES_sample_variables 1
#define GL_OES_shader_image_atomic 1\
#define GL_OES_shader_multisample_interpolation 1
#define GL_OES_texture_storage_multisample_2d_array 1
#define GL_EXT_geometry_shader 1
#define GL_EXT_geometry_point_size 1
#define GL_EXT_gpu_shader5 1
#define GL_EXT_primitive_bounding_box 1
#define GL_EXT_shader_io_blocks 1
#define GL_EXT_tessellation_shader 1
#define GL_EXT_tessellation_point_size 1
#define GL_EXT_texture_buffer 1
#define GL_EXT_texture_cube_map_array 1\
#define GL_OES_geometry_shader 1
#define GL_OES_geometry_point_size 1
#define GL_OES_gpu_shader5 1\
#define GL_OES_primitive_bounding_box 1
#define GL_OES_shader_io_blocks 1
#define GL_OES_tessellation_shader 1
#define GL_OES_tessellation_point_size 1
#define GL_OES_texture_buffer 1
#define GL_OES_texture_cube_map_array 1";
					
			} else {
				return
@"#define GL_FRAGMENT_PRECISION_HIGH 1
#define GL_ARB_texture_rectangle 1
#define GL_ARB_shading_language_420pack 1
#define GL_ARB_texture_gather 1
#define GL_ARB_gpu_shader5 1
#define GL_ARB_separate_shader_objects 1
#define GL_ARB_compute_shader 1
#define GL_ARB_tessellation_shader 1
#define GL_ARB_enhanced_layouts 1
#define GL_ARB_texture_cube_map_array 1
#define GL_ARB_shader_texture_lod 1
#define GL_ARB_explicit_attrib_location 1
#define GL_ARB_shader_image_load_store 1
#define GL_ARB_shader_atomic_counters 1
#define GL_ARB_derivative_control 1
#define GL_ARB_shader_texture_image_samples 1
#define GL_ARB_viewport_array 1";
					//            "#define GL_ARB_cull_distance 1\n"    // present for 4.5, but need extension control over block members
				
			}
		}
	}
}

