using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace GLSLSyntaxAST.CodeDom
{
	public class TBuiltins
	{
		public StringBuilder commonBuiltins;
		private Dictionary<TSamplerDim, int> dimMap;
		private Dictionary<TBasicType, string> prefixes;
		private Dictionary<int, string> postfixes;
		public TBuiltins ()
		{
			commonBuiltins = new StringBuilder ();

			// Set up textual representations for making all the permutations
			// of texturing/imaging functions.
			prefixes = new Dictionary<TBasicType, string>();
			prefixes[TBasicType.EbtFloat] =  "";
			prefixes[TBasicType.EbtInt]   = "i";
			prefixes[TBasicType.EbtUint]  = "u";
			postfixes = new Dictionary<int, string> ();
			postfixes[2] = "2";
			postfixes[3] = "3";
			postfixes[4] = "4";

			dimMap = new Dictionary<TSamplerDim, int> ();
			// Map from symbolic class of texturing dimension to numeric dimensions.
			dimMap[TSamplerDim.Esd1D] = 1;
			dimMap[TSamplerDim.Esd2D] = 2;
			dimMap[TSamplerDim.EsdRect] = 2;
			dimMap[TSamplerDim.Esd3D] = 3;
			dimMap[TSamplerDim.EsdCube] = 3;
			dimMap[TSamplerDim.EsdBuffer] = 1;
		}

		public string getCommonString() 
		{
			return commonBuiltins.ToString ();
		}
		/// <summary>
		/// Add all context-independent built-in functions and variables that are present
		/// for the given version and profile.  Share common ones across stages, otherwise
		/// make stage-specific entries.
		/// 
		/// Most built-ins variables can be added as simple text strings.  Some need to
		/// be added programmatically, which is done later in IdentifyBuiltIns() below.
		/// </summary>
		/// <param name="version">Version.</param>
		/// <param name="profile">Profile.</param>
		void initialize(int version, Profile profile)
		{
			//============================================================================
			//
			// Prototypes for built-in functions seen by both vertex and fragment shaders.
			//
			//============================================================================

			//
			// Angle and Trigonometric Functions.
			//
			commonBuiltins.Append(
@"float radians(float degrees);
vec2  radians(vec2  degrees);
vec3  radians(vec3  degrees);
vec4  radians(vec4  degrees);
float degrees(float radians);
vec2  degrees(vec2  radians);
vec3  degrees(vec3  radians);
vec4  degrees(vec4  radians);
float sin(float angle);
vec2  sin(vec2  angle);
vec3  sin(vec3  angle);
vec4  sin(vec4  angle);
float cos(float angle);
vec2  cos(vec2  angle);
vec3  cos(vec3  angle);
vec4  cos(vec4  angle);
float tan(float angle);
vec2  tan(vec2  angle);
vec3  tan(vec3  angle);
vec4  tan(vec4  angle);
float asin(float x);
vec2  asin(vec2  x);
vec3  asin(vec3  x);
vec4  asin(vec4  x);
float acos(float x);
vec2  acos(vec2  x);
vec3  acos(vec3  x);
vec4  acos(vec4  x);
float atan(float y, float x);
vec2  atan(vec2  y, vec2  x);
vec3  atan(vec3  y, vec3  x);
vec4  atan(vec4  y, vec4  x);
float atan(float y_over_x);
vec2  atan(vec2  y_over_x);
vec3  atan(vec3  y_over_x);
vec4  atan(vec4  y_over_x);
");

			if (version >= 130) {
				commonBuiltins.Append(
@"float sinh(float angle);
vec2  sinh(vec2  angle);
vec3  sinh(vec3  angle);
vec4  sinh(vec4  angle);
float cosh(float angle);
vec2  cosh(vec2  angle);
vec3  cosh(vec3  angle);
vec4  cosh(vec4  angle);
float tanh(float angle);
vec2  tanh(vec2  angle);
vec3  tanh(vec3  angle);
vec4  tanh(vec4  angle);
float asinh(float x);
vec2  asinh(vec2  x);
vec3  asinh(vec3  x);
vec4  asinh(vec4  x);
float acosh(float x);
vec2  acosh(vec2  x);
vec3  acosh(vec3  x);
vec4  acosh(vec4  x);
float atanh(float y_over_x);
vec2  atanh(vec2  y_over_x);
vec3  atanh(vec3  y_over_x);
vec4  atanh(vec4  y_over_x);
");
			}

			//
			// Exponential Functions.
			//
			commonBuiltins.Append(
@"float pow(float x, float y);
vec2  pow(vec2  x, vec2  y);
vec3  pow(vec3  x, vec3  y);
vec4  pow(vec4  x, vec4  y);
float exp(float x);
vec2  exp(vec2  x);
vec3  exp(vec3  x);
vec4  exp(vec4  x);
float log(float x);
vec2  log(vec2  x);
vec3  log(vec3  x);
vec4  log(vec4  x);
float exp2(float x);
vec2  exp2(vec2  x);
vec3  exp2(vec3  x);
vec4  exp2(vec4  x);
float log2(float x);
vec2  log2(vec2  x);
vec3  log2(vec3  x);
vec4  log2(vec4  x);
float sqrt(float x);
vec2  sqrt(vec2  x);
vec3  sqrt(vec3  x);
vec4  sqrt(vec4  x);
float inversesqrt(float x);
vec2  inversesqrt(vec2  x);
vec3  inversesqrt(vec3  x);
vec4  inversesqrt(vec4  x);
");

			//
			// Common Functions.
			//
			commonBuiltins.Append(
@"float abs(float x);
vec2  abs(vec2  x);
vec3  abs(vec3  x);
vec4  abs(vec4  x);
float sign(float x);
vec2  sign(vec2  x);
vec3  sign(vec3  x);
vec4  sign(vec4  x);
float floor(float x);
vec2  floor(vec2  x);
vec3  floor(vec3  x);
vec4  floor(vec4  x);
float ceil(float x);
vec2  ceil(vec2  x);
vec3  ceil(vec3  x);
vec4  ceil(vec4  x);
float fract(float x);
vec2  fract(vec2  x);
vec3  fract(vec3  x);
vec4  fract(vec4  x);
float mod(float x, float y);
vec2  mod(vec2  x, float y);
vec3  mod(vec3  x, float y);
vec4  mod(vec4  x, float y);
vec2  mod(vec2  x, vec2  y);
vec3  mod(vec3  x, vec3  y);
vec4  mod(vec4  x, vec4  y);
float min(float x, float y);
vec2  min(vec2  x, float y);
vec3  min(vec3  x, float y);
vec4  min(vec4  x, float y);
vec2  min(vec2  x, vec2  y);
vec3  min(vec3  x, vec3  y);
vec4  min(vec4  x, vec4  y);
float max(float x, float y);
vec2  max(vec2  x, float y);
vec3  max(vec3  x, float y);
vec4  max(vec4  x, float y);
vec2  max(vec2  x, vec2  y);
vec3  max(vec3  x, vec3  y);
vec4  max(vec4  x, vec4  y);
float clamp(float x, float minVal, float maxVal);
vec2  clamp(vec2  x, float minVal, float maxVal);
vec3  clamp(vec3  x, float minVal, float maxVal);
vec4  clamp(vec4  x, float minVal, float maxVal);
vec2  clamp(vec2  x, vec2  minVal, vec2  maxVal);
vec3  clamp(vec3  x, vec3  minVal, vec3  maxVal);
vec4  clamp(vec4  x, vec4  minVal, vec4  maxVal);
float mix(float x, float y, float a);
vec2  mix(vec2  x, vec2  y, float a);
vec3  mix(vec3  x, vec3  y, float a);
vec4  mix(vec4  x, vec4  y, float a);
vec2  mix(vec2  x, vec2  y, vec2  a);
vec3  mix(vec3  x, vec3  y, vec3  a);
vec4  mix(vec4  x, vec4  y, vec4  a);
float step(float edge, float x);
vec2  step(vec2  edge, vec2  x);
vec3  step(vec3  edge, vec3  x);
vec4  step(vec4  edge, vec4  x);
vec2  step(float edge, vec2  x);
vec3  step(float edge, vec3  x);
vec4  step(float edge, vec4  x);
float smoothstep(float edge0, float edge1, float x);
vec2  smoothstep(vec2  edge0, vec2  edge1, vec2  x);
vec3  smoothstep(vec3  edge0, vec3  edge1, vec3  x);
vec4  smoothstep(vec4  edge0, vec4  edge1, vec4  x);
vec2  smoothstep(float edge0, float edge1, vec2  x);
vec3  smoothstep(float edge0, float edge1, vec3  x);
vec4  smoothstep(float edge0, float edge1, vec4  x);
");

			if (version >= 130) {
				commonBuiltins.Append(
@"  int abs(  int x);
ivec2 abs(ivec2 x);
ivec3 abs(ivec3 x);
ivec4 abs(ivec4 x);
  int sign(  int x);
ivec2 sign(ivec2 x);
ivec3 sign(ivec3 x);
ivec4 sign(ivec4 x);
float trunc(float x);
vec2  trunc(vec2  x);
vec3  trunc(vec3  x);
vec4  trunc(vec4  x);
float round(float x);
vec2  round(vec2  x);
vec3  round(vec3  x);
vec4  round(vec4  x);
float roundEven(float x);
vec2  roundEven(vec2  x);
vec3  roundEven(vec3  x);
vec4  roundEven(vec4  x);
float modf(float, out float);
vec2  modf(vec2,  out vec2 );
vec3  modf(vec3,  out vec3 );
vec4  modf(vec4,  out vec4 );
  int min(int    x, int y);
ivec2 min(ivec2  x, int y);
ivec3 min(ivec3  x, int y);
ivec4 min(ivec4  x, int y);
ivec2 min(ivec2  x, ivec2  y);
ivec3 min(ivec3  x, ivec3  y);
ivec4 min(ivec4  x, ivec4  y);
 uint min(uint   x, uint y);
uvec2 min(uvec2  x, uint y);
uvec3 min(uvec3  x, uint y);
uvec4 min(uvec4  x, uint y);
uvec2 min(uvec2  x, uvec2  y);
uvec3 min(uvec3  x, uvec3  y);
uvec4 min(uvec4  x, uvec4  y);
  int max(int    x, int y);
ivec2 max(ivec2  x, int y);
ivec3 max(ivec3  x, int y);
ivec4 max(ivec4  x, int y);
ivec2 max(ivec2  x, ivec2  y);
ivec3 max(ivec3  x, ivec3  y);
ivec4 max(ivec4  x, ivec4  y);
 uint max(uint   x, uint y);
uvec2 max(uvec2  x, uint y);
uvec3 max(uvec3  x, uint y);
uvec4 max(uvec4  x, uint y);
uvec2 max(uvec2  x, uvec2  y);
uvec3 max(uvec3  x, uvec3  y);
uvec4 max(uvec4  x, uvec4  y);
int    clamp(int x, int minVal, int maxVal);
ivec2  clamp(ivec2  x, int minVal, int maxVal);
ivec3  clamp(ivec3  x, int minVal, int maxVal);
ivec4  clamp(ivec4  x, int minVal, int maxVal);
ivec2  clamp(ivec2  x, ivec2  minVal, ivec2  maxVal);
ivec3  clamp(ivec3  x, ivec3  minVal, ivec3  maxVal);
ivec4  clamp(ivec4  x, ivec4  minVal, ivec4  maxVal);
uint   clamp(uint x, uint minVal, uint maxVal);
uvec2  clamp(uvec2  x, uint minVal, uint maxVal);
uvec3  clamp(uvec3  x, uint minVal, uint maxVal);
uvec4  clamp(uvec4  x, uint minVal, uint maxVal);
uvec2  clamp(uvec2  x, uvec2  minVal, uvec2  maxVal);
uvec3  clamp(uvec3  x, uvec3  minVal, uvec3  maxVal);
uvec4  clamp(uvec4  x, uvec4  minVal, uvec4  maxVal);
float mix(float x, float y, bool  a);
vec2  mix(vec2  x, vec2  y, bvec2 a);
vec3  mix(vec3  x, vec3  y, bvec3 a);
vec4  mix(vec4  x, vec4  y, bvec4 a);
bool  isnan(float x);
bvec2 isnan(vec2  x);
bvec3 isnan(vec3  x);
bvec4 isnan(vec4  x);
bool  isinf(float x);
bvec2 isinf(vec2  x);
bvec3 isinf(vec3  x);
bvec4 isinf(vec4  x);
");
			}

			if ((profile == Profile.EsProfile && version >= 310) ||
				(profile != Profile.EsProfile && version >= 430)) {
				commonBuiltins.Append(
@"uint atomicAdd(coherent volatile inout uint, uint);
 int atomicAdd(coherent volatile inout  int,  int);
uint atomicMin(coherent volatile inout uint, uint);
 int atomicMin(coherent volatile inout  int,  int);
uint atomicMax(coherent volatile inout uint, uint);
 int atomicMax(coherent volatile inout  int,  int);
uint atomicAnd(coherent volatile inout uint, uint);
 int atomicAnd(coherent volatile inout  int,  int);
uint atomicOr (coherent volatile inout uint, uint);
 int atomicOr (coherent volatile inout  int,  int);
uint atomicXor(coherent volatile inout uint, uint);
 int atomicXor(coherent volatile inout  int,  int);
uint atomicExchange(coherent volatile inout uint, uint);
 int atomicExchange(coherent volatile inout  int,  int);
uint atomicCompSwap(coherent volatile inout uint, uint, uint);
 int atomicCompSwap(coherent volatile inout  int,  int,  int);
");
			}

			if ((profile == Profile.EsProfile && version >= 310) ||
				(profile != Profile.EsProfile && version >= 450)) {
				commonBuiltins.Append(
@"int    mix(int    x, int    y, bool  a);
ivec2  mix(ivec2  x, ivec2  y, bvec2 a);
ivec3  mix(ivec3  x, ivec3  y, bvec3 a);
ivec4  mix(ivec4  x, ivec4  y, bvec4 a);
uint   mix(uint   x, uint   y, bool  a);
uvec2  mix(uvec2  x, uvec2  y, bvec2 a);
uvec3  mix(uvec3  x, uvec3  y, bvec3 a);
uvec4  mix(uvec4  x, uvec4  y, bvec4 a);
bool   mix(bool   x, bool   y, bool  a);
bvec2  mix(bvec2  x, bvec2  y, bvec2 a);
bvec3  mix(bvec3  x, bvec3  y, bvec3 a);
bvec4  mix(bvec4  x, bvec4  y, bvec4 a);
");
			}

			if ((profile == Profile.EsProfile && version >= 300) ||
				(profile != Profile.EsProfile && version >= 330)) {
				commonBuiltins.Append(
@"int   floatBitsToInt(float value);
ivec2 floatBitsToInt(vec2  value);
ivec3 floatBitsToInt(vec3  value);
ivec4 floatBitsToInt(vec4  value);
uint  floatBitsToUint(float value);
uvec2 floatBitsToUint(vec2  value);
uvec3 floatBitsToUint(vec3  value);
uvec4 floatBitsToUint(vec4  value);
float intBitsToFloat(int   value);
vec2  intBitsToFloat(ivec2 value);
vec3  intBitsToFloat(ivec3 value);
vec4  intBitsToFloat(ivec4 value);
float uintBitsToFloat(uint  value);
vec2  uintBitsToFloat(uvec2 value);
vec3  uintBitsToFloat(uvec3 value);
vec4  uintBitsToFloat(uvec4 value);
");
			}

			if (profile != Profile.EsProfile && version >= 400) {
				commonBuiltins.Append(
@"float  fma(float,  float,  float );
vec2   fma(vec2,   vec2,   vec2  );
vec3   fma(vec3,   vec3,   vec3  );
vec4   fma(vec4,   vec4,   vec4  );
double fma(double, double, double);
dvec2  fma(dvec2,  dvec2,  dvec2 );
dvec3  fma(dvec3,  dvec3,  dvec3 );
dvec4  fma(dvec4,  dvec4,  dvec4 );
");
			}

			if ((profile == Profile.EsProfile && version >= 310) ||
				(profile != Profile.EsProfile && version >= 400)) {
				commonBuiltins.Append(
@"highp float frexp(highp float, out highp int);
highp vec2  frexp(highp vec2,  out highp ivec2);
highp vec3  frexp(highp vec3,  out highp ivec3);
highp vec4  frexp(highp vec4,  out highp ivec4);
highp float ldexp(highp float, highp int);
highp vec2  ldexp(highp vec2,  highp ivec2);
highp vec3  ldexp(highp vec3,  highp ivec3);
highp vec4  ldexp(highp vec4,  highp ivec4);
");
			}

			if (profile != Profile.EsProfile && version >= 400) {
				commonBuiltins.Append(
@"double frexp(double, out int);
dvec2  frexp( dvec2, out ivec2);
dvec3  frexp( dvec3, out ivec3);
dvec4  frexp( dvec4, out ivec4);
double ldexp(double, int);
dvec2  ldexp( dvec2, ivec2);
dvec3  ldexp( dvec3, ivec3);
dvec4  ldexp( dvec4, ivec4);
double packDouble2x32(uvec2);
uvec2 unpackDouble2x32(double);
");
			}

			if ((profile == Profile.EsProfile && version >= 300) ||
				(profile != Profile.EsProfile && version >= 400)) {
				commonBuiltins.Append(
@"highp uint packUnorm2x16(vec2);
highp vec2 unpackUnorm2x16(highp uint);
");
			}

			if ((profile == Profile.EsProfile && version >= 300) ||
				(profile != Profile.EsProfile && version >= 420)) {
				commonBuiltins.Append(
@"highp uint packSnorm2x16(vec2);
highp vec2 unpackSnorm2x16(highp uint);
highp uint packHalf2x16(mediump vec2);
mediump vec2 unpackHalf2x16(highp uint);
");
			}

			if ((profile == Profile.EsProfile && version >= 310) ||
				(profile != Profile.EsProfile && version >= 400)) {
				commonBuiltins.Append(
@"highp   uint packSnorm4x8  (mediump vec4);
mediump vec4 unpackSnorm4x8(highp   uint);
highp   uint packUnorm4x8  (mediump vec4);
mediump vec4 unpackUnorm4x8(highp   uint);
");
			}

			//
			// Geometric Functions.
			//
			commonBuiltins.Append(
@"float length(float x);
float length(vec2  x);
float length(vec3  x);
float length(vec4  x);
float distance(float p0, float p1);
float distance(vec2  p0, vec2  p1);
float distance(vec3  p0, vec3  p1);
float distance(vec4  p0, vec4  p1);
float dot(float x, float y);
float dot(vec2  x, vec2  y);
float dot(vec3  x, vec3  y);
float dot(vec4  x, vec4  y);
vec3 cross(vec3 x, vec3 y);
float normalize(float x);
vec2  normalize(vec2  x);
vec3  normalize(vec3  x);
vec4  normalize(vec4  x);
float faceforward(float N, float I, float Nref);
vec2  faceforward(vec2  N, vec2  I, vec2  Nref);
vec3  faceforward(vec3  N, vec3  I, vec3  Nref);
vec4  faceforward(vec4  N, vec4  I, vec4  Nref);
float reflect(float I, float N);
vec2  reflect(vec2  I, vec2  N);
vec3  reflect(vec3  I, vec3  N);
vec4  reflect(vec4  I, vec4  N);
float refract(float I, float N, float eta);
vec2  refract(vec2  I, vec2  N, float eta);
vec3  refract(vec3  I, vec3  N, float eta);
vec4  refract(vec4  I, vec4  N, float eta);
");

			//
			// Matrix Functions.
			//
			commonBuiltins.Append(
@"mat2 matrixCompMult(mat2 x, mat2 y);
mat3 matrixCompMult(mat3 x, mat3 y);
mat4 matrixCompMult(mat4 x, mat4 y);
");

			// 120 is correct for both ES and desktop
			if (version >= 120) {
				commonBuiltins.Append(
@"mat2   outerProduct(vec2 c, vec2 r);
mat3   outerProduct(vec3 c, vec3 r);
mat4   outerProduct(vec4 c, vec4 r);
mat2x3 outerProduct(vec3 c, vec2 r);
mat3x2 outerProduct(vec2 c, vec3 r);
mat2x4 outerProduct(vec4 c, vec2 r);
mat4x2 outerProduct(vec2 c, vec4 r);
mat3x4 outerProduct(vec4 c, vec3 r);
mat4x3 outerProduct(vec3 c, vec4 r);
mat2   transpose(mat2   m);
mat3   transpose(mat3   m);
mat4   transpose(mat4   m);
mat2x3 transpose(mat3x2 m);
mat3x2 transpose(mat2x3 m);
mat2x4 transpose(mat4x2 m);
mat4x2 transpose(mat2x4 m);
mat3x4 transpose(mat4x3 m);
mat4x3 transpose(mat3x4 m);
mat2x3 matrixCompMult(mat2x3, mat2x3);
mat2x4 matrixCompMult(mat2x4, mat2x4);
mat3x2 matrixCompMult(mat3x2, mat3x2);
mat3x4 matrixCompMult(mat3x4, mat3x4);
mat4x2 matrixCompMult(mat4x2, mat4x2);
mat4x3 matrixCompMult(mat4x3, mat4x3);
");

				// 150 is correct for both ES and desktop
				if (version >= 150) {
					commonBuiltins.Append(
@"float determinant(mat2 m);
float determinant(mat3 m);
float determinant(mat4 m);
mat2 inverse(mat2 m);
mat3 inverse(mat3 m);
mat4 inverse(mat4 m);
");
				}
			}

			//
			// Vector relational functions.
			//
			commonBuiltins.Append(
@"bvec2 lessThan(vec2 x, vec2 y);
bvec3 lessThan(vec3 x, vec3 y);
bvec4 lessThan(vec4 x, vec4 y);
bvec2 lessThan(ivec2 x, ivec2 y);
bvec3 lessThan(ivec3 x, ivec3 y);
bvec4 lessThan(ivec4 x, ivec4 y);
bvec2 lessThanEqual(vec2 x, vec2 y);
bvec3 lessThanEqual(vec3 x, vec3 y);
bvec4 lessThanEqual(vec4 x, vec4 y);
bvec2 lessThanEqual(ivec2 x, ivec2 y);
bvec3 lessThanEqual(ivec3 x, ivec3 y);
bvec4 lessThanEqual(ivec4 x, ivec4 y);
bvec2 greaterThan(vec2 x, vec2 y);
bvec3 greaterThan(vec3 x, vec3 y);
bvec4 greaterThan(vec4 x, vec4 y);
bvec2 greaterThan(ivec2 x, ivec2 y);
bvec3 greaterThan(ivec3 x, ivec3 y);
bvec4 greaterThan(ivec4 x, ivec4 y);
bvec2 greaterThanEqual(vec2 x, vec2 y);
bvec3 greaterThanEqual(vec3 x, vec3 y);
bvec4 greaterThanEqual(vec4 x, vec4 y);
bvec2 greaterThanEqual(ivec2 x, ivec2 y);
bvec3 greaterThanEqual(ivec3 x, ivec3 y);
bvec4 greaterThanEqual(ivec4 x, ivec4 y);
bvec2 equal(vec2 x, vec2 y);
bvec3 equal(vec3 x, vec3 y);
bvec4 equal(vec4 x, vec4 y);
bvec2 equal(ivec2 x, ivec2 y);
bvec3 equal(ivec3 x, ivec3 y);
bvec4 equal(ivec4 x, ivec4 y);
bvec2 equal(bvec2 x, bvec2 y);
bvec3 equal(bvec3 x, bvec3 y);
bvec4 equal(bvec4 x, bvec4 y);
bvec2 notEqual(vec2 x, vec2 y);
bvec3 notEqual(vec3 x, vec3 y);
bvec4 notEqual(vec4 x, vec4 y);
bvec2 notEqual(ivec2 x, ivec2 y);
bvec3 notEqual(ivec3 x, ivec3 y);
bvec4 notEqual(ivec4 x, ivec4 y);
bvec2 notEqual(bvec2 x, bvec2 y);
bvec3 notEqual(bvec3 x, bvec3 y);
bvec4 notEqual(bvec4 x, bvec4 y);
bool any(bvec2 x);
bool any(bvec3 x);
bool any(bvec4 x);
bool all(bvec2 x);
bool all(bvec3 x);
bool all(bvec4 x);
bvec2 not(bvec2 x);
bvec3 not(bvec3 x);
bvec4 not(bvec4 x);
");

			if (version >= 130) {
				commonBuiltins.Append(
@"bvec2 lessThan(uvec2 x, uvec2 y);
bvec3 lessThan(uvec3 x, uvec3 y);
bvec4 lessThan(uvec4 x, uvec4 y);
bvec2 lessThanEqual(uvec2 x, uvec2 y);
bvec3 lessThanEqual(uvec3 x, uvec3 y);
bvec4 lessThanEqual(uvec4 x, uvec4 y);
bvec2 greaterThan(uvec2 x, uvec2 y);
bvec3 greaterThan(uvec3 x, uvec3 y);
bvec4 greaterThan(uvec4 x, uvec4 y);
bvec2 greaterThanEqual(uvec2 x, uvec2 y);
bvec3 greaterThanEqual(uvec3 x, uvec3 y);
bvec4 greaterThanEqual(uvec4 x, uvec4 y);
bvec2 equal(uvec2 x, uvec2 y);
bvec3 equal(uvec3 x, uvec3 y);
bvec4 equal(uvec4 x, uvec4 y);
bvec2 notEqual(uvec2 x, uvec2 y);
bvec3 notEqual(uvec3 x, uvec3 y);
bvec4 notEqual(uvec4 x, uvec4 y);
");
			}

			//
			// Original-style texture functions existing in all stages.
			// (Per-stage functions below.)
			//
			if ((profile == Profile.EsProfile && version == 100) ||
				profile == Profile.CompatibilityProfile ||
				(profile == Profile.CoreProfile && version < 420) ||
				profile == Profile.NoProfile) {
				commonBuiltins.Append(
@"vec4 texture2D(sampler2D, vec2);
vec4 texture2DProj(sampler2D, vec3);
vec4 texture2DProj(sampler2D, vec4);
vec4 texture3D(sampler3D, vec3);
vec4 texture3DProj(sampler3D, vec4);
vec4 textureCube(samplerCube, vec3);
");
			}

			if ( profile == Profile.CompatibilityProfile ||
				(profile == Profile.CoreProfile && version < 420) ||
				profile == Profile.NoProfile) {
				commonBuiltins.Append(
@"vec4 texture1D(sampler1D, float);
vec4 texture1DProj(sampler1D, vec2);
vec4 texture1DProj(sampler1D, vec4);
vec4 shadow1D(sampler1DShadow, vec3);
vec4 shadow2D(sampler2DShadow, vec3);
vec4 shadow1DProj(sampler1DShadow, vec4);
vec4 shadow2DProj(sampler2DShadow, vec4);
vec4 texture2DRect(sampler2DRect, vec2);
vec4 texture2DRectProj(sampler2DRect, vec3);
vec4 texture2DRectProj(sampler2DRect, vec4);
vec4 shadow2DRect(sampler2DRectShadow, vec3);
vec4 shadow2DRectProj(sampler2DRectShadow, vec4);
");
			}

			if (profile == Profile.EsProfile) {        
				commonBuiltins.Append(
@"vec4 texture2D(samplerExternalOES, vec2 coord);
vec4 texture2DProj(samplerExternalOES, vec3);
vec4 texture2DProj(samplerExternalOES, vec4);
vec4 texture2DGradEXT(sampler2D, vec2, vec2, vec2);
vec4 texture2DProjGradEXT(sampler2D, vec3, vec2, vec2);
vec4 texture2DProjGradEXT(sampler2D, vec4, vec2, vec2);
vec4 textureCubeGradEXT(samplerCube, vec3, vec3, vec3);
");
			}

			//
			// Noise functions.
			//
			if (profile != Profile.EsProfile) {
				commonBuiltins.Append(
@"float noise1(float x);
float noise1(vec2  x);
float noise1(vec3  x);
float noise1(vec4  x);
vec2 noise2(float x);
vec2 noise2(vec2  x);
vec2 noise2(vec3  x);
vec2 noise2(vec4  x);
vec3 noise3(float x);
vec3 noise3(vec2  x);
vec3 noise3(vec3  x);
vec3 noise3(vec4  x);
vec4 noise4(float x);
vec4 noise4(vec2  x);
vec4 noise4(vec3  x);
vec4 noise4(vec4  x);
");
			}

			//
			// Atomic counter functions.
			//
			if ((profile != Profile.EsProfile && version >= 300) ||
				(profile == Profile.EsProfile && version >= 310)) {
				commonBuiltins.Append(
@"uint atomicCounterIncrement(atomic_uint x);
uint atomicCounterDecrement(atomic_uint x);
uint atomicCounter(atomic_uint x);
");
			}

			// Bitfield
			if ((profile == Profile.EsProfile && version >= 310) ||
				(profile != Profile.EsProfile && version >= 400)) {
				commonBuiltins.Append(
@" uint uaddCarry( uint,  uint, out  uint carry);
uvec2 uaddCarry(uvec2, uvec2, out uvec2 carry);
uvec3 uaddCarry(uvec3, uvec3, out uvec3 carry);
uvec4 uaddCarry(uvec4, uvec4, out uvec4 carry);
 uint usubBorrow( uint,  uint, out  uint borrow);
uvec2 usubBorrow(uvec2, uvec2, out uvec2 borrow);
uvec3 usubBorrow(uvec3, uvec3, out uvec3 borrow);
uvec4 usubBorrow(uvec4, uvec4, out uvec4 borrow);
void umulExtended( uint,  uint, out  uint, out  uint lsb);
void umulExtended(uvec2, uvec2, out uvec2, out uvec2 lsb);
void umulExtended(uvec3, uvec3, out uvec3, out uvec3 lsb);
void umulExtended(uvec4, uvec4, out uvec4, out uvec4 lsb);
void imulExtended(  int,   int, out   int, out   int lsb);
void imulExtended(ivec2, ivec2, out ivec2, out ivec2 lsb);
void imulExtended(ivec3, ivec3, out ivec3, out ivec3 lsb);
void imulExtended(ivec4, ivec4, out ivec4, out ivec4 lsb);
  int bitfieldExtract(  int, int, int);
ivec2 bitfieldExtract(ivec2, int, int);
ivec3 bitfieldExtract(ivec3, int, int);
ivec4 bitfieldExtract(ivec4, int, int);
 uint bitfieldExtract( uint, int, int);
uvec2 bitfieldExtract(uvec2, int, int);
uvec3 bitfieldExtract(uvec3, int, int);
uvec4 bitfieldExtract(uvec4, int, int);
  int bitfieldInsert(  int base,   int, int, int);
ivec2 bitfieldInsert(ivec2 base, ivec2, int, int);
ivec3 bitfieldInsert(ivec3 base, ivec3, int, int);
ivec4 bitfieldInsert(ivec4 base, ivec4, int, int);
 uint bitfieldInsert( uint base,  uint, int, int);
uvec2 bitfieldInsert(uvec2 base, uvec2, int, int);
uvec3 bitfieldInsert(uvec3 base, uvec3, int, int);
uvec4 bitfieldInsert(uvec4 base, uvec4, int, int);
  int bitfieldReverse(  int);
ivec2 bitfieldReverse(ivec2);
ivec3 bitfieldReverse(ivec3);
ivec4 bitfieldReverse(ivec4);
 uint bitfieldReverse( uint);
uvec2 bitfieldReverse(uvec2);
uvec3 bitfieldReverse(uvec3);
uvec4 bitfieldReverse(uvec4);
  int bitCount(  int);
ivec2 bitCount(ivec2);
ivec3 bitCount(ivec3);
ivec4 bitCount(ivec4);
  int bitCount( uint);
ivec2 bitCount(uvec2);
ivec3 bitCount(uvec3);
ivec4 bitCount(uvec4);
  int findLSB(  int);
ivec2 findLSB(ivec2);
ivec3 findLSB(ivec3);
ivec4 findLSB(ivec4);
  int findLSB( uint);
ivec2 findLSB(uvec2);
ivec3 findLSB(uvec3);
ivec4 findLSB(uvec4);
  int findMSB(  int);
ivec2 findMSB(ivec2);
ivec3 findMSB(ivec3);
ivec4 findMSB(ivec4);
  int findMSB( uint);
ivec2 findMSB(uvec2);
ivec3 findMSB(uvec3);
ivec4 findMSB(uvec4);
");
			}

			//============================================================================
			//
			// Prototypes for built-in functions seen by vertex shaders only.
			// (Except legacy lod functions, where it depends which release they are
			// vertex only.)
			//
			//============================================================================

			//
			// Geometric Functions.
			//
			if (IncludeLegacy(version, profile))
				stageBuiltins[EShLangVertex].append("vec4 ftransform();");

			//
			// Original-style texture Functions with lod.
			//
			TString* s;
			if (version == 100)
				s = &stageBuiltins[EShLangVertex];
			else
				s = &commonBuiltins;
			if ((profile == Profile.EsProfile && version == 100) ||
				profile == Profile.CompatibilityProfile ||
				(profile == Profile.CoreProfile && version < 420) ||
				profile == Profile.NoProfile) {             
				s->append(
					"vec4 texture2DLod(sampler2D, vec2, float);"         // GL_ARB_shader_texture_lod
					"vec4 texture2DProjLod(sampler2D, vec3, float);"     // GL_ARB_shader_texture_lod
					"vec4 texture2DProjLod(sampler2D, vec4, float);"     // GL_ARB_shader_texture_lod
					"vec4 texture3DLod(sampler3D, vec3, float);"         // GL_ARB_shader_texture_lod  // OES_texture_3D, but caught by keyword check
					"vec4 texture3DProjLod(sampler3D, vec4, float);"     // GL_ARB_shader_texture_lod  // OES_texture_3D, but caught by keyword check
					"vec4 textureCubeLod(samplerCube, vec3, float);"     // GL_ARB_shader_texture_lod

					"\n");
			}
			if ( profile == Profile.CompatibilityProfile ||
				(profile == Profile.CoreProfile && version < 420) ||
				profile == Profile.NoProfile) {
				s->append(
					"vec4 texture1DLod(sampler1D, float, float);"                          // GL_ARB_shader_texture_lod
					"vec4 texture1DProjLod(sampler1D, vec2, float);"                       // GL_ARB_shader_texture_lod
					"vec4 texture1DProjLod(sampler1D, vec4, float);"                       // GL_ARB_shader_texture_lod
					"vec4 shadow1DLod(sampler1DShadow, vec3, float);"                      // GL_ARB_shader_texture_lod
					"vec4 shadow2DLod(sampler2DShadow, vec3, float);"                      // GL_ARB_shader_texture_lod
					"vec4 shadow1DProjLod(sampler1DShadow, vec4, float);"                  // GL_ARB_shader_texture_lod
					"vec4 shadow2DProjLod(sampler2DShadow, vec4, float);"                  // GL_ARB_shader_texture_lod

					"vec4 texture1DGradARB(sampler1D, float, float, float);"               // GL_ARB_shader_texture_lod
					"vec4 texture1DProjGradARB(sampler1D, vec2, float, float);"            // GL_ARB_shader_texture_lod
					"vec4 texture1DProjGradARB(sampler1D, vec4, float, float);"            // GL_ARB_shader_texture_lod
					"vec4 texture2DGradARB(sampler2D, vec2, vec2, vec2);"                  // GL_ARB_shader_texture_lod
					"vec4 texture2DProjGradARB(sampler2D, vec3, vec2, vec2);"              // GL_ARB_shader_texture_lod
					"vec4 texture2DProjGradARB(sampler2D, vec4, vec2, vec2);"              // GL_ARB_shader_texture_lod
					"vec4 texture3DGradARB(sampler3D, vec3, vec3, vec3);"                  // GL_ARB_shader_texture_lod
					"vec4 texture3DProjGradARB(sampler3D, vec4, vec3, vec3);"              // GL_ARB_shader_texture_lod
					"vec4 textureCubeGradARB(samplerCube, vec3, vec3, vec3);"              // GL_ARB_shader_texture_lod
					"vec4 shadow1DGradARB(sampler1DShadow, vec3, float, float);"           // GL_ARB_shader_texture_lod
					"vec4 shadow1DProjGradARB( sampler1DShadow, vec4, float, float);"      // GL_ARB_shader_texture_lod
					"vec4 shadow2DGradARB(sampler2DShadow, vec3, vec2, vec2);"             // GL_ARB_shader_texture_lod
					"vec4 shadow2DProjGradARB( sampler2DShadow, vec4, vec2, vec2);"        // GL_ARB_shader_texture_lod
					"vec4 texture2DRectGradARB(sampler2DRect, vec2, vec2, vec2);"          // GL_ARB_shader_texture_lod
					"vec4 texture2DRectProjGradARB( sampler2DRect, vec3, vec2, vec2);"     // GL_ARB_shader_texture_lod
					"vec4 texture2DRectProjGradARB( sampler2DRect, vec4, vec2, vec2);"     // GL_ARB_shader_texture_lod
					"vec4 shadow2DRectGradARB( sampler2DRectShadow, vec3, vec2, vec2);"    // GL_ARB_shader_texture_lod
					"vec4 shadow2DRectProjGradARB(sampler2DRectShadow, vec4, vec2, vec2);" // GL_ARB_shader_texture_lod

					"\n");
			}

			if ((profile != Profile.EsProfile && version >= 150) ||
				(profile == Profile.EsProfile && version >= 310)) {
				//============================================================================
				//
				// Prototypes for built-in functions seen by geometry shaders only.
				//
				//============================================================================

				if (profile != Profile.EsProfile && version >= 400) {
					stageBuiltins[EShLangGeometry].append(
						"void EmitStreamVertex(int);"
						"void EndStreamPrimitive(int);"
					);
				}
				stageBuiltins[EShLangGeometry].append(
					"void EmitVertex();"
					"void EndPrimitive();"
					"\n");
			}

			//============================================================================
			//
			// Prototypes for all control functions.
			//
			//============================================================================
			bool esBarrier = (profile == Profile.EsProfile && version >= 310);
			if (profile != Profile.EsProfile && version >= 150 || esBarrier)
				stageBuiltins[EShLangTessControl].append(
					"void barrier();"
				);
			if ((profile != Profile.EsProfile && version >= 430) || esBarrier)
				stageBuiltins[EShLangCompute].append(
					"void barrier();"
				);
			if ((profile != Profile.EsProfile && version >= 130) || esBarrier)
				commonBuiltins.Append(
					"void memoryBarrier();"
				);
			if ((profile != Profile.EsProfile && version >= 430) || esBarrier) {
				commonBuiltins.Append(
@"void memoryBarrierAtomicCounter();
void memoryBarrierBuffer();
void memoryBarrierImage();"
				);
				stageBuiltins[EShLangCompute].append(
					"void memoryBarrierShared();"
					"void groupMemoryBarrier();"
				);
			}

			//============================================================================
			//
			// Prototypes for built-in functions seen by fragment shaders only.
			//
			//============================================================================

			//
			// Original-style texture Functions with bias.
			//
			if (profile != EEsProfile || version == 100) {
				stageBuiltins[EShLangFragment].append(
					"vec4 texture2D(sampler2D, vec2, float);"
					"vec4 texture2DProj(sampler2D, vec3, float);"
					"vec4 texture2DProj(sampler2D, vec4, float);"
					"vec4 texture3D(sampler3D, vec3, float);"        // OES_texture_3D
					"vec4 texture3DProj(sampler3D, vec4, float);"    // OES_texture_3D
					"vec4 textureCube(samplerCube, vec3, float);"

					"\n");
			}
			if (profile != EEsProfile && version > 100) {
				stageBuiltins[EShLangFragment].append(
					"vec4 texture1D(sampler1D, float, float);"
					"vec4 texture1DProj(sampler1D, vec2, float);"
					"vec4 texture1DProj(sampler1D, vec4, float);"
					"vec4 shadow1D(sampler1DShadow, vec3, float);"
					"vec4 shadow2D(sampler2DShadow, vec3, float);"
					"vec4 shadow1DProj(sampler1DShadow, vec4, float);"
					"vec4 shadow2DProj(sampler2DShadow, vec4, float);"

					"\n");
			}
			if (profile == EEsProfile) {
				stageBuiltins[EShLangFragment].append(
					"vec4 texture2DLodEXT(sampler2D, vec2, float);"      // GL_EXT_shader_texture_lod
					"vec4 texture2DProjLodEXT(sampler2D, vec3, float);"  // GL_EXT_shader_texture_lod
					"vec4 texture2DProjLodEXT(sampler2D, vec4, float);"  // GL_EXT_shader_texture_lod
					"vec4 textureCubeLodEXT(samplerCube, vec3, float);"  // GL_EXT_shader_texture_lod

					"\n");
			}

			stageBuiltins[EShLangFragment].append(
				"float dFdx(float p);"
				"vec2  dFdx(vec2  p);"
				"vec3  dFdx(vec3  p);"
				"vec4  dFdx(vec4  p);"

				"float dFdy(float p);"
				"vec2  dFdy(vec2  p);"
				"vec3  dFdy(vec3  p);"
				"vec4  dFdy(vec4  p);"

				"float fwidth(float p);"
				"vec2  fwidth(vec2  p);"
				"vec3  fwidth(vec3  p);"
				"vec4  fwidth(vec4  p);"

				"\n");

			// GL_ARB_derivative_control
			if (profile != EEsProfile && version >= 400) {
				stageBuiltins[EShLangFragment].append(
					"float dFdxFine(float p);"
					"vec2  dFdxFine(vec2  p);"
					"vec3  dFdxFine(vec3  p);"
					"vec4  dFdxFine(vec4  p);"

					"float dFdyFine(float p);"
					"vec2  dFdyFine(vec2  p);"
					"vec3  dFdyFine(vec3  p);"
					"vec4  dFdyFine(vec4  p);"

					"float fwidthFine(float p);"
					"vec2  fwidthFine(vec2  p);"
					"vec3  fwidthFine(vec3  p);"
					"vec4  fwidthFine(vec4  p);"

					"\n");

				stageBuiltins[EShLangFragment].append(
					"float dFdxCoarse(float p);"
					"vec2  dFdxCoarse(vec2  p);"
					"vec3  dFdxCoarse(vec3  p);"
					"vec4  dFdxCoarse(vec4  p);"

					"float dFdyCoarse(float p);"
					"vec2  dFdyCoarse(vec2  p);"
					"vec3  dFdyCoarse(vec3  p);"
					"vec4  dFdyCoarse(vec4  p);"

					"float fwidthCoarse(float p);"
					"vec2  fwidthCoarse(vec2  p);"
					"vec3  fwidthCoarse(vec3  p);"
					"vec4  fwidthCoarse(vec4  p);"

					"\n");
			}

			//============================================================================
			//
			// Standard Uniforms
			//
			//============================================================================

			//
			// Depth range in window coordinates, p. 33
			//
			commonBuiltins.Append(
				"struct gl_DepthRangeParameters {"
			);
			if (profile == Profile.EsProfile) {
				commonBuiltins.Append(
@"highp float near;
highp float far;
highp float diff;"
				);
			} else {
				commonBuiltins.Append(
@"float near;
float far;
float diff;"
				);
			}
			commonBuiltins.Append(
@"};
uniform gl_DepthRangeParameters gl_DepthRange;
");

			if (IncludeLegacy(version, profile)) {
				//
				// Matrix state. p. 31, 32, 37, 39, 40.
				//
				commonBuiltins.Append(
@"uniform mat4  gl_ModelViewMatrix;
uniform mat4  gl_ProjectionMatrix;
uniform mat4  gl_ModelViewProjectionMatrix;
uniform mat3  gl_NormalMatrix;
uniform mat4  gl_ModelViewMatrixInverse;
uniform mat4  gl_ProjectionMatrixInverse;
uniform mat4  gl_ModelViewProjectionMatrixInverse;
uniform mat4  gl_ModelViewMatrixTranspose;
uniform mat4  gl_ProjectionMatrixTranspose;
uniform mat4  gl_ModelViewProjectionMatrixTranspose;
uniform mat4  gl_ModelViewMatrixInverseTranspose;
uniform mat4  gl_ProjectionMatrixInverseTranspose;
uniform mat4  gl_ModelViewProjectionMatrixInverseTranspose;
uniform float gl_NormalScale;
struct gl_PointParameters {
float size;
float sizeMin;
float sizeMax;
float fadeThresholdSize;
float distanceConstantAttenuation;
float distanceLinearAttenuation;
float distanceQuadraticAttenuation;
};
uniform gl_PointParameters gl_Point;
struct gl_MaterialParameters {
vec4  emission;
vec4  ambient;
vec4  diffuse;
vec4  specular;
float shininess;
};
uniform gl_MaterialParameters  gl_FrontMaterial;
uniform gl_MaterialParameters  gl_BackMaterial;
struct gl_LightSourceParameters {
vec4  ambient;
vec4  diffuse;
vec4  specular;
vec4  position;
vec4  halfVector;
vec3  spotDirection;
float spotExponent;
float spotCutoff;
float spotCosCutoff;
float constantAttenuation;
float linearAttenuation;
float quadraticAttenuation;
};
struct gl_LightModelParameters {
vec4  ambient;
};
uniform gl_LightModelParameters  gl_LightModel;
struct gl_LightModelProducts {
vec4  sceneColor;
};
uniform gl_LightModelProducts gl_FrontLightModelProduct;
uniform gl_LightModelProducts gl_BackLightModelProduct;
struct gl_LightProducts {
vec4  ambient;
vec4  diffuse;
vec4  specular;
};
struct gl_FogParameters {
vec4  color;
float density;
float start;
float end;
float scale;
};
uniform gl_FogParameters gl_Fog;
");
			}

			//============================================================================
			//
			// Define the interface to the compute shader.
			//
			//============================================================================

			if ((profile != Profile.EsProfile && version >= 430) ||
				(profile == Profile.EsProfile && version >= 310)) {
				stageBuiltins[EShLangCompute].append(
					"in uvec3 gl_NumWorkGroups;"
					"const uvec3 gl_WorkGroupSize = uvec3(1,1,1);"

					"in uvec3 gl_WorkGroupID;"
					"in uvec3 gl_LocalInvocationID;"

					"in uvec3 gl_GlobalInvocationID;"
					"in uint gl_LocalInvocationIndex;"

					"\n");
			}

			//============================================================================
			//
			// Define the interface to the vertex shader.
			//
			//============================================================================

			if (profile != Profile.EsProfile) {
				if (version < 130) {
					stageBuiltins[EShLangVertex].append(
						"attribute vec4  gl_Color;"
						"attribute vec4  gl_SecondaryColor;"
						"attribute vec3  gl_Normal;"
						"attribute vec4  gl_Vertex;"
						"attribute vec4  gl_MultiTexCoord0;"
						"attribute vec4  gl_MultiTexCoord1;"
						"attribute vec4  gl_MultiTexCoord2;"
						"attribute vec4  gl_MultiTexCoord3;"
						"attribute vec4  gl_MultiTexCoord4;"
						"attribute vec4  gl_MultiTexCoord5;"
						"attribute vec4  gl_MultiTexCoord6;"
						"attribute vec4  gl_MultiTexCoord7;"
						"attribute float gl_FogCoord;"
						"\n");
				} else if (IncludeLegacy(version, profile)) {
					stageBuiltins[EShLangVertex].append(
						"in vec4  gl_Color;"
						"in vec4  gl_SecondaryColor;"
						"in vec3  gl_Normal;"
						"in vec4  gl_Vertex;"
						"in vec4  gl_MultiTexCoord0;"
						"in vec4  gl_MultiTexCoord1;"
						"in vec4  gl_MultiTexCoord2;"
						"in vec4  gl_MultiTexCoord3;"
						"in vec4  gl_MultiTexCoord4;"
						"in vec4  gl_MultiTexCoord5;"
						"in vec4  gl_MultiTexCoord6;"
						"in vec4  gl_MultiTexCoord7;"
						"in float gl_FogCoord;"            
						"\n");
				}

				if (version < 150) {
					if (version < 130) {
						stageBuiltins[EShLangVertex].append(
							"        vec4  gl_ClipVertex;"       // needs qualifier fixed later
							"varying vec4  gl_FrontColor;"
							"varying vec4  gl_BackColor;"
							"varying vec4  gl_FrontSecondaryColor;"
							"varying vec4  gl_BackSecondaryColor;"
							"varying vec4  gl_TexCoord[];"
							"varying float gl_FogFragCoord;"
							"\n");
					} else if (IncludeLegacy(version, profile)) {
						stageBuiltins[EShLangVertex].append(
							"    vec4  gl_ClipVertex;"       // needs qualifier fixed later
							"out vec4  gl_FrontColor;"
							"out vec4  gl_BackColor;"
							"out vec4  gl_FrontSecondaryColor;"
							"out vec4  gl_BackSecondaryColor;"
							"out vec4  gl_TexCoord[];"
							"out float gl_FogFragCoord;"
							"\n");
					}
					stageBuiltins[EShLangVertex].append(
						"vec4 gl_Position;"   // needs qualifier fixed later
						"float gl_PointSize;" // needs qualifier fixed later
					);

					if (version == 130 || version == 140)
						stageBuiltins[EShLangVertex].append(
							"out float gl_ClipDistance[];"
						);
				} else {
					// version >= 150
					stageBuiltins[EShLangVertex].append(
						"out gl_PerVertex {"
						"vec4 gl_Position;"     // needs qualifier fixed later
						"float gl_PointSize;"   // needs qualifier fixed later
						"float gl_ClipDistance[];"
					);            
					if (IncludeLegacy(version, profile))
						stageBuiltins[EShLangVertex].append(
							"vec4 gl_ClipVertex;"   // needs qualifier fixed later
							"vec4 gl_FrontColor;"
							"vec4 gl_BackColor;"
							"vec4 gl_FrontSecondaryColor;"
							"vec4 gl_BackSecondaryColor;"
							"vec4 gl_TexCoord[];"
							"float gl_FogFragCoord;"
						);
					if (version >= 450)
						stageBuiltins[EShLangVertex].append(
							"float gl_CullDistance[];"
						);
					stageBuiltins[EShLangVertex].append(
						"};"
						"\n");
				}
				if (version >= 130)
					stageBuiltins[EShLangVertex].append(
						"int gl_VertexID;"            // needs qualifier fixed later
					);
				if (version >= 140)
					stageBuiltins[EShLangVertex].append(
						"int gl_InstanceID;"          // needs qualifier fixed later
					);
			} else {
				// ES profile
				if (version == 100) {
					stageBuiltins[EShLangVertex].append(
						"highp   vec4  gl_Position;"  // needs qualifier fixed later
						"mediump float gl_PointSize;" // needs qualifier fixed later
					);
				} else {
					stageBuiltins[EShLangVertex].append(
						"highp int gl_VertexID;"      // needs qualifier fixed later
						"highp int gl_InstanceID;"    // needs qualifier fixed later
					);
					if (version < 310)
						stageBuiltins[EShLangVertex].append(
							"highp vec4  gl_Position;"    // needs qualifier fixed later
							"highp float gl_PointSize;"   // needs qualifier fixed later
						);
					else
						stageBuiltins[EShLangVertex].append(
							"out gl_PerVertex {"
							"highp vec4  gl_Position;"    // needs qualifier fixed later
							"highp float gl_PointSize;"   // needs qualifier fixed later
							"};"
						);
				}
			}

			//============================================================================
			//
			// Define the interface to the geometry shader.
			//
			//============================================================================

			if (profile == Profile.CoreProfile || profile == Profile.CompatibilityProfile) {
				stageBuiltins[EShLangGeometry].append(
					"in gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
					"float gl_ClipDistance[];"
				);
				if (profile == Profile.CompatibilityProfile)
					stageBuiltins[EShLangGeometry].append(
						"vec4 gl_ClipVertex;"
						"vec4 gl_FrontColor;"
						"vec4 gl_BackColor;"
						"vec4 gl_FrontSecondaryColor;"
						"vec4 gl_BackSecondaryColor;"
						"vec4 gl_TexCoord[];"
						"float gl_FogFragCoord;"
					);
				if (version >= 450)
					stageBuiltins[EShLangGeometry].append(
						"float gl_CullDistance[];"
					);
				stageBuiltins[EShLangGeometry].append(
					"} gl_in[];"

					"in int gl_PrimitiveIDIn;"
					"out gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
					"float gl_ClipDistance[];"
					"\n");
				if (profile == Profile.CompatibilityProfile && version >= 400)
					stageBuiltins[EShLangGeometry].append(
						"vec4 gl_ClipVertex;"
						"vec4 gl_FrontColor;"
						"vec4 gl_BackColor;"
						"vec4 gl_FrontSecondaryColor;"
						"vec4 gl_BackSecondaryColor;"
						"vec4 gl_TexCoord[];"
						"float gl_FogFragCoord;"
					);
				if (version >= 450)
					stageBuiltins[EShLangGeometry].append(
						"float gl_CullDistance[];"
					);
				stageBuiltins[EShLangGeometry].append(
					"};"

					"out int gl_PrimitiveID;"
					"out int gl_Layer;");

				if (profile == ECompatibilityProfile && version < 400)
					stageBuiltins[EShLangGeometry].append(
						"out vec4 gl_ClipVertex;"
					);

				if (version >= 400)
					stageBuiltins[EShLangGeometry].append(
						"in int gl_InvocationID;"
					);
				// GL_ARB_viewport_array
				if (version >= 150)
					stageBuiltins[EShLangGeometry].append(
						"out int gl_ViewportIndex;"
					);
				stageBuiltins[EShLangGeometry].append("\n");
			} else if (profile == Profile.EsProfile && version >= 310) {
				stageBuiltins[EShLangGeometry].append(
					"in gl_PerVertex {"
					"highp vec4 gl_Position;"
					"highp float gl_PointSize;"
					"} gl_in[];"
					"\n"
					"in highp int gl_PrimitiveIDIn;"
					"in highp int gl_InvocationID;"
					"\n"
					"out gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
					"};"
					"\n"
					"out int gl_PrimitiveID;"
					"out int gl_Layer;"
					"\n"
				);
			}


			//============================================================================
			//
			// Define the interface to the tessellation control shader.
			//
			//============================================================================

			if (profile != Profile.EsProfile && version >= 150) {
				// Note:  "in gl_PerVertex {...} gl_in[gl_MaxPatchVertices];" is declared in initialize() below,
				// as it depends on the resource sizing of gl_MaxPatchVertices.

				stageBuiltins[EShLangTessControl].append(
					"in int gl_PatchVerticesIn;"
					"in int gl_PrimitiveID;"
					"in int gl_InvocationID;"

					"out gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
					"float gl_ClipDistance[];"
				);
				if (profile == ECompatibilityProfile)
					stageBuiltins[EShLangTessControl].append(
						"vec4 gl_ClipVertex;"
						"vec4 gl_FrontColor;"
						"vec4 gl_BackColor;"
						"vec4 gl_FrontSecondaryColor;"
						"vec4 gl_BackSecondaryColor;"
						"vec4 gl_TexCoord[];"
						"float gl_FogFragCoord;"
					);
				if (version >= 450)
					stageBuiltins[EShLangTessControl].append(
						"float gl_CullDistance[];"
					);
				stageBuiltins[EShLangTessControl].append(
					"} gl_out[];"

					"patch out float gl_TessLevelOuter[4];"
					"patch out float gl_TessLevelInner[2];"
					"\n");
			} else {
				// Note:  "in gl_PerVertex {...} gl_in[gl_MaxPatchVertices];" is declared in initialize() below,
				// as it depends on the resource sizing of gl_MaxPatchVertices.

				stageBuiltins[EShLangTessControl].append(
					"in highp int gl_PatchVerticesIn;"
					"in highp int gl_PrimitiveID;"
					"in highp int gl_InvocationID;"

					"out gl_PerVertex {"
					"highp vec4 gl_Position;"
					"highp float gl_PointSize;"
				);
				stageBuiltins[EShLangTessControl].append(
					"} gl_out[];"

					"patch out highp float gl_TessLevelOuter[4];"
					"patch out highp float gl_TessLevelInner[2];"
					"\n");
			}

			//============================================================================
			//
			// Define the interface to the tessellation evaluation shader.
			//
			//============================================================================

			if (profile != Profile.EsProfile && version >= 150) {
				// Note:  "in gl_PerVertex {...} gl_in[gl_MaxPatchVertices];" is declared in initialize() below,
				// as it depends on the resource sizing of gl_MaxPatchVertices.

				stageBuiltins[EShLangTessEvaluation].append(
					"in int gl_PatchVerticesIn;"
					"in int gl_PrimitiveID;"
					"in vec3 gl_TessCoord;"

					"patch in float gl_TessLevelOuter[4];"
					"patch in float gl_TessLevelInner[2];"

					"out gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
					"float gl_ClipDistance[];"
				);
				if (version >= 400 && profile == Profile.CompatibilityProfile)
					stageBuiltins[EShLangTessEvaluation].append(
						"vec4 gl_ClipVertex;"
						"vec4 gl_FrontColor;"
						"vec4 gl_BackColor;"
						"vec4 gl_FrontSecondaryColor;"
						"vec4 gl_BackSecondaryColor;"
						"vec4 gl_TexCoord[];"
						"float gl_FogFragCoord;"
					);
				if (version >= 450)
					stageBuiltins[EShLangTessEvaluation].append(
						"float gl_CullDistance[];"
					);
				stageBuiltins[EShLangTessEvaluation].append(
					"};"
					"\n");
			} else if (profile == EEsProfile && version >= 310) {
				// Note:  "in gl_PerVertex {...} gl_in[gl_MaxPatchVertices];" is declared in initialize() below,
				// as it depends on the resource sizing of gl_MaxPatchVertices.

				stageBuiltins[EShLangTessEvaluation].append(
					"in highp int gl_PatchVerticesIn;"
					"in highp int gl_PrimitiveID;"
					"in highp vec3 gl_TessCoord;"

					"patch in highp float gl_TessLevelOuter[4];"
					"patch in highp float gl_TessLevelInner[2];"

					"out gl_PerVertex {"
					"vec4 gl_Position;"
					"float gl_PointSize;"
				);
				stageBuiltins[EShLangTessEvaluation].append(
					"};"
					"\n");
			}

			//============================================================================
			//
			// Define the interface to the fragment shader.
			//
			//============================================================================

			if (profile != Profile.EsProfile) {

				stageBuiltins[EShLangFragment].append(
					"vec4  gl_FragCoord;"   // needs qualifier fixed later
					"bool  gl_FrontFacing;" // needs qualifier fixed later
					"float gl_FragDepth;"   // needs qualifier fixed later
				);
				if (version >= 120)
					stageBuiltins[EShLangFragment].append(
						"vec2 gl_PointCoord;"  // needs qualifier fixed later
					);
				if (IncludeLegacy(version, profile) || (! ForwardCompatibility && version < 420))
					stageBuiltins[EShLangFragment].append(
						"vec4 gl_FragColor;"   // needs qualifier fixed later
					);

				if (version < 130) {
					stageBuiltins[EShLangFragment].append(
						"varying vec4  gl_Color;"
						"varying vec4  gl_SecondaryColor;"
						"varying vec4  gl_TexCoord[];"
						"varying float gl_FogFragCoord;"
					);
				} else {
					stageBuiltins[EShLangFragment].append(
						"in float gl_ClipDistance[];"
					);

					if (IncludeLegacy(version, profile)) {
						if (version < 150)
							stageBuiltins[EShLangFragment].append(
								"in float gl_FogFragCoord;"
								"in vec4  gl_TexCoord[];"
								"in vec4  gl_Color;"
								"in vec4  gl_SecondaryColor;"
							);
						else
							stageBuiltins[EShLangFragment].append(
								"in gl_PerFragment {"
								"in float gl_FogFragCoord;"
								"in vec4  gl_TexCoord[];"
								"in vec4  gl_Color;"
								"in vec4  gl_SecondaryColor;"
								"};"
							);
					}
				}

				if (version >= 150)
					stageBuiltins[EShLangFragment].append(
						"flat in int gl_PrimitiveID;"
					);

				if (version >= 400)
					stageBuiltins[EShLangFragment].append(
						"flat in  int  gl_SampleID;"
						"     in  vec2 gl_SamplePosition;"
						"flat in  int  gl_SampleMaskIn[];"
						"     out int  gl_SampleMask[];"
					);

				if (version >= 430)
					stageBuiltins[EShLangFragment].append(
						"flat in int gl_Layer;"
						"flat in int gl_ViewportIndex;"
					);

				if (version >= 450)
					stageBuiltins[EShLangFragment].append(
						"in float gl_CullDistance[];"
						"bool gl_HelperInvocation;"     // needs qualifier fixed later
					);
			} else {
				// ES profile

				if (version == 100)
					stageBuiltins[EShLangFragment].append(
						"mediump vec4 gl_FragCoord;"    // needs qualifier fixed later
						"        bool gl_FrontFacing;"  // needs qualifier fixed later
						"mediump vec4 gl_FragColor;"    // needs qualifier fixed later
						"mediump vec2 gl_PointCoord;"   // needs qualifier fixed later
					);
				else if (version >= 300) {
					stageBuiltins[EShLangFragment].append(
						"highp   vec4  gl_FragCoord;"    // needs qualifier fixed later
						"        bool  gl_FrontFacing;"  // needs qualifier fixed later
						"mediump vec2  gl_PointCoord;"   // needs qualifier fixed later
						"highp   float gl_FragDepth;"    // needs qualifier fixed later
					);
					if (version >= 310)
						stageBuiltins[EShLangFragment].append(
							"bool gl_HelperInvocation;"    // needs qualifier fixed later
							"flat in highp int gl_PrimitiveID;"  // needs qualifier fixed later
							"flat in highp int gl_Layer;"        // needs qualifier fixed later
						);
				}
				stageBuiltins[EShLangFragment].append(
					"highp float gl_FragDepthEXT;"       // GL_EXT_frag_depth
				);
			}
			stageBuiltins[EShLangFragment].append("\n");

			if (version >= 130)
				add2ndGenerationSamplingImaging(version, profile);

			// printf("%s\n", commonBuiltins.c_str());
		}

		// TODO: ARB_Compatability: do full extension support
		public bool ARBCompatibility = true;

		bool IncludeLegacy(int version, Profile profile)
		{
			return profile != Profile.EsProfile && (version <= 130 || ARBCompatibility || profile == Profile.CompatibilityProfile);
		}
		//

		public enum TBasicType 
		{
			EbtVoid = 0,
			EbtFloat,
			EbtDouble,
			EbtInt,
			EbtUint,
			EbtBool,
			EbtAtomicUint,
			EbtSampler,
			EbtStruct,
			EbtBlock,
			EbtNumTypes
		}

		//
		// Details within a sampler type
		//
		public enum TSamplerDim
		{
			EsdNone = 0,
			Esd1D,
			Esd2D,
			Esd3D,
			EsdCube,
			EsdRect,
			EsdBuffer,
			EsdNumDims
		};

		public class TSampler 
		{
			public TBasicType type;  // type returned by sampler
			public TSamplerDim dim;
			public bool    arrayed;
			public bool     shadow;
			public bool         ms;
			public bool      image;
			public bool   external;  // GL_OES_EGL_image_external

			void clear()
			{
				type = TBasicType.EbtVoid;
				dim = TSamplerDim.EsdNone;
				arrayed = false;
				shadow = false;
				ms = false;
				image = false;
				external = false;
			}

			void set(TBasicType t, TSamplerDim d, bool a, bool s, bool m)
			{
				type = t;
				dim = d;
				arrayed = a;
				shadow = s;
				ms = m;
				image = false;
				external = false;
			}

			void setImage(TBasicType t, TSamplerDim d, bool a = false, bool s = false, bool m = false)
			{
				type = t;
				dim = d;
				arrayed = a;
				shadow = s;
				ms = m;
				image = true;
				external = false;
			}

//			bool operator==(const TSampler& right) const
//			{
//				return type == right.type &&
//					dim == right.dim &&
//					arrayed == right.arrayed &&
//					shadow == right.shadow &&
//					ms == right.ms &&
//					image == right.image &&
//					external == right.external;
//			}

			public string getString()
			{
				var s = new StringBuilder ();

				switch (type) {
					case TBasicType.EbtFloat:
						break;
					case TBasicType.EbtInt:
						s.Append("i");
						break;
					case TBasicType.EbtUint:
						s.Append("u");
						break;
					default:
						break;  // some compilers want this
				}
				if (image)
					s.Append("image");
				else
					s.Append("sampler");
				if (external) {
					s.Append("ExternalOES");
					return s;
				}
				switch (dim) {
					case TSamplerDim.Esd1D:
						s.Append("1D");
						break;
					case TSamplerDim.Esd2D:
						s.Append("2D");
						break;
					case TSamplerDim.Esd3D:
						s.Append("3D");
						break;
					case TSamplerDim.EsdCube:
						s.Append("Cube");
						break;
					case TSamplerDim.EsdRect:
						s.Append("2DRect");
						break;
					case TSamplerDim.EsdBuffer:
						s.Append("Buffer");
						break;
					default:
						break;  // some compilers want this
				}
				if (ms)
					s.Append("MS");
				if (arrayed)
					s.Append("Array");
				if (shadow)
					s.Append("Shadow");

				return s.ToString();
			}
		};

		//
		// Helper function for initialize(), to add the second set of names for texturing, 
		// when adding context-independent built-in functions.
		//
		void add2ndGenerationSamplingImaging(int version, Profile profile)
		{
			//
			// In this function proper, enumerate the types, then calls the next set of functions
			// to enumerate all the uses for that type.
			//

			var  bTypes = new TBasicType[]{ TBasicType.EbtFloat, TBasicType.EbtInt, TBasicType.EbtUint };

			// enumerate all the types
			for (int image = 0; image <= 1; ++image) { // loop over "bool" image vs sampler

				for (int shadow = 0; shadow <= 1; ++shadow) { // loop over "bool" shadow or not
					for (int ms = 0; ms <=1; ++ms) {

						if ((ms > 0 || image > 0) && shadow)
							continue;
						if (ms > 0 && profile != Profile.EsProfile && version < 150)
							continue;
						if (ms > 0 && image && profile == Profile.EsProfile)
							continue;
						if (ms > 0 && profile == Profile.EsProfile && version < 310)
							continue;

						for (int arrayed = 0; arrayed <= 1; ++arrayed) { // loop over "bool" arrayed or not
							for (int dim = TSamplerDim.Esd1D; dim < (int) TSamplerDim.EsdNumDims; ++dim) { // 1D, 2D, ..., buffer

								if ((dim == (int) TSamplerDim.Esd1D || dim == TSamplerDim.EsdRect) && profile == Profile.EsProfile)
									continue;
								if (dim != (int) TSamplerDim.Esd2D && ms)
									continue;
								if ((dim == (int) TSamplerDim.Esd3D || dim == (int) TSamplerDim.EsdRect) && arrayed)
									continue;
								if (dim == (int) TSamplerDim.Esd3D && shadow)
									continue;
								if (dim == (int) TSamplerDim.EsdCube && arrayed && (profile == Profile.EsProfile || version < 130))
									continue;
								if (dim == (int) TSamplerDim.EsdBuffer && (profile == Profile.EsProfile || version < 140))
									continue;
								if (dim == (int) TSamplerDim.EsdBuffer && (shadow || arrayed || ms))
									continue;
								if (ms && arrayed && profile == Profile.EsProfile)
									continue;

								for (int bType = 0; bType < 3; ++bType) { // float, int, uint results

									if (shadow > 0 && bType > 0)
										continue;

									if (dim == (int) TSamplerDim.EsdRect && version < 140 && bType > 0)
										continue;

									//
									// Now, make all the function prototypes for the type we just built...
									//

									TSampler sampler;
									sampler.set(
										bTypes[bType], 
										(TSamplerDim)dim,
										arrayed > 0,
										shadow > 0,
										ms > 0);
									if (image > 0)
										sampler.image = true;

									string typeName = sampler.getString();

									addQueryFunctions(sampler, typeName, version, profile);

									if (image > 0)
										addImageFunctions(sampler, typeName, version, profile);
									else {
										addSamplingFunctions(sampler, typeName, version, profile);
										addGatherFunctions(sampler, typeName, version, profile);
									}
								}
							}
						}
					}
				}
			}
		}

		//
		// Helper function for add2ndGenerationSamplingImaging(), 
		// when adding context-independent built-in functions.
		//
		// Add all the query functions for the given type.
		//
		void addQueryFunctions(TSampler sampler, string typeName, int version, Profile profile)
		{
			//
			// textureSize
			//

			if (sampler.image && ((profile == Profile.EsProfile && version < 310) || (profile != Profile.EsProfile && version < 430)))
				return;

			if (profile == Profile.EsProfile)
				commonBuiltins.Append("highp ");
			int dims = dimMap[sampler.dim] + (sampler.arrayed ? 1 : 0) - (sampler.dim == TSamplerDim.EsdCube ? 1 : 0);
			if (dims == 1)
				commonBuiltins.Append("int");
			else {
				commonBuiltins.Append("ivec");
				commonBuiltins.Append(postfixes[dims]);
			}
			if (sampler.image)
				commonBuiltins.Append(" imageSize(readonly writeonly volatile coherent ");
			else
				commonBuiltins.Append(" textureSize(");
			commonBuiltins.Append(typeName);
			if (! sampler.image && sampler.dim != TSamplerDim.EsdRect && sampler.dim != TSamplerDim.EsdBuffer && ! sampler.ms)
				commonBuiltins.Append(",int);\n");
			else
				commonBuiltins.Append(");\n");

			// GL_ARB_shader_texture_image_samples
			// TODO: spec issue? there are no memory qualifiers; how to query a writeonly/readonly image, etc?
			if (profile != Profile.EsProfile && version >= 430 && sampler.ms) {
				commonBuiltins.Append("int ");
				if (sampler.image)
					commonBuiltins.Append("imageSamples(readonly writeonly volatile coherent ");
				else
					commonBuiltins.Append("textureSamples(");
				commonBuiltins.Append(typeName);
				commonBuiltins.Append(");\n");
			}
		}

		//
		// Helper function for add2ndGenerationSamplingImaging(), 
		// when adding context-independent built-in functions.
		//
		// Add all the image access functions for the given type.
		//
		void addImageFunctions(TSampler sampler, string typeName, int version, Profile profile)
		{
			int dims = dimMap[sampler.dim] + (sampler.arrayed ? 1 : 0);
			var imageParams = new StringBuilder();
			imageParams.Append(typeName);

			if (dims == 1)
				imageParams.Append(", int");
			else {
				imageParams.Append(", ivec");
				imageParams.Append(postfixes[dims]);
			}
			if (sampler.ms)
				imageParams.Append(", int");

			commonBuiltins.Append(prefixes[sampler.type]);
			commonBuiltins.Append("vec4 imageLoad(readonly volatile coherent ");
			commonBuiltins.Append(imageParams.ToString());
			commonBuiltins.Append(");\n");

			commonBuiltins.Append("void imageStore(writeonly volatile coherent ");
			commonBuiltins.Append(imageParams.ToString());
			commonBuiltins.Append(", ");
			commonBuiltins.Append(prefixes[sampler.type]);
			commonBuiltins.Append("vec4);\n");

			if (profile != Profile.EsProfile) {
				if (sampler.type == TBasicType.EbtInt || sampler.type == TBasicType.EbtUint) {
					string dataType = sampler.type == TBasicType.EbtInt ? "int" : "uint";

					const string[] atomicFunc = new string[]
					{
						" imageAtomicAdd(volatile coherent ",
						" imageAtomicMin(volatile coherent ",
						" imageAtomicMax(volatile coherent ",
						" imageAtomicAnd(volatile coherent ",
						" imageAtomicOr(volatile coherent ",
						" imageAtomicXor(volatile coherent ",
						" imageAtomicExchange(volatile coherent "
					}; 

					for (uint i = 0; i < atomicFunc.Length; ++i) {
						commonBuiltins.Append(dataType);
						commonBuiltins.Append(atomicFunc[i]);
						commonBuiltins.Append(imageParams);
						commonBuiltins.Append(", ");
						commonBuiltins.Append(dataType);
						commonBuiltins.Append(");\n");
					}

					commonBuiltins.Append(dataType);
					commonBuiltins.Append(" imageAtomicCompSwap(volatile coherent ");
					commonBuiltins.Append(imageParams);
					commonBuiltins.Append(", ");
					commonBuiltins.Append(dataType);
					commonBuiltins.Append(", ");
					commonBuiltins.Append(dataType);
					commonBuiltins.Append(");\n");
				} else {
					// not int or uint
					// GL_ARB_ES3_1_compatibility
					// TODO: spec issue: are there restrictions on the kind of layout() that can be used?  what about dropping memory qualifiers?
					if (version >= 450) {
						commonBuiltins.Append("float imageAtomicExchange(volatile coherent ");
						commonBuiltins.Append(imageParams);
						commonBuiltins.Append(", float);\n");
					}
				}
			}
		}

		//
		// Helper function for add2ndGenerationSamplingImaging(), 
		// when adding context-independent built-in functions.
		//
		// Add all the texture lookup functions for the given type.
		//
		void addSamplingFunctions(TSampler sampler, string typeName)
		{
			//
			// texturing
			//
			for (int proj = 0; proj <= 1; ++proj) { // loop over "bool" projective or not

				if (proj > 0 && (sampler.dim == TSamplerDim.EsdCube || sampler.dim == TSamplerDim.EsdBuffer || sampler.arrayed || sampler.ms))
					continue;

				for (int lod = 0; lod <= 1; ++lod) {

					if (lod > 0 && (sampler.dim == TSamplerDim.EsdBuffer || sampler.dim == TSamplerDim.EsdRect || sampler.ms))
						continue;
					if (lod > 0 && sampler.dim == TSamplerDim.Esd2D && sampler.arrayed && sampler.shadow)
						continue;
					if (lod > 0 && sampler.dim == TSamplerDim.EsdCube && sampler.shadow)
						continue;

					for (int bias = 0; bias <= 1; ++bias) {

						if (bias > 0 && (lod || sampler.ms))
							continue;
						if (bias > 0 && sampler.dim == TSamplerDim.Esd2D && sampler.shadow && sampler.arrayed)
							continue;
						if (bias > 0 && (sampler.dim == TSamplerDim.EsdRect || sampler.dim == TSamplerDim.EsdBuffer))
							continue;

						for (int offset = 0; offset <= 1; ++offset) { // loop over "bool" offset or not

							if (proj + offset + bias + lod > 3)
								continue;
							if (offset > 0 && (sampler.dim == TSamplerDim.EsdCube || sampler.dim == TSamplerDim.EsdBuffer || sampler.ms))
								continue;

							for (int fetch = 0; fetch <= 1; ++fetch) { // loop over "bool" fetch or not

								if (proj + offset + fetch + bias + lod > 3)
									continue;
								if (fetch > 0 && (lod || bias))
									continue;
								if (fetch > 0 && (sampler.shadow || sampler.dim == TSamplerDim.EsdCube))
									continue;
								if (fetch == 0 && (sampler.ms || sampler.dim == TSamplerDim.EsdBuffer))
									continue;

								for (int grad = 0; grad <= 1; ++grad) { // loop over "bool" grad or not

									if (grad > 0 && (lod || bias || sampler.ms))
										continue;
									if (grad > 0 && sampler.dim == TSamplerDim.EsdBuffer)
										continue;
									if (proj + offset + fetch + grad + bias + lod > 3)
										continue;

									for (int extraProj = 0; extraProj <= 1; ++extraProj) {
										bool compare = false;
										int totalDims = dimMap[sampler.dim] + (sampler.arrayed ? 1 : 0);
										// skip dummy unused second component for 1D non-array shadows
										if (sampler.shadow && totalDims < 2)
											totalDims = 2;
										totalDims += (sampler.shadow ? 1 : 0) + proj;
										if (totalDims > 4 && sampler.shadow) {
											compare = true;
											totalDims = 4;
										}
										Debug.Assert(totalDims <= 4);

										if (extraProj > 0 && proj == 0)
											continue;
										if (extraProj > 0 && (sampler.dim == TSamplerDim.Esd3D || sampler.shadow))
											continue;

										var s = new StringBuilder ();

										// return type
										if (sampler.shadow)
											s.Append("float ");
										else {
											s.Append(prefixes[sampler.type]);
											s.Append("vec4 ");
										}

										// name
										if (fetch > 0)
											s.Append("texel");
										else
											s.Append("texture");
										if (proj > 0)
											s.Append("Proj");
										if (lod > 0)
											s.Append("Lod");
										if (grad > 0)
											s.Append("Grad");
										if (fetch > 0)
											s.Append("Fetch");
										if (offset > 0)
											s.Append("Offset");
										s.Append("(");

										// sampler type
										s.Append(typeName);

										// P coordinate
										if (extraProj > 0)
											s.Append(",vec4");
										else {
											s.Append(",");
											var t = fetch ? TBasicType.EbtInt : TBasicType.EbtFloat;
											if (totalDims == 1)
												s.Append(getBasicString(t));
											else {
												s.Append(prefixes[t]);
												s.Append("vec");
												s.Append(postfixes[totalDims]);
											}
										}

										if (bias > 0 && compare)
											continue;

										// non-optional lod argument (lod that's not driven by lod loop)
										if (fetch > 0 && sampler.dim != TSamplerDim.EsdBuffer && sampler.dim != TSamplerDim.EsdRect && !sampler.ms)
											s.Append(",int");

										// non-optional lod
										if (lod > 0)
											s.Append(",float");

										// gradient arguments
										if (grad > 0) {
											if (dimMap[sampler.dim] == 1)
												s.Append(",float,float");
											else {
												s.Append(",vec");
												s.Append(postfixes[dimMap[sampler.dim]]);
												s.Append(",vec");
												s.Append(postfixes[dimMap[sampler.dim]]);
											}
										}

										// offset
										if (offset > 0) {
											if (dimMap[sampler.dim] == 1)
												s.Append(",int");
											else {
												s.Append(",ivec");
												s.Append(postfixes[dimMap[sampler.dim]]);
											}
										}

										// optional bias or non-optional compare
										if (bias > 0 || compare)
											s.Append(",float");

										s.Append(");\n");

										// Add to the per-language set of built-ins

										if (bias > 0)
											stageBuiltins[EShLangFragment].append(s.ToString());
										else
											commonBuiltins.Append(s.ToString());
									}
								}
							}
						}
					}
				}
			}
		}

		static string getBasicString(TBasicType t)
		{
			switch (t) {
			case TBasicType.EbtVoid:
				return "void";
			case TBasicType.EbtFloat:
				return "float";
			case TBasicType.EbtDouble:
				return "double";
			case TBasicType.EbtInt:
				return "int";
			case TBasicType.EbtUint:
				return "uint";
			case TBasicType.EbtBool:
				return "bool";
			case TBasicType.EbtAtomicUint:
				return "atomic_uint";
			case TBasicType.EbtSampler:
				return "sampler/image";
			case TBasicType.EbtStruct:
				return "structure";
			case TBasicType.EbtBlock:
				return "block";
			default:                   
				return "unknown type";
			}
		}

		//
		// Helper function for add2ndGenerationSamplingImaging(), 
		// when adding context-independent built-in functions.
		//
		// Add all the texture gather functions for the given type.
		//
		void addGatherFunctions(TSampler sampler, string typeName, int version, Profile profile)
		{
			switch (sampler.dim) {
			case TSamplerDim.Esd2D:
			case TSamplerDim.EsdRect:
			case TSamplerDim.EsdCube:
				break;
			default:
				return;
			}

			if (sampler.ms)
				return;

			if (version < 140 && sampler.dim == TSamplerDim.EsdRect && sampler.type != TBasicType.EbtFloat)
				return;

			for (int offset = 0; offset < 3; ++offset) { // loop over three forms of offset in the call name:  none, Offset, and Offsets

				if (profile == Profile.EsProfile && offset == 2)
					continue;

				for (int comp = 0; comp < 2; ++comp) { // loop over presence of comp argument

					if (comp > 0 && sampler.shadow)
						continue;

					if (offset > 0 && sampler.dim == TSamplerDim.EsdCube)
						continue;

					var s = new StringBuilder ();

					// return type
					s.Append(prefixes[sampler.type]);
					s.Append("vec4 ");

					// name
					s.Append("textureGather");
					switch (offset) {
					case 1:
						s.Append("Offset");
						break;
					case 2:
						s.Append("Offsets");
					default:
						break;
					}
					s.Append("(");

					// sampler type argument
					s.Append(typeName);

					// P coordinate argument
					s.Append(",vec");
					int totalDims = dimMap[sampler.dim] + (sampler.arrayed ? 1 : 0);
					s.Append(postfixes[totalDims]);

					// refZ argument
					if (sampler.shadow)
						s.Append(",float");

					// offset argument
					if (offset > 0) {
						s.Append(",ivec2");
						if (offset == 2)
							s.Append("[4]");
					}

					// comp argument
					if (comp > 0)
						s.Append(",int");

					s.Append(");\n");
					commonBuiltins.Append(s.ToString());
					//printf("%s", s.c_str());
				}
			}
		}
	}
}

