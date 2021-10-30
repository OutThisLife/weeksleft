#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

// ---------------------------------------------------

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, c) smoothstep(a, b, c)
#define hue(h) (.6 + .6 * cos(h + vec3(0, 23, 21)))
#define R(a, b) mat2(a, -b, b, a)
#define H(a) fract(sin(a) * 43758.5453)

// ---------------------------------------------------

float tomoe(vec2 p) {
  float r = length(p - .03), a = atan(p.y, p.x), s = dot(p, p);

  float d = 1. - saturate(r / .2);
  d *= saturate(sin(a + 12. - 12. * sqrt(s)));

  return d;
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 uv = vUv * vUvRes.xy;
  vec2 m = iMouse * vUvRes.xy;

  float t = iTime;

  vec3 col = mix(vec3(.33), vec3(1.), SM(-.33, .99, 1. - length(st)));
  vec2 p = st;

  float r = length(p), a = abs(atan(p.y, p.x)), s = dot(p, p);

  col = mix(col, vec3(0.), SM(0., .1, 2. * tomoe(p)));

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
