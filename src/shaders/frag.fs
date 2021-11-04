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

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec3 col;

  float t = iTime;

  {
    vec2 p = st;

    vec2 gv = fract(p) - .5;
    vec2 id = floor(p);

    float w = cos(4. * atan(gv.y, p.x));

    float x = fract(w - t * .5);
    float y = abs(2. * x - 1.);
    float m = rangeTo(y, 8.);

    p -= vec2(m, 0);

    col += y * SM(p, .1);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
}
