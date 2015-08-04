using System;
using GLSLSyntaxAST.CodeDom;

namespace GLSLSyntaxAST.UnitTests
{
	public class Standalone
	{
		public Standalone ()
		{
		}

		//
		// These are the default resources for TBuiltInResources, used for both
		//  - parsing this string for the case where the user didn't supply one
		//  - dumping out a template for user construction of a config file
		//

		TBuiltInResource Resources;

		//
		// Parse either a .conf file provided by the user or the default string above.
		//
		void ProcessConfigFile()
		{
			Resources = new TBuiltInResource ();
			Resources.limits = new TLimits ();
			Resources.maxLights = 32;
			Resources.maxClipPlanes = 6;
			Resources.maxTextureUnits = 32;
			Resources.maxTextureCoords = 32;
			Resources.maxVertexAttribs = 64;
			Resources.maxVertexUniformComponents = 4096;
			Resources.maxVaryingFloats = 64;
			Resources.maxVertexTextureImageUnits = 32;
			Resources.maxCombinedTextureImageUnits = 80;
			Resources.maxTextureImageUnits = 32;
			Resources.maxFragmentUniformComponents = 4096;
			Resources.maxDrawBuffers = 32;
			Resources.maxVertexUniformVectors = 128;
			Resources.maxVaryingVectors = 8;
			Resources.maxFragmentUniformVectors = 16;
			Resources.maxVertexOutputVectors = 16;
			Resources.maxFragmentInputVectors = 15;
			Resources.minProgramTexelOffset = -8;
			Resources.maxProgramTexelOffset = 7;
			Resources.maxClipDistances = 8;
			Resources.maxComputeWorkGroupCountX = 65535;
			Resources.maxComputeWorkGroupCountY = 65535;
			Resources.maxComputeWorkGroupCountZ = 65535;
			Resources.maxComputeWorkGroupSizeX = 1024;
			Resources.maxComputeWorkGroupSizeY = 1024;
			Resources.maxComputeWorkGroupSizeZ = 64;
			Resources.maxComputeUniformComponents = 1024;
			Resources.maxComputeTextureImageUnits = 16;
			Resources.maxComputeImageUniforms = 8;
			Resources.maxComputeAtomicCounters = 8;
			Resources.maxComputeAtomicCounterBuffers = 1;
			Resources.maxVaryingComponents = 60;
			Resources.maxVertexOutputComponents = 64;
			Resources.maxGeometryInputComponents = 64;
			Resources.maxGeometryOutputComponents = 128;
			Resources.maxFragmentInputComponents = 128;
			Resources.maxImageUnits = 8;
			Resources.maxCombinedImageUnitsAndFragmentOutputs = 8;
			Resources.maxCombinedShaderOutputResources = 8;
			Resources.maxImageSamples = 0;
			Resources.maxVertexImageUniforms = 0;
			Resources.maxTessControlImageUniforms = 0;
			Resources.maxTessEvaluationImageUniforms = 0;
			Resources.maxGeometryImageUniforms = 0;
			Resources.maxFragmentImageUniforms = 8;
			Resources.maxCombinedImageUniforms = 8;
			Resources.maxGeometryTextureImageUnits = 16;
			Resources.maxGeometryOutputVertices = 256;
			Resources.maxGeometryTotalOutputComponents = 1024;
			Resources.maxGeometryUniformComponents = 1024;
			Resources.maxGeometryVaryingComponents = 64;
			Resources.maxTessControlInputComponents = 128;
			Resources.maxTessControlOutputComponents = 128;
			Resources.maxTessControlTextureImageUnits = 16;
			Resources.maxTessControlUniformComponents = 1024;
			Resources.maxTessControlTotalOutputComponents = 4096;
			Resources.maxTessEvaluationInputComponents = 128;
			Resources.maxTessEvaluationOutputComponents = 128;
			Resources.maxTessEvaluationTextureImageUnits = 16;
			Resources.maxTessEvaluationUniformComponents = 1024;
			Resources.maxTessPatchComponents = 120;
			Resources.maxPatchVertices = 32;
			Resources.maxTessGenLevel = 64;
			Resources.maxViewports = 16;
			Resources.maxVertexAtomicCounters = 0;
			Resources.maxTessControlAtomicCounters = 0;
			Resources.maxTessEvaluationAtomicCounters = 0;
			Resources.maxGeometryAtomicCounters = 0;
			Resources.maxFragmentAtomicCounters = 8;
			Resources.maxCombinedAtomicCounters = 8;
			Resources.maxAtomicCounterBindings = 1;
			Resources.maxVertexAtomicCounterBuffers = 0;
			Resources.maxTessControlAtomicCounterBuffers = 0;
			Resources.maxTessEvaluationAtomicCounterBuffers = 0;
			Resources.maxGeometryAtomicCounterBuffers = 0;
			Resources.maxFragmentAtomicCounterBuffers = 1;
			Resources.maxCombinedAtomicCounterBuffers = 1;
			Resources.maxAtomicCounterBufferSize = 16384;
			Resources.maxTransformFeedbackBuffers = 4;
			Resources.maxTransformFeedbackInterleavedComponents = 64;
			Resources.maxCullDistances = 8;
			Resources.maxCombinedClipAndCullDistances = 8;
			Resources.maxSamples = 4;
			Resources.limits.nonInductiveForLoops = (1 != 0);
			Resources.limits.whileLoops = (1 != 0);
			Resources.limits.doWhileLoops = (1 != 0);
			Resources.limits.generalUniformIndexing = (1 != 0);
			Resources.limits.generalAttributeMatrixVectorIndexing = (1 != 0);
			Resources.limits.generalVaryingIndexing = (1 != 0);
			Resources.limits.generalSamplerIndexing = (1 != 0);
			Resources.limits.generalVariableIndexing = (1 != 0);
			Resources.limits.generalConstantMatrixVectorIndexing = (1 != 0);
		}

