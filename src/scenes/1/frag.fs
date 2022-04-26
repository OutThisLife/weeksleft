#version 300 es
precision highp float;

// ---------------------------------------------------

#define R vUvRes
#define Rpx vResolution
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.61803398875
#define TAU 1.618033988749895

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(0., r / Rpx.x, v)
#define dot2(v) dot(v, v)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))

// ---------------------------------------------------

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// ---------------------------------------------------

vec2 hash(vec2 p) {
  p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
  return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float noise(in vec2 p) {
  const float K1 = 0.366025404; // (sqrt(3)-1)/2;
  const float K2 = 0.211324865; // (3-sqrt(3))/6;

  vec2 i = floor(p + (p.x + p.y) * K1);

  vec2 a = p - i + (i.x + i.y) * K2;
  vec2 o = (a.x > a.y) ? vec2(1, 0) : vec2(0, 1);
  vec2 b = a - o + K2;
  vec2 c = a - 1. + 2. * K2;

  vec3 h = max(0.5 - vec3(dot(a, a), dot(b, b), dot(c, c)), 0.);

  vec3 n =
      h * h * h * h *
      vec3(dot(a, hash(i + 0.)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));

  return dot(n, vec3(70));
}

float fbm(vec2 p, vec4 opts) {
  float t = opts.x, amp = opts.y, s = opts.z, a = opts.w;
  mat2 rot = mat2(s, a, -a, s);

  for (int i = 0; i < 6; i++) {
    p *= rot;
    amp *= opts.y;
    t += noise(p) * amp;
  }

  return .5 + t * .5;
}

float fbm(vec2 p) { return fbm(p, vec4(0, .5, 1.2, 1.6)); }

float paint(vec2 p) {
  float t = 1.;

  p.y *= fbm(p - vec2(0, t));
  p.x += .5 - fbm(3. * p - vec2(0, t));

  float d = fbm(p);
  d = S(.25, abs(p.x)) + S(.5, abs(p.y));
  // d = SM(-.2, .5, d);

  return d;
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;

  float t = iTime * .2;
  float t0 = fract(t * .1 + .5);
  float t1 = fract(t * .1);
  float lerp = abs((.5 - t0) / .5);

  {
    vec2 p = st * 2.;

    float d = paint(p);

    col += mix(col, vec3(d, pow(d, 4.), pow(d, 6.)), d);
  }

  fragColor = vec4(saturate(col), 1);
}
