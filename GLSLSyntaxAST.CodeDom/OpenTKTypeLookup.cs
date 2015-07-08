using System;
using System.Collections.Generic;
using OpenTK;

namespace GLSLSyntaxAST.CodeDom
{
	public class OpenTKTypeLookup : IGLSLTypeLookup
	{
		#region IGLSLTypeLookup implementation
		private  Dictionary<string, Type> mClosestTypes;
		public void Initialize ()
		{
			mClosestTypes = new Dictionary<string, Type> ();
			mClosestTypes.Add ("uint", typeof(uint));			
			mClosestTypes.Add ("int", typeof(int));
			mClosestTypes.Add ("float", typeof(float));
			mClosestTypes.Add ("double", typeof(double));
			mClosestTypes.Add ("bool", typeof(bool));
			mClosestTypes.Add ("vec2", typeof(Vector2));
			mClosestTypes.Add ("vec3", typeof(Vector3));
			mClosestTypes.Add ("vec4", typeof(Vector4));
			mClosestTypes.Add ("dvec2", typeof(Vector2d));
			mClosestTypes.Add ("dvec3", typeof(Vector3d));
			mClosestTypes.Add ("dvec4", typeof(Vector4d));
			// TODO: double check
			//			mClosestTypes.Add ("bvec2", typeof(byte));
			//			mClosestTypes.Add ("bvec3", typeof(byte));
			//			mClosestTypes.Add ("bvec4", typeof(byte));
			//			mClosestTypes.Add ("ivec2", typeof(int));
			//			mClosestTypes.Add ("ivec3", typeof(int));
			//			mClosestTypes.Add ("ivec4", typeof(int));
			//			mClosestTypes.Add ("uvec2", typeof(uint));
			//			mClosestTypes.Add ("uvec3", typeof(uint));
			//			mClosestTypes.Add ("uvec4", typeof(uint));

			mClosestTypes.Add ("mat2", typeof(Matrix2));
			mClosestTypes.Add ("mat3", typeof(Matrix3));
			mClosestTypes.Add ("mat4", typeof(Matrix4));

			//mClosestTypes.Add ("mat2x2", typeof(Matrix2));
			mClosestTypes.Add ("mat2x3", typeof(Matrix2x3));
			mClosestTypes.Add ("mat2x4", typeof(Matrix2x4));

			mClosestTypes.Add ("mat3x2", typeof(Matrix3x2));
			mClosestTypes.Add ("mat3x3", typeof(Matrix3));
			mClosestTypes.Add ("mat3x4", typeof(Matrix3x4));

			mClosestTypes.Add ("mat4x2", typeof(Matrix4x2));
			mClosestTypes.Add ("mat4x3", typeof(Matrix4x3));
			mClosestTypes.Add ("mat4x4", typeof(Matrix4));

			mClosestTypes.Add ("dmat2", typeof(Matrix2d));
			mClosestTypes.Add ("dmat3", typeof(Matrix3d));
			mClosestTypes.Add ("dmat4", typeof(Matrix4d));

			//mClosestTypes.Add ("dmat2x2", typeof(Matrix2));
			mClosestTypes.Add ("dmat2x3", typeof(Matrix2x3d));
			mClosestTypes.Add ("dmat2x4", typeof(Matrix2x4d));

			mClosestTypes.Add ("dmat3x2", typeof(Matrix3x2d));
			mClosestTypes.Add ("dmat3x3", typeof(Matrix3d));
			mClosestTypes.Add ("dmat3x4", typeof(Matrix3x4d));

			mClosestTypes.Add ("dmat4x2", typeof(Matrix4x2d));
			mClosestTypes.Add ("dmat4x3", typeof(Matrix4x3d));
			mClosestTypes.Add ("dmat4x4", typeof(Matrix4d));

			// TODO : more types
			//				| atomic_uint
			//				| sampler1d
			//				| sampler2d
			//				| sampler3d
			//				| samplercube
			//				| sampler1dshadow
			//				| sampler2dshadow
			//				| samplercubeshadow
			//				| sampler1darray
			//				| sampler2darray
			//				| sampler1darrayshadow
			//				| sampler2darrayshadow
			//				| samplercubearray
			//				| samplercubearrayshadow
			//				| isampler1d
			//				| isampler2d
			//				| isampler3d
			//				| isamplercube
			//				| isampler1darray
			//				| isampler2darray
			//				| isamplercubearray
			//				| usampler1d
			//				| usampler2d
			//				| usampler3d
			//				| usamplercube
			//				| usampler1darray
			//				| usampler2darray
			//				| usamplercubearray
			//				| sampler2drect
			//				| sampler2drectshadow
			//				| isampler2drect
			//				| usampler2drect
			//				| samplerbuffer
			//				| isamplerbuffer
			//				| usamplerbuffer
			//				| sampler2dms
			//				| isampler2dms
			//				| usampler2dms
			//				| sampler2dmsarray
			//				| isampler2dmsarray
			//				| usampler2dmsarray
			//				| image1d
			//				| iimage1d
			//				| uimage1d
			//				| image2d
			//				| iimage2d
			//				| uimage2d
			//				| image3d
			//				| iimage3d
			//				| uimage3d
			//				| image2drect
			//				| iimage2drect
			//				| uimage2drect
			//				| imagecube
			//				| iimagecube
			//				| uimagecube
			//				| imagebuffer
			//				| iimagebuffer
			//				| uimagebuffer
			//				| image1darray
			//				| iimage1darray
			//				| uimage1darray
			//				| image2darray
			//				| iimage2darray
			//				| uimage2darray
			//				| imagecubearray
			//				| iimagecubearray
			//				| uimagecubearray
			//				| image2dms
			//				| iimage2dms
			//				| uimage2dms
			//				| image2dmsarray
			//				| iimage2dmsarray
			//				| uimage2dmsarray
			//				| samplerexternaloes
		}

		public Type FindClosestType (string typeName)
		{
			Type result = null;
			mClosestTypes.TryGetValue (typeName.ToLowerInvariant (), out result);
			return result;
		}

		#endregion
	}
}

