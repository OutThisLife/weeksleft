#version 300 es
precision highp float;

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

const vec3 cOuter = vec3(.52, .69, .87);
const vec3 cInner = pow(cOuter, vec3(1.2));
const vec3 cBody = vec3(.22, .81, .95);
const vec3 cBG = vec3(.12, .1, 1.);

void sparkles(vec3 p, float s, vec3 tmp, inout vec3 col) {
  float a = atan(p.x, p.y);
  float r = length(p);

  float d = saturate(S(1.3, 2., s / r));
  float outline = saturate(fract(S(.99, 1., d)) * .1);
  float glow = saturate(S(.66, 0., r / (s - .1)));

  col = mix(col, tmp, d) + tmp * outline;
  col = mix(col, pow(tmp, vec3(.2)), glow);
}

float Hash21(vec3 p) {
  p = fract(p * vec3(123.34, 456.21, 0.));
  p += dot(p, p + 50.);
  return fract(p.x * p.y);
}

// ---------------------------------------------------

void main() {
  vec3 col;
  vec3 st = vec3(vUv * 2. - 1., 0.) * vResolution;
  vec3 mo = vec3(iMouse, 0.) * vResolution;

  float t = iTime * .1;

  // Wings
  {
    float w = .5 + .5 * S(3., 2., 1. / distance(.45, st.y));
    mat3 m = mat3(vec3(w, 0., 0.), vec3(0., w, 0.), vec3(0., 0., 1.));

    vec3 p = abs(st * 1.3 * m);
    float a = atan(p.y, p.x) * 2.;
    float r = length(p) * PI;

    float d = cos(a);
    d = 1. - S(d - .02, d + .02, r);

    // Shading
    col -= 1. - S(0., d / 1.5, r);

    // Tiny wings
    m = mat3(vec3(1., 0., 0.), vec3(0., -1., 2.), vec3(0., 1.1, -1.));
    p = abs((st + vec3(0., .3, .1)) * m);

    a = atan(p.y + .01, p.x - .01);
    r = length(p) * TWOPI;

    float d1 = cos(a);
    d += 1. - S(d1, d1 + .02, r * 1.1);

    col += cOuter * saturate(fract(d));
    col += cInner * saturate(d);

    float tips = S(0., r / 3., 1. - a * sqrt(r / 3.));
    col *= saturate(cInner + tips);

    // Segments
    vec3 seg = d * p * r * 2.;
    vec3 gv = fract(seg * r) - .5;

    d = S(0., .03, fsat(gv.y - gv.x));

    gv = fract(cos(seg)) - .5;
    d *= S(0., .02, fsat(gv.x + gv.y));

    d = (1. - d) * a * .3;

    col += saturate(cInner * d);
  }

  // Body
  {
    vec3 p = abs(st * 3.);
    col += cBG * saturate(.2 / length(p));

    {
      vec3 p = abs((st + vec3(-.13, .17, 0.)) * 10.);
      sparkles(p, .2, cBody, col);
    }

    {
      vec3 p = abs((st + vec3(-.16, .2, 0.)) * 10.);
      sparkles(p, .1, cBody, col);
    }

    sparkles(p, .5, cBody, col);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
