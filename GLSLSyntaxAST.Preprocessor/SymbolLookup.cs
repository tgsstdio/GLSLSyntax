using System;
using System.Collections.Generic;

namespace GLSLSyntaxAST.Preprocessor
{
	public class SymbolLookup
	{
		internal AtomLookup Atoms {get; private set;}
		private readonly Dictionary<int, Symbol> mSymbols;
		public SymbolLookup ()
		{
			Atoms = new AtomLookup ();
			mSymbols = new Dictionary<int, Symbol> ();
		}

		internal Symbol Add(int atom)
		{
			Symbol lSymb;

			lSymb = NewSymbol(atom);
			mSymbols[lSymb.atom] = lSymb;

			return lSymb;
		}

		/// <summary>
		/// Allocate a new symbol node;
		/// </summary>
		/// <returns>The symbol.</returns>
		/// <param name="atom">Atom.</param>
		private static Symbol NewSymbol(int atom)
		{
			Symbol lSymb = new Symbol ();
			lSymb.atom = atom;
			// MEMORY POOL
			//			Symbol* lSymb;
			//			char* pch;
			//			int ii;
			//
			//			lSymb = (Symbol *) mem_Alloc(pool, sizeof(Symbol));
			//			lSymb->atom = atom;
			//
			//			// Clear macro
			//			pch = (char*) &lSymb->mac;
			//			for (ii = 0; ii < sizeof(lSymb->mac); ii++)
			//				*pch++ = 0;

			return lSymb;
		}

		public Symbol LookUp(int atom)
		{
			Symbol result = null;
			if(mSymbols.TryGetValue(atom, out result))
			{	
				return result;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Sets the program define as int.
		/// Too slow to use to preamble; bypass normal processing 
		/// </summary>
		/// <param name="definedName">Defined name.</param>
		/// <param name="value">Value.</param>
		public void DefineAs(string definedName, int value)
		{
			int atom = Atoms.LookUpAddString (definedName);

			Symbol result = null;
			if(!mSymbols.TryGetValue(atom, out result))
			{	
				result = this.Add (atom);
			}

			result.mac = new MacroSymbol ();
			result.mac.body = new TokenStream ();
			var packet = new PreprocessorToken ();
			packet.name = value.ToString();
			Atoms.RecordToken (result.mac.body, (int) CppEnums.INTCONSTANT, packet);
		}
		/// <summary>
		/// Sets the program define as string.
		/// Too slow to use to preamble; bypass normal processing 
		/// Does not support recursion on string; 
		/// </summary>
		/// <param name="definedName">Defined name.</param>
		/// <param name="valueString">Value string.</param>
		public void DefineAs(string definedName, string valueString)
		{
			int atom = Atoms.LookUpAddString (definedName);

			Symbol result = null;
			if(!mSymbols.TryGetValue(atom, out result))
			{	
				result = this.Add (atom);
			}

			result.mac = new MacroSymbol ();
			result.mac.body = new TokenStream ();
			var packet = new PreprocessorToken ();
			packet.atom = Atoms.LookUpAddString (valueString);
			Atoms.RecordToken (result.mac.body, (int) CppEnums.STRCONSTANT, packet);
		}

		public void SetPreambleManually(Profile profile)
		{
			if (profile == Profile.EsProfile)
			{
				DefineAs ("GL_ES", 1);
				DefineAs ("GL_FRAGMENT_PRECISION_HIGH", 1);
				DefineAs ("GL_OES_texture_3D", 1);
				DefineAs ("GL_OES_standard_derivatives", 1);
				DefineAs ("GL_EXT_frag_depth", 1);
				DefineAs ("GL_OES_EGL_image_external", 1);
				DefineAs ("GL_EXT_shader_texture_lod", 1);
				DefineAs ("GL_ANDROID_extension_pack_es31a", 1);
				DefineAs ("GL_KHR_blend_equation_advanced", 1);
				DefineAs ("GL_OES_sample_variables", 1);
				DefineAs ("GL_OES_shader_image_atomic", 1);
				DefineAs ("GL_OES_shader_multisample_interpolation", 1);
				DefineAs ("GL_OES_texture_storage_multisample_2d_array", 1);
				DefineAs ("GL_EXT_geometry_shader", 1);
				DefineAs ("GL_EXT_geometry_point_size", 1);
				DefineAs ("GL_EXT_gpu_shader5", 1);
				DefineAs ("GL_EXT_primitive_bounding_box", 1);
				DefineAs ("GL_EXT_shader_io_blocks", 1);
				DefineAs ("GL_EXT_tessellation_shader", 1);
				DefineAs ("GL_EXT_tessellation_point_size", 1);
				DefineAs ("GL_EXT_texture_buffer", 1);
				DefineAs ("GL_EXT_texture_cube_map_array", 1);
				DefineAs ("GL_OES_geometry_shader", 1);
				DefineAs ("GL_OES_geometry_point_size", 1);
				DefineAs ("GL_OES_gpu_shader5", 1);
				DefineAs ("GL_OES_primitive_bounding_box", 1);
				DefineAs ("GL_OES_shader_io_blocks", 1);
				DefineAs ("GL_OES_tessellation_shader", 1);
				DefineAs ("GL_OES_tessellation_point_size", 1);
				DefineAs ("GL_OES_texture_buffer", 1);
				DefineAs ("GL_OES_texture_cube_map_array", 1);		
			}
			else
			{
				DefineAs ("GL_FRAGMENT_PRECISION_HIGH", 1);
				DefineAs ("GL_ARB_texture_rectangle", 1);
				DefineAs ("GL_ARB_shading_language_420pack", 1);
				DefineAs ("GL_ARB_texture_gather", 1);
				DefineAs ("GL_ARB_gpu_shader5", 1);
				DefineAs ("GL_ARB_separate_shader_objects", 1);
				DefineAs ("GL_ARB_compute_shader", 1);
				DefineAs ("GL_ARB_tessellation_shader", 1);
				DefineAs ("GL_ARB_enhanced_layouts", 1);
				DefineAs ("GL_ARB_texture_cube_map_array", 1);
				DefineAs ("GL_ARB_shader_texture_lod", 1);
				DefineAs ("GL_ARB_explicit_attrib_location", 1);
				DefineAs ("GL_ARB_shader_image_load_store", 1);
				DefineAs ("GL_ARB_shader_atomic_counters", 1);
				DefineAs ("GL_ARB_derivative_control", 1);
				DefineAs ("GL_ARB_shader_texture_image_samples", 1);
				DefineAs ("GL_ARB_viewport_array", 1);
			}
		}
	}
}

