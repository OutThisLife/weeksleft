#version 300 es
precision mediump float;

uniform vec3 iResolution;
uniform float iTime;
uniform vec2 iMouse;

in vec2 vUv;

out vec4 fragColor;

#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)

// ------------------------------------------------------

const int MAX = 255;
const float tmax = float(MAX);

mat2 R(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

float triangle(float x) { return abs(fract((x - 1.) / 4.) - .5) * 4. - 1.; }

void main() {
  vec3 col;
  vec3 res = normalize(iResolution);
  vec2 st = (vUv * 2. - 1.) * res.xy;

  float zoom = 30.;

  vec2 C = 2. / zoom * st + vec2(-.89, .24);
  vec2 p = C;

  for (int i = 0; i < MAX; i++) {
    float fi = float(i);

    p = vec2(p.x * p.x - p.y * p.y, 2. * p.x * p.y) + C;

    if (dot(p, p) >= 4.) {
      float d = (.3 + .1 * triangle(fi / (tmax / 4.))) * 2.;
      col += vec3(d, d - .3, d + .5);

      break;
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}