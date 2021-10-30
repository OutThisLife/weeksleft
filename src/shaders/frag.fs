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

float noise(in vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);

  float a = rand(i);
  float b = rand(i + vec2(1., 0.));
  float c = rand(i + vec2(0., 1.));
  float d = rand(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float fbm(vec2 p, float t, float amplitude, float s, float a) {
  float mask = length(p), amp = amplitude;

  for (int i = 0; i < 8; i++) {
    p *= R(s, a);
    t += noise(p) * amp;
    amp *= amplitude;
  }

  return t;
}

float fbm(vec2 p) { return fbm(p, .5, .5, 1.6, 1.2); }

float repeat(vec2 p, float r, float s) {
  float t = mod(iTime / 2., 1.);

  float th = atan(5. / 8.);
  mat2 rot = R(cos(th), sin(th));

  vec2 offset = vec2(1, 2) * t * s * rot;
  vec2 uv = round(rot * (vec2(r) - offset) / s);

  uv = (uv + vec2(-1, 0)) * rot * s + offset;

  return saturate(uv.x);
}

float tomoe(vec2 p) {
  float r = length(p), a = atan(p.y, p.x), s = dot(p, p);

  float d = 1. - saturate(r / .2);
  d *= saturate(sin(a + 12. - 12. * sqrt(s)));

  return d;
}

// ---------------------------------------------------

const vec3 cIris = vec3(1., 0., 0.);
// const vec3 cIrisGrad = vec3(.9, .6, .2);
const vec3 cIrisGrad = hue(-2. * dot(cIris, cIris));
const vec3 cReflection = vec3(1., .9, .8);

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 uv = vUv * vUvRes.xy;
  vec2 m = iMouse * vUvRes.xy;

  float t = iTime;

  vec3 col = mix(vec3(.33), vec3(1.), SM(-.33, .99, 1. - length(st)));
  vec2 p = st;

  float r = length(p), a = abs(atan(p.y, p.x)), s = dot(p, p);

  // Tomoe!
  {
    vec2 p = st * 1.;
    col = mix(col, vec3(0.), SM(0., .1, tomoe(p)));
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
