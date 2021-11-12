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

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SMP(v, r) SM(3. / Rpx.y, 0., length(v) - r)
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;

  float t = iTime / 2.;
  float t0 = fract(t * .5 + .5);
  float t1 = fract(t * .5);
  float lerp = abs((.5 - t0) / .5);

  {
    vec2 p = st;
    vec2 rd = -normalize(p) * .5;

    vec2 p0 = p + t0 * rd, p1 = p + t1 * rd;

    float d0 = 1. - SMP(p0, .01);
    float d1 = 1. - SMP(p1, .01);

    float d = mix(d0, d1, lerp);

    col = mix(col, hsv(vec3(rd.x - t1, 1, 1)), 1. - d);
  }

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}
