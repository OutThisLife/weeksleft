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
#define R(a, b) mat2(a, b, -b, a)

// ---------------------------------------------------

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

float noise(in vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);

  float a = rand(i);
  float b = rand(i + vec2(1., 0.));
  float c = rand(i + vec2(0., 1.));
  float d = rand(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float fbm(vec2 p, float t, float amplitude, float s, float a) {
  float mask = length(p), amp = amplitude;

  for (int i = 0; i < 8; i++) {
    p *= R(s, a);
    t += noise(p) * amp;
    amp *= amplitude;
  }

  return t;
}

float fbm(vec2 p) { return fbm(p, .5, .5, 1.6, 1.2); }

// ---------------------------------------------------

const vec3 cIris = vec3(1., 0., 0.);
// const vec3 cIrisGrad = vec3(.9, .6, .2);
const vec3 cIrisGrad = hue(-2. * dot(cIris, cIris));
const vec3 cReflection = vec3(1., .9, .8);

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * vUvRes.xy;
  vec2 uv = vUv / vUvRes.xy;
  vec2 mo = iMouse;

  vec3 col;
  vec2 p = st;

  float r = length(p);
  float a = atan(abs(p.y), abs(p.x));
  float t = iTime * .2;

  {
    vec2 q = p - (vec2(0., .1) * R(t, 1.));
    a += .2 * fbm(10. * q, .5, .5, 1.6, 1.2);

    // Iris base
    {
      float d = 1. - fbm(p);
      col = mix(cIris, cIris + .5, d);
    }

    // Iris gradient
    {
      float d = 1. - SM(.1, .9, r * 2.);
      col = mix(col, cIrisGrad, d);
    }

    // Lines
    {
      float d = 1. - SM(.3, 1., fbm(vec2(6. * r, 20. * a)));
      col = mix(col, vec3(1.), d);

      d = 1. - SM(.4, .9, fbm(vec2(12. * r, 10. * a), .33, .33, 1.6, 1.2));
      d /= max(.1, SM(.45, 1., fbm(vec2(20. * a, 100. * r))));

      col *= d;
    }

    // Outline
    {
      float d = 1. - SM(.45, .6, r);
      col *= d;
    }

    // Pupil
    {
      float r = length(p - (mo / 15.));
      float d = SM(.2, .25, r * 2.);

      col *= d;

      // Reflection
      {
        vec2 p = (p * 2.) - vec2(.24, .33);
        float r = length(p);

        float d = 1. - SM(0., .4, r);
        col += cReflection * d;
      }
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
