#version 150

in vec2 in_position;

in vec2 in_texcoords;

varying vec2 uvs;

void main()
{
	gl_Position = vec4(in_position, 0.0, 1.0);
	uvs = in_texcoords;
}
