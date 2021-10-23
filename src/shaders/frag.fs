#version 300 es
precision mediump float;

uniform vec3 iResolution;
uniform vec2 iMouse;
uniform float iTime;
uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec2 vUv;
in vec3 vNormal;
in vec4 vPos;

out vec4 fragColor;

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)
#define S(a, b, c) smoothstep(a, b, c)

// ------------------------------------------------------

const int MAX = 255;
const float tmax = float(MAX);

float triangle(float x) { return abs(fract((x - 1.) / 4.) - .5) * 4. - 1.; }

mat2 R(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }
mat2 R(float s, float a) { return mat2((s), -(a), (a), (s)); }

vec2 hash(vec2 p) {
  p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
  return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float N(float t) { return fract(sin(t * 12345.564) * 7658.76); }

float noise(in vec2 p) {
  const float K1 = 0.366025404;  // (sqrt(3)-1)/2;
  const float K2 = 0.211324865;  // (3-sqrt(3))/6;
  vec2 i = floor(p + (p.x + p.y) * K1);
  vec2 a = p - i + (i.x + i.y) * K2;
  vec2 o = (a.x > a.y)
               ? vec2(1.0, 0.0)
               : vec2(0.0, 1.0);  // vec2 of = 0.5 + 0.5*vec2(sign(a.x-a.y),
                                  // sign(a.y-a.x));
  vec2 b = a - o + K2;
  vec2 c = a - 1.0 + 2.0 * K2;
  vec3 h = max(0.5 - vec3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
  vec3 n =
      h * h * h * h *
      vec3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));
  return dot(n, vec3(70.0));
}

float fbm(vec2 p, float t, float amplitude, float s, float a) {
  float mask = length(p), amp = amplitude;
  vec2 n = p;

  for (int i = 0; i < 8; i++) {
    n *= R(s, a);
    t += noise(n) * amp;
    amp *= amplitude;
  }

  return t - mask;
}

void main() {
  vec3 col;
  vec3 res = normalize(iResolution);
  vec2 st = (vUv * 2. - 1.) * res.xy;
  vec2 mo = iMouse * res.xy;

  vec3 nor = normalize(vNormal);
  vec3 ro = cameraPosition;
  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * vPos).xyz;

  float t = iTime * 1.;
  vec2 c = cos(vec2(0., .699) + .1 * iTime) - cos(vec2(0., .699) + .2 * iTime);

  vec2 p = abs(st);

  p *= 20.;

  float a = atan(p.y, p.x);
  float r = length(p);

  float d = fbm(p / 30., .5, .9, noise(p), 1.5);
  d *= 1.5;

  col += vec3(d, d - .5, triangle(d + 3.)) * a;
  ;

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}