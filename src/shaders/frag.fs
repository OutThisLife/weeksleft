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

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

float tomoe(vec2 p) {
  float r = length(p - .03), a = atan(p.y, p.x), s = dot(p, p);

  float d = 1. - saturate(r / .2);
  d *= saturate(sin(a + 12. - 12. * sqrt(s)));

  return d;
}

vec2 repeatUV(vec2 p, float r, float s) {
  float t = mod(iTime / 2., 1.);

  float th = atan(TWOPI / 10.);
  mat2 rot = R(cos(th), sin(th));

  vec2 offset = vec2(1, 2) * t * s * rot;
  vec2 uv = round(rot * (vec2(r) - offset) / s);

  uv = (uv + vec2(-1, 0)) * rot * s + offset;

  return uv;
}

float repeat(vec2 p, float r, float s) {
  vec2 uv = repeatUV(p, r, s);
  return saturate(pow(uv.x, 2.) + pow(uv.y, 2.));
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 uv = vUv * vUvRes.xy;
  vec2 m = iMouse * vUvRes.xy;

  float t = iTime / 2.;

  vec3 col = mix(vec3(.33), vec3(1.), SM(-.33, .99, 1. - length(st)));
  vec3 noise = vec3(rand(st * 1.5), rand(st * 2.5), rand(st));

  col = mix(col, noise, .1);

  vec2 p = st;

  float r = length(p), a = atan(abs(p).y, abs(p).x), s = dot(p, p);
  vec2 o = vec2(.1, .12) / 3.;

  {
    float d = SM(0., .2, 2. * tomoe(p));
    col = mix(col, vec3(0.), d);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
