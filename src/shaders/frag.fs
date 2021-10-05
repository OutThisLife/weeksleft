#version 300 es

precision highp float;

#define EPSILON 0.0005
#define MAX_STEPS 255
#define MIN_DIST 0.
#define MAX_DIST 60.
#define AA 1
uniform float iTime;
uniform int iFrame;
uniform vec3 iResolution;

in vec3 vUv;
in vec4 vPos;

out vec4 fragColor;

// ----------------------------------------------------------------------

#define PI 3.1415926535898
#define TWOPI 6.2831853071796
#define LOG2 1.442695
#define w .05 / iResolution.z

// clang-format off
#pragma glslify: import './lib.glsl'
// clang-format on

float grid(vec2 p, float s) {
  vec2 st = fract(p);
  return clamp(step(1. - s, st.x) + step(1. - s, st.y), 0., 1.);
}

float plot(vec2 st) {
  const int len = 13;

  float t = 0.;
  float idx = 0.;

  float lw = .4 * w / float(len);
  float an = iTime * 1.5;
  float n = (fbm(st) / rand(st.yy)) * (1. - .9 * (abs(sin(an))));

  for (int x = 0; x < len; x++) {
    for (int y = 0; y < len; y++) {
      if (x * x + y * y > len * len) {
        continue;
      }

      float xx = st.x + float(x) * lw;
      float yy = st.y + float(y) * lw;

      float d = abs(step(n + fract(10. * inversesqrt(pow(xx, 2.)) - an),
                         fract((abs(yy / xx)) + an)));

      d -= yy;

      t += (d >= 0.) ? 1. : -1.;
      idx++;
    }
  }

  if (abs(t) != idx) {
    return clamp((abs(t) / idx) * float(len), 0., 1.);
  }

  return 0.;
}

vec2 scale(vec2 p, float s) { return (p * s) - (s / 2.); }

// ----------------------------------------------------------------------

void main() {
  vec4 ndc = vec4(vUv.xy, 1., 1.);
  vec2 st = scale(ndc.xy, 10.);

  vec3 col = vec3(0.);
  col = mix(col, vec3(.001), grid(st, 0.01));
  col += plot(st);

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(.4545)), 1.);
}
