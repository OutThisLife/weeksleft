#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, c) smoothstep(a, b, c)
#define fsat(a) abs(abs(a) - .5)
#define hue(h) .6 + .6 * cos(h + vec3(0, 23, 21))

// HUE_TEMPLATE(vec3)

const vec3 cBase = vec3(1., 0., 0.);
// vec3 hue(vec3 c, float s) {
//   vec3 m = vec3(cos(s), s = sin(s) * .5774, -s);
//   return c * mat3(m += (1. - m.x) / 3., m.zxy, m.yzx);
// }

// ---------------------------------------------------

void main() {
  vec3 col;
  vec3 st = vec3(vUv * 2. - 1., 0.) * vUvRes;
  vec3 mo = vec3(iMouse, 0.) * vUvRes;

  float t = iTime;
  float t4 = SM(0., 1., abs(fract(t * .5) - .5) / .5);

  vec3 p = abs(st);

  float d = 1. - length(st);
  float b = 1. - S(.1, st.y + 1. * vUvRes.y);
  float b2 = S(.1, st.y + 1.01 * vUvRes.y);

  col -= 1. - SM(-2., 2., mod(1., dot(p, p)));
  col += 1. - SM(-.2, .3, length(st));

  vec3 h = cBase * hue(st.x * 3.);
  h = mix(h, vec3(1.), 1. - S(.005 * vUvRes.y, distance(mo.x, st.x)));
  h = mix(h, vec3(1.), b2);

  col += 1. - hue(mo.x * 3.);
  col = mix(col, saturate(h), b);

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
