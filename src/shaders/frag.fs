#version 300 es
precision mediump float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vResolution;

out vec4 fragColor;

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b, c) smoothstep(a, b, c)

// ---------------------------------------------------

mat2 R(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }
mat2 R(float s, float a) { return mat2(s, -a, a, s); }

float triangle(float a) { return abs(fract((a - 1.) / 4.) - .5) * 4. - 1.; }

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vResolution.z;
  vec2 mo = iMouse * vResolution.xy;

  vec3 col;
  float t = iTime * .3;

  {
    vec2 p = abs(st);

    float r = length(p);

    float d0 = abs(abs(fract(3. * r - t) - .5) - .5);
    float d1 = abs(abs(fract(4. * d0 + t) - .5) - .5);

    d0 = step(.1, d0);
    d1 = step(.1, d1);

    col += vec3(.001 / d0, dot(d0, d1), .5 / d1);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
