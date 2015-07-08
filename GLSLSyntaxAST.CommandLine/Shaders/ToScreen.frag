#version 150

varying vec2 uvs;

uniform sampler2D in_colour;

out vec4 outColor;

void main()
{
	outColor = texture2D(in_colour, uvs);
}