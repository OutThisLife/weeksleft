float rand(vec2 p) {
  vec2 k1 = vec2(23.14069263277926, 2.665144142690225);
  return fract(cos(dot(p, k1)) * 12345.6789);
}

void mainImage(const in vec4 col, const in vec2 st, out vec4 fragColor) {
  vec2 p = st;
  p.y *= rand(vec2(p.y, .2));

  float d = rand(p) * .4;

  fragColor = mix(col, vec4(vec3(0), 1), d);
}