		//
		//   Deduce the language from the filename.  Files must end in one of the
		//   following extensions:
		//
		//   .vert = vertex
		//   .tesc = tessellation control
		//   .tese = tessellation evaluation
		//   .geom = geometry
		//   .frag = fragment
		//   .comp = compute
		//
		EShLanguage FindLanguage(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return EShLanguage.EShLangVertex;
			}

			string suffix = System.IO.Path.GetExtension(fileName);
			if (suffix == "vert")
				return EShLanguage.EShLangVertex;
			else if (suffix == "tesc")
				return EShLanguage.EShLangTessControl;
			else if (suffix == "tese")
				return EShLanguage.EShLangTessEvaluation;
			else if (suffix == "geom")
				return EShLanguage.EShLangGeometry;
			else if (suffix == "frag")
				return EShLanguage.EShLangFragment;
			else if (suffix == "comp")
				return EShLanguage.EShLangCompute;

			return EShLanguage.EShLangVertex;
		}

		[Flags]
		public enum TOptions : int
		{
			EOptionNone               = 0x0000,
			EOptionIntermediate       = 0x0001,
			EOptionSuppressInfolog    = 0x0002,
			EOptionMemoryLeakMode     = 0x0004,
			EOptionRelaxedErrors      = 0x0008,
			EOptionGiveWarnings       = 0x0010,
			EOptionLinkProgram        = 0x0020,
			EOptionMultiThreaded      = 0x0040,
			EOptionDumpConfig         = 0x0080,
			EOptionDumpReflection     = 0x0100,
			EOptionSuppressWarnings   = 0x0200,
			EOptionDumpVersions       = 0x0400,
			EOptionSpv                = 0x0800,
			EOptionHumanReadableSpv   = 0x1000,
			EOptionVulkanRules        = 0x2000,
			EOptionDefaultDesktop     = 0x4000,
			EOptionOutputPreprocessed = 0x8000,
		};




		int Options = 0;
		//
		// Translate the meaningful subset of command-line options to parser-behavior options.
		//
		void SetMessageOptions(EShMessages messages)
		{
			if ((Options & (int) TOptions.EOptionRelaxedErrors) > 0)
				messages = (EShMessages)(messages | EShMessages.RelaxedErrors);
			if ((Options & (int) TOptions.EOptionIntermediate) > 0)
				messages = (EShMessages)(messages | EShMessages.EShMsgAST);
			if ((Options & (int) TOptions.EOptionSuppressWarnings) > 0)
				messages = (EShMessages)(messages | EShMessages.EShMsgSuppressWarnings);
			if ((Options & (int) TOptions.EOptionSpv) > 0)
				messages = (EShMessages)(messages | EShMessages.EShMsgSpvRules);
			if ((Options & (int) TOptions.EOptionVulkanRules) > 0)
				messages = (EShMessages)(messages | EShMessages.EShMsgVulkanRules);
			if ((Options & (int) TOptions.EOptionOutputPreprocessed) > 0)
				messages = (EShMessages)(messages | EShMessages.EShMsgOnlyPreprocessor);
		}



		public void Run(string fileName)
		{
			ProcessConfigFile();

			// keep track of what to free
			//std::list<glslang::TShader*> shaders;

			EShMessages messages = EShMessages.Default;
			SetMessageOptions(messages);

			//
			// Per-shader processing...
			//

			EShLanguage stage = FindLanguage(fileName);
			var infoSink = new TInfoSink ();
			var compiler = new TDeferredCompiler(stage, infoSink);
			var intermediate = new TIntermediate(stage);

			var shader = new TShader(stage, infoSink, compiler, intermediate);

			string shaderStrings = null;
			using(var fs = OpenRead(fileName))
			{
			//	shaderStrings = fs
			}

			const int defaultVersion = Options & EOptionDefaultDesktop? 110: 100;

			shader.setStrings(shaderStrings, 1);
			if (Options & EOptionOutputPreprocessed) {
				string str;
				if (shader.preprocess(&Resources, defaultVersion, ENoProfile,
					false, false, messages, &str)) {
					PutsIfNonEmpty(str.c_str());
				} else {
					CompileFailed = true;
				}
				StderrIfNonEmpty(shader->getInfoLog());
				StderrIfNonEmpty(shader->getInfoDebugLog());
				FreeFileData(shaderStrings);
				continue;
			}
			if (! shader->parse(&Resources, defaultVersion, false, messages))
				CompileFailed = true;


			if (! (Options & EOptionSuppressInfolog)) {
				PutsIfNonEmpty(workItem->name.c_str());
				PutsIfNonEmpty(shader->getInfoLog());
				PutsIfNonEmpty(shader->getInfoDebugLog());
			}

			FreeFileData(shaderStrings);


		}
	}
}

