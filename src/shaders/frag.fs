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
#define fsat(a) abs(abs(a) - .5)

// ---------------------------------------------------

float triangle(float a) { return abs(fract((a - 1.) / 4.) - .5) * 4. - 1.; }

const vec3 cOuter = vec3(.52, .69, .87);
const vec3 cInner = pow(cOuter, vec3(1.2));
const vec3 cBody = vec3(.22, .81, .95);
const vec3 cBG = vec3(.3, .3, 1.);

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
  vec3 st = vec3((vUv * 2. - 1.) * vResolution.z, 0.);
  vec2 mo = iMouse / vResolution.z;

  vec3 col;
  float t = iTime * .1;

  // Wings
  {
    float w = .5 + .5 * S(3., 1.5, 1. / distance(.5, st.y));
    mat3 m = mat3(vec3(w, 0., 0.), vec3(0., w, 0.), vec3(0., 0., 1.));

    vec3 p = abs(st * w);
    float a = atan(p.y, p.x) * 2.;
    float r = length(p) * PI;

    float d = cos(a);
    d = 1. - S(d - .02, d + .02, r);

    float sha = 1. - S(0., d / 1.5, r);
    col -= sha;

    // Tiny wings
    m = mat3(vec3(1., 0., 0.), vec3(0., -1., 2.), vec3(0., 1.1, -1.));
    p = abs((st + vec3(0., .3, .1)) * m);

    a = atan(p.y + .01, p.x - .01);
    r = length(p) * TWOPI;

    float d1 = cos(a);
    d += 1. - S(d1 - .05, d1, r);

    col += cOuter * saturate(fract(d));
    col += cInner * saturate(d);

    float tips = S(0., r / 3., 1. - a * sqrt(r / 2.));
    col *= saturate(cInner + tips);

    // Segments
    vec3 seg = d * p * r * 2.;
    vec3 gv = fract(seg * r) - .5;

    d = S(0., .03, fsat(gv.y - gv.x));

    gv = fract(cos(seg)) - .5;
    d *= S(0., .02, fsat(gv.x + gv.y));

    d = (1. - d) * a * .2;

    col += saturate(cInner * d);
  }

  // Body
  {
    vec3 p = abs(st * 2.2);
    float a = atan(p.x, p.y);
    float r = length(p), d = .5 / r;

    col += cBG * saturate(.3 / r);

    d = saturate(S(1.3, 2., d));
    float d1 = saturate(fract(S(.99, 1., d)) * .1);
    float d2 = saturate(S(.66, 0., r / .3));

    col = mix(col, cBody, d) + cBody * d1;
    col = mix(col, pow(cBody, vec3(.2)), d2);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 1.)), 1.);
}
