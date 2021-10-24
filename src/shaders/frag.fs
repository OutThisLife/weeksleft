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
#define ST(a, b) step(a, b)

// ---------------------------------------------------

float triangle(float a) { return abs(fract((a - 1.) / 4.) - .5) * 4. - 1.; }

const vec3 cInner = vec3(.74, .85, .95);
const vec3 cOuter = vec3(.52, .69, .87);
const vec3 cBody = vec3(.22, .81, .95);
const vec3 cBG = vec3(.3, .2, 1.);

// ---------------------------------------------------

void sparkle(vec2 p, inout vec3 col) {
  p = abs(p);
  float r = length(p), d = .5 / r;

  float body = saturate(S(1.3, 2., d));
  float outline = saturate(fract(S(.99, 1., body)));

  col = mix(col, cBody, body) + cBody * outline;
  col = mix(col, pow(cBody, vec3(.3)), S(.7, 0., r / .3));
}

void main() {
  vec2 st = (vUv * 2. - 1.) / vResolution.z;
  vec2 mo = iMouse / vResolution.z;

  vec3 col;
  float t = iTime * .1;

  // Wings
  {
    float w = .5 + .5 * smoothstep(3., 1., 1. / distance(.5, st.y));
    vec2 p = abs(st * mat2(w, 0., 0., w));
    float a = atan(p.y, p.x) * 2.;
    float r = length(p) * PI;

    float d = cos(a);
    d = 1. - smoothstep(d, d + .09, r);

    col += saturate(cOuter * fract(d));
    col += saturate(cInner * d);
  }

  // cBody
  {
    vec2 p = st * 2.2;

    {
      vec2 q = abs(p);
      float r = length(q);
      float a = atan(p.x, p.y);

      float tips = S(1., .7, sqrt(a / r));
      col *= saturate(cOuter + tips);

      float sha = S(-.1, .7, r);
      col *= saturate((cOuter / 2.) + sha);
    }

    {
      vec2 q = abs(p);
      float r = length(q), d = .5 / r;

      col += cBG * saturate(.3 / r);

      float body = saturate(S(1.3, 2., d));
      float outline = saturate(fract(S(.99, 1., body)));

      col = mix(col, cBody, body) + cBody * outline;
      col = mix(col, pow(cBody, vec3(.3)), S(.7, 0., r / .3));
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 1.)), 1.);
}
