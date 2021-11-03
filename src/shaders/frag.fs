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
#define R2 vResolution
#define PI 3.1415926538
#define TWOPI 6.2831853076
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(v, r) smoothstep(3. / R2.y, 0., length(v) - r)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))
#define rangeFrom(a, b) ((b / -2.) - b * a)
#define rangeTo(a, b) ((b / -2.) + b * a)

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec3 col;

  float t = iTime;

  {
    float t = fract(t * .2);
    float t2 = 1. - abs(2. * t - 1.);
    float m = rangeTo(t, 4.);

    vec2 p = st - vec2(m, 0);
    float d = S(.8 + .2 * t2, 1. - length(p));

    col = mix(col, vec3(0, 0, 1. - m), d);

    p = st + vec2(m, .3);
    d = S(.8 + .2 * t2, 1. - length(p));

    col = mix(col, vec3(0, 1. - m, 0), d);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
