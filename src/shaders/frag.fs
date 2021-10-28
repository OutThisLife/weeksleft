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

  float t = iTime;
  float t1 = .5 + .5 * sin((t * 3.));

  vec2 p = abs(st * 1.8);

  {
    float d = max(p.x + .45, p.y - .45);
    d = 1. - S(.5 + .03 * t1, d);

    col += vec3(1. - t1, 0., t1) * d;
  }

  {
    float d = max(p.x + .46, p.y - .44);
    d = 1. - S(.5, d);

    col = mix(col, vec3(t1, 0., 1. - t1), d);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
