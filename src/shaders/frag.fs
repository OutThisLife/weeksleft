#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;
uniform float iFrame;
uniform sampler2D iChannel0;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

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

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

// ---------------------------------------------------

float map(vec3 p) {
  float res = 1e3;

  float c = mod(length(p), .1) + .1;
  vec3 o = vec3(c, c, 1);

  {
    float s = .089;
    vec3 q = (mod(p - s * .5, s) - s * .5) / s;

    float l = length(q);
    float d = sqrt(pow(c, 1.)) / (l / (c - .1));

    res = min(res, d);
  }

  return saturate(res);
}

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  float t = iTime * .3;
  float t0 = fract(t + .5);
  float t1 = fract(t);
  float lerp = abs((.5 - t0) / .5);

  vec3 col;

  {
    vec3 p = vec3(st, 0);
    float l = 1. + (length(p) - .1) / .1;
    vec3 rd = -normalize(p) / pow(l, 3.);

    p.xy -= mo;
    p -= p / l * 1.1;
    p += 2. * rd;

    p.xy *= mat2(cos(t), sin(t), -sin(t), cos(t));
    float d = map(p);
    col += d * SM(1., 3., l);
  }

  fragColor = vec4(saturate(col), 1.);
}
