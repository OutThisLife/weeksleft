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

uniform vec2 uMouse;
uniform float uTime;
uniform float uSeed;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// ---------------------------------------------------

float hash(float n) { return fract(sin(n) * 753.5453123); }

float rand(vec2 p) {
  return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

float noise(in vec2 p) {
  vec2 i = floor(p), f = fract(p);

  float a = rand(i);
  float b = rand(i + vec2(1, 0));
  float c = rand(i + vec2(0, 1));
  float d = rand(i + vec2(1));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = uMouse * R.xy;

  vec3 col;

  float t = uTime;
  float t0 = fract(t * .1 + .5);
  float t1 = fract(t * .1);
  float lerp = abs((.5 - t0) / .5);

  {
    vec2 p = vUv;

    float m = distance(p - .5, uMouse);
    float d = 1. - SM(-.1, .2, m);

    vec2 q = mix(p, saturate(p - d), 1. - d);
    vec3 rd = 1. - normalize((p * rot(uSeed)).xyy);

    d = 11. +
        sin((vec2(q * rot(uSeed)) * vec2(1. - uSeed)).x * 3. * (.001 + uSeed));
    float r = sin(atan(q.y, q.x) * d - d * t * .2);

    col = p.xxy;
    col -= (rd * r / 1.);
  }

  fragColor = saturate(vec4(col, 1));
}
