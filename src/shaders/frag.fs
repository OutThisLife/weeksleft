#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
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

vec4 sparkles(vec3 p, float s) {
  vec3 col;

  float a = atan(p.x, p.y);
  float r = length(p);

  float d = saturate(S(1.3, 2., s / r));
  float outline = saturate(fract(S(.99, 1., d)) * .1);
  float glow = saturate(S(.66, 0., r / (s - .1)));

  col += cBody * d;
  col += cBody * outline;
  col += pow(cBody, vec3(.2)) * glow;

  return saturate(vec4(col, d));
}

vec3 hueShift(vec3 c, float s) {
  vec3 m = vec3(cos(s), s = sin(s) * .5774, -s);
  return c * mat3(m += (1. - m.x) / 3., m.zxy, m.yzx);
}

// ---------------------------------------------------

void main() {
  vec3 col;
  vec3 st = vec3(vUv * 2. - 1., 0.) * vUvRes;
  vec3 mo = vec3(iMouse, 0.) * vUvRes;

  float t = iTime;
  float t4 = abs(fract(t * .5) - .5) / .5;
  float bou = abs(-1. + 2. * t4);

  st.y += .01 * bou;

  // Wings
  {
    float w = .5 + .5 * S(3., 2., 1. / distance(.45, st.y));
    mat3 m = mat3(vec3(w, 0., 0.), vec3(0., w, 0.), vec3(0., 0., 1.));

    vec3 p = abs(st * 1.3 * m);
    p.x += .008 * bou;

    float a = atan(p.y, p.x) * 2.;
    float r = length(p) * PI;

    float d = cos(a);
    d = 1. - S(d - .02, d + .02, r);

    // Shading
    col -= 1. - S(0., d / 1.5, r);

    col += saturate(cOuter * fract(d));
    col += saturate(cInner * d);

    // Tiny wings
    m = mat3(vec3(1., 0., 0.), vec3(0., -1., 2.), vec3(0., 1.1, -.95));
    p = abs((st + vec3(0., .28, .1)) * m);

    a = atan(p.y * 1.2 + .01, p.x - .01);
    r = length(p) * TWOPI;

    float d1 = cos(a);
    d1 = 1. - S(d1 - .02, d1, r / .95);

    col += saturate(pow(cInner, vec3(2.5)) * (d1 + fract(d1)));

    float tips = S(0., r / 3., 1. - a * sqrt(r / 3.));
    col *= saturate(cInner + tips);

    // Segments
    vec3 seg = d * p * r * PI;

    vec3 gv = fract(seg * r) - .5;
    d = S(0., .03, fsat(gv.y - gv.x));

    gv = fract(cos(seg)) - .5;
    d = min(d, S(0., .02, fsat(gv.x + gv.y)));

    col += saturate(cInner * (1. - d) * a * .3);
    col = hueShift(col, bou);
  }

  // Body
  {
    vec4 lin = sparkles(abs(st * 3.), .5);

    {
      vec3 p = abs((st + vec3(-.13, .17, 0.)) * 10.);
      lin += sparkles(p, .2 + (.01 * bou));
    }

    {
      vec3 p = abs((st + vec3(-.16, .2, 0.)) * 10.);
      lin += sparkles(p, .1 + (.015 * bou));
    }

    col = mix(col, lin.xyz, lin.w);
  }

  col += cBG * (1. - dot(st, st));

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
