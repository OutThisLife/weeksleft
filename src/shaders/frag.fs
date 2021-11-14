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
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.61803398875
#define TAU 1.618033988749895

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(0., r / Rpx.x, v)
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

vec2 spiral(vec2 p) {
  float r = length(p);
  float a = atan(p.y, p.x) + TWOPI * SM(.5, 0., length(p * vec2(R.z, 1)));

  return r * vec2(cos(a), sin(a));
}

float distort(vec3 p) {
  float r = length(p);
  float a = atan(p.y, p.x) + TWOPI * SM(.2, 0., r);

  vec3 o = r * vec3(cos(a), sin(a), p.z + 1.);
  vec3 q = clamp(mix(p, o, 1. / sqrt(dot(p, p))), -1., 1.);

  return SM(0., 1., saturate(q.y * q.x * q.z));
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  float t = iTime;
  float t0 = fract(t * .1 + .5);
  float t1 = fract(t * .1);
  float lerp = abs((.5 - t0) / .5);

  vec3 col;
  vec3 ndc = vec3(st, 0);

  {
    vec2 p = fract(st * 2.) - .5;

    float d = 1. - SM(0., .1, dot(p, p));

    col += d * hsv(vec3(dot(p, p), 1, 1));
  }

  {
    vec3 lin = normalize(vec3(col.xy, col.z + 1.));
    vec3 p = cross(ndc, lin);
    vec3 rd = -normalize(p / lin);

    float d0 = distort(p + t0 * rd);
    float d1 = distort(p + t1 * rd);

    float d = mix(d0, d1, lerp);

    col = vec3(d);
  }

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}
