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
    t += noise(p) * amp;
    amp *= opts.y;
  }

  return .5 + .5 * t;
}

float fbm(vec2 p) { return fbm(p, vec4(0, .5, 1.6, 1.2)); }

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;

  float t = iTime * 5.;
  float t0 = fract(t * .1 + .5);
  float t1 = fract(t * .1);
  float lerp = abs((.5 - t0) / .5);

  {
    vec2 p = st;

    float d = 1. - (S(.1, abs(p.x)) + S(.1, abs(p.y)));
  }

  {
    vec2 p = st + vec2(0, .5);
    p *= vec2(6, 2);

    float a = cos(10. * atan(p.y, p.x) + 2. - 4. * dot2(p));

    float d0 = fbm((p - vec2(0, iTime)) * rot(fbm(p) * .0001));
    float d1 = length(p * vec2(1.25 + p.y * 1.5, 1.5));
    float d2 = d0 * max(0., p.y + .75);

    float n = pow(max(0., d1 - d2), 1.2);

    float d3 = clamp(mix(1. - n, 1. / n, .5), 0., p.y + .75);

    float d = saturate(d3 * d0);

    col += vec3(1.5 * d, 1.5 * pow(d, 3.), pow(d, 6.));
    col = mix(vec3(0), col, d3 * (1. - pow(st.y, 4.)));
  }

  fragColor = vec4(saturate(col), 1);
}
