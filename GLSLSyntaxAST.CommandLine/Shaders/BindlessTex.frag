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

flat varying uint materialIndex;
smooth varying vec2 uv;

out vec4 fragColor;

void main()
{
	vec4 image = tex2d(sentances[materialIndex].Handle.TextureId, uv);
	fragColor = image;
}