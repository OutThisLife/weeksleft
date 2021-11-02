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
#define hue(h) (.6 + .6 * cos((6.3 * h) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec3 col;

  float t = iTime;

  const int STEPS = 63;
  const float GAP = 4.;
  float res = 1e3;

  for (int i = STEPS; i > 0; i--) {
    float fi = float(i);

    float w = fi / float(STEPS);
    float t = fract(w + t * .2);

    float a = fi / 16. + t * .1;
    vec2 o = rot(a * TWOPI) * vec2(GAP + GAP * 2. * t, 0);

    vec2 p = st * pow(GAP, 1.8) - o;

    float d = saturate(1. - abs(2. * t - 1.));

    res = min(res, 1. - SM(p, d));
  }

  {
    vec2 p = st;
    float r, l = length(p), a = atan(p.y, p.x);

    l *= 4.;
    r = l - a / TWOPI;
    a += TWOPI * (ceil(r) - .5);
    a *= (a / 2.) * (PI / TWOPI);

    float u = (fract(a) - .5) / PI;
    float v = fract(r) - .5;
    float i = floor(a);

    float t = fract(t * .2);
    vec2 o = vec2(u, v);

    col += hue(i / 9.) * SM(o, .1);
  }

  col += res;

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
