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

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 1.);
  vec2 mo = iMouse * (R.xy * 1.);
  vec3 col;

  float t = iTime;

  // Vars
  {

    vec2 p = st;
    vec2 dir = p * -.4;

    float dc = 1. - (length(p) * 2.);
    float pdc = pow(dc, 1.5);

    float t0 = fract(t * .3 + .5);
    float t1 = fract(t * .3);

    vec2 p0 = p + t0 * dir;
    vec2 p1 = p + t1 * dir;

    {
      vec2 o = vec2(0, .2);
      p0 += o;
      p1 -= o;

      float d0 = max(abs(p0).x, abs(p0).y);
      float d1 = max(abs(p1).x, abs(p1).y);

      float lerp = abs((.5 - t0) / .5);
      float d = mix(d0, d1, lerp);
      col += d;
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
}
