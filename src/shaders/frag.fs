#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

// ---------------------------------------------------

#define R vUvRes
#define Rpx vResolution
#define PI 3.1415926538
#define TWOPI 6.28318530718
#define PHI 2.399963229728653

#define C(v, a, b) clamp(v, a, b)
#define saturate(a) C(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SMP(v, r) SM(3. / Rpx.y, 0., length(v) - r)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

const mat2 myt = mat2(.12121212, .13131313, -.13131313, .12121212);
const vec2 mys = vec2(1e4, 1e6);

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

vec2 rhash(vec2 uv) {
  uv *= myt;
  uv *= mys;
  return fract(fract(uv / mys) * uv);
}

float voronoi2d(const vec2 st) {
  vec2 p = floor(st), f = fract(st);
  float res = 0.;

  for (int j = -1; j <= 1; j++) {
    for (int i = -1; i <= 1; i++) {
      vec2 b = vec2(i, j);
      vec2 r = vec2(b) - f + rhash(p + b);

      res += 1. / pow(dot(r, r), 8.);
    }
  }

  return pow(1. / res, .0625);
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec2 mo = iMouse * (R.xy * 2.);

  vec3 col;
  float t = iTime;

  {
    vec2 p = st;
    vec2 dir = -normalize(p);

    p += .5 * tan(dir);
    p *= rot(length(dir));
    p *= rot(length(p) + t / 2.);

    float d = length(p);
    d -= min(SM(0., .1, abs(p.y)), SM(0., .1, abs(p.x)));

    d = SM(3. / Rpx.y, 1., d);

    col += d;
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
}
