#version 150

in vec3 in_position;
varying vec4 corners;

layout(std140) uniform CameraBlock
{
	mat4 projectionMatrix;
	mat4 inverseProjectionMatrix;
	mat4 viewMatrix;
	mat4 inverseViewMatrix;
	float x;
	float y;
	float z;
	float w;
	vec3 eye;
	float fieldOfView;
} in_cameras[1];

void main()
{
	corners = vec4(in_position, 1.0);
	//corners = vec4(in_cameras[0].eye, 1.0f);
	//corners = vec4(in_cameras[0].projectionMatrix[3].xyzw);
	//gl_Position = vec4(in_position, 1.0);

	vec4 pos = vec4(in_position, 1.0);
	//pos += vec4(in_cameras[0].eye, 1.0);

	//gl_Position = in_cameras[0].viewMatrix * pos;
	//gl_Position = in_cameras[0].projectionMatrix * gl_Position;

	//gl_Position = in_cameras[0].projectionMatrix * pos;
	gl_Position = in_cameras[0].projectionMatrix * in_cameras[0].viewMatrix * pos;
	//gl_Position = in_cameras[0].viewMatrix * pos;
	//gl_Position = pos;
	//gl_Position = pos;

	//corners = gl_Position;
}
