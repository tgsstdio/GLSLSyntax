struct BindlessTextureHandle
{
	sampler2d TextureId;
};

struct SentanceBlock 
{
	BindlessTextureHandle Handle;
	vec4 colour;
	mat4 transform;	
};

layout(binding = 0, std140) buffer PrintLines
{
	SentanceBlock sentances[];
};

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoords;
layout(location = 2) in uint in_drawId;

flat varying uint materialIndex;
smooth varying vec2 uv;

void main(void)
{
	uv = in_texCoords;
	materialIndex = in_drawId;
	gl_Position = sentances[materialIndex].transform * in_position;
}


