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

float map(vec2 p) {
  float res = 1e3;

  {
    vec2 q = mod(p, .2) - .1;
    float d = .01 / length(q);

    res = min(res, d);
  }

  {
    vec2 q = mod(p - .25, .5) - .25;
    float d = .003 / min(abs(q.x), abs(q.y));
    d = min(d, .03 / max(abs(q.x) + .02, abs(q.y) + .02));

    res = max(res, 2. * d);
  }

  return saturate(res);
}

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  float t = iTime;
  float t0 = fract(t * .3 + .5);
  float t1 = fract(t * .3);
  float lerp = abs((.5 - t0) / .5);

  vec3 col;

  vec2 p = st - vec2(sin(t) + R.z, cos(t * 1.25) + sin(t * .5)) * .2;
  p = st - mo;
  vec2 rd = -normalize(p);
  float l = 1. + (length(p) - .1) / .1;

  if (l > .55) {
    vec2 p = st * cos(length(p) * .5);
    p -= p / pow(l, 2.);

    col += map(p + .08 * rd);
  }

  fragColor = vec4(saturate(col), 1.);
}
