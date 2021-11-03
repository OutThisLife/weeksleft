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

  const int STEPS = 3;
  const float GAP = 8.;

  {
    vec2 p = st * GAP;

    for (int i = STEPS; i > 0; i--) {
      float fi = float(i);

      float t = fract((fi / float(STEPS)) + t * .2);
      float d = saturate(1. - abs(2. * t - 1.));

      vec2 o = vec2(rangeTo(t, GAP), 0);

      col += SM(p - o, d);
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
