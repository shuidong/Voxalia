uniform sampler2D tex;

void main()
{
	vec4 color = texture2D(tex,gl_TexCoord[0].st);
	gl_FragColor = vec4(color[0] * gl_Color[0], color[1] * gl_Color[1],
	color[2] * gl_Color[2], color[3] * gl_Color[3]);
}
