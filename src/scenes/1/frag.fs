#version 300 es
precision highp float;

// ---------------------------------------------------

#define R vUvRes
#define Rpx vResolution
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.61803398875
#define TAU 1.618033988749895

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(0., r / Rpx.x, v)
#define dot2(v) dot(v, v)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

uniform vec2 iMouse;
uniform float iTime;
uniform float uProgress;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// ---------------------------------------------------

float line(vec2 p, float d) {
  return SM(0., .5 + d * .5, .5 * abs(sin(p.x * 30.) + d * 2.));
}

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(in vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);

  float a = rand(i);
  float b = rand(i + vec2(1, 0));
  float c = rand(i + vec2(0, 1));
  float d = rand(i + vec2(1));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

// ---------------------------------------------------

vec2 distort(vec2 p, float pr, float e) {
  p = 2. * p - 1.;
  p /= (1. - pr * length(p) * e);

  return (p + 1.) * .5;
}

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;
  float t = iTime * .5;
  float pr = uProgress;

  {
    vec2 p = vPos.xy;
    // p = distort(p, (1. - pr) * -10., pr * 2.);
    // p *= rot(noise(p * 5. + t)) * .18;

    float d0 = line(p, .4);
    float d1 = line(p, .1);

    col = mix(#e09442, #789e71, d0);
    col = mix(col, #000, d1);
  }

  {
    vec2 p = vPos.xy;
    float d = length(p);

    col = mix(vec3(0), vec3(1, 0, 0), 1. - SM(0., pr, SM(0., .5, d)));
  }

  fragColor = vec4(saturate(col), 1.);
}
