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
  vec2 n = p;

  for (int i = 0; i < 8; i++) {
    n *= R(s, a);
    t += noise(n) * amp;
    amp *= amplitude;
  }

  return t - mask;
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) / vResolution.xy;
  vec2 mo = iMouse * vResolution.xy;

  vec3 col;
  float t = iTime;

  {
    vec2 p = abs(st);

    float a = atan(p.x, p.y);
    float sq = mod(a + (PI / 4.), PI / 2.) - (PI / 4.);
    float b = mod(a, .03 * vResolution.z);

    float r = length(p);
    r *= cos(sq) / 1. + dot(p.y, pow(p.y - p.x, 2.));

    vec2 q = vec2((1. / r) - .3 * t, (2. / r) - .4 * t);

    vec2 gv = fract(q / PI) - .5;

    float d = abs((gv.x + gv.y) - b) - .5;
    float f = triangle(d / 2.);

    float n = abs(fbm(fract(10. * gv - r), a, .25, .1, PI / sq));
    float f2 = saturate(.9 / n);

    col += saturate(abs(vec3((.03 / -f), (.02 / d), .01 / d * 4.)));
    col *= f2;

    col = mix(col, f2 * normalize(pow(-col, vec3(-1.))),
              saturate(f2 - r * 3.) / vResolution.z);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
