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

// ---------------------------------------------------

void main() {
  vec3 col;
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 mo = iMouse * vUvRes.xy;

  float t = iTime * .5;

  float t1 = .5 + .5 * sin(t * 4.);
  float t2 = clamp(0., 5., t);
  t2 = saturate(.5 + .5 * sin(dot(t2, abs(st).y - t)));

  vec2 p = abs(st * 1.8);

  float d1 = max(p.x + .45, p.y - .45);
  d1 = 1. - saturate(S(.5 + .03 * t1, d1));

  float d2 = max(p.x + .46, p.y - .44);
  d2 = 1. - saturate(S(.5, d2));

  col += t2 * vec3(1. - t1, 0., t1) * d1;
  col = mix(col, vec3(t1, 0., 1. - t1), d2);

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
