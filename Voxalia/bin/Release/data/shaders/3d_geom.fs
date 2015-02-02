uniform sampler2D tex;

void main()
{
	vec4 color = texture2D(tex,gl_TexCoord[0].st);
	float dist = 0.0;
	if (gl_TexCoord[0].x < 0.5)
	{
		dist = gl_TexCoord[0].x;
	}
	else
	{
		dist = 1.0 - gl_TexCoord[0].x;
	}
	if (gl_TexCoord[0].y < 0.5)
	{
		dist *= gl_TexCoord[0].y;
	}
	else
	{
		dist *= 1.0 - gl_TexCoord[0].y;
	}
	dist *= 32;
	dist = min(1, dist);
	gl_FragColor = vec4(color[0] * gl_Color[0] * dist, color[1] * gl_Color[1] * dist,
	color[2] * gl_Color[2] * dist, color[3] * gl_Color[3]);
}
