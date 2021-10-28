#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

// ---------------------------------------------------

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, c) smoothstep(a, b, c)
#define hue(h) .6 + .6 * cos(h + vec3(0, 23, 21))

// ---------------------------------------------------

float hash(vec2 p) {
  p = fract(p * vec2(123.34, 456.21));
  p += dot(p, p + 45.32);
  return fract(p.x * p.y);
}

void main() {
  vec3 col;
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 mo = iMouse * vUvRes.xy;

  float t = iTime / 4.;

  vec2 p = st * 20.;
  float a = atan(p.y, p.x), r = length(p) * TWOPI;

  float path = 1. - saturate(cos(pow(40. * r, .44) - a) + (r / 35.));

  {
    vec2 p = st * mat2(cos(t), sin(t), -sin(t), cos(t));
    p = mod(p + .16 * .5, .16) - .16 * .5;

    float r = length(p) * 2.;
    float a = atan(p.y, p.x);
    r *= cos(r * 5. * tan(a * 4.));

    float d = 1. - saturate(S(.02, r));

    col += vec3(0., .6, .5) * path * d;
  }

  col += vec3(.001) * path;

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
