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
#define hue(h) (.6 + .6 * cos(h + vec3(0, 23, 21)))
#define R(a, b) mat2(a, -b, b, a)
#define H(a) fract(sin(a) * 753.5453123)

// ---------------------------------------------------

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 uv = vUv * vUvRes.xy;
  vec2 m = iMouse * vUvRes.xy;

  float t = iTime;

  vec3 col;

  {
    vec2 p = st * 2.;

    float x = p.x, y = p.y;
    float d, r = length(p), a = atan(y, x), s = dot(p, p);

    d = 1. - (saturate(1. - r / 2.) * sin(3. * a + 12. - 12. * sqrt(s)));
    d = abs((y + 1.2) - pow(x, 2.));
    d = abs(y - sin(pow(x, 3.)));

    d = 1. - SM(d * 10., d, rand(p));

    col += d;
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
