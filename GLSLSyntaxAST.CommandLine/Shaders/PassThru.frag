#version 150

out vec4 outColor;
varying vec4 corners;

void main()
{
	outColor = corners;
	//outColor = vec4(1,1,1,1);
}