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
#define SMP(v, r) smoothstep(3. / Rpx.y, 0., length(v) - r)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define rangeFrom(a, b) ((b / -2.) - b * a)
#define rangeTo(a, b) ((b / -2.) + b * a)

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

vec3 hsv(float h, float s, float v) { return hsv(vec3(h, s, v)); }

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec2 mo = iMouse * (R.xy * 2.);
  vec3 col;

  float t = iTime;

  {
    vec2 p = st;

    float s = TWOPI / 5.;
    float a = atan(p.y, p.x);
    float b = mod(a + s * .5, s) - s * .5;
    float r = length(p) * 2.;

    p = r * vec2(cos(b), sin(b));
    p -= vec2(.75, 0) * abs(sin(t));

    float d = sqrt(b * b * (r / .1)) + 2. - 2.25 * r;
    d = length(length(p) / vec2(cos(d), sin(d))) - 1.;

    d = 1. - d;

    col += hsv(vec3(a / TWOPI, 1, 1)) * d;
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
}
