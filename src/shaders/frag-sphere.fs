#version 300 es
precision mediump float;

uniform vec3 iResolution;
uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vNormal;
in vec4 vPos;

out vec4 fragColor;

#define PI 3.1415926538
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)

vec2 N(float a) { return vec2(sin(a), cos(a)); }
float pixel(float a) { return a / max(iResolution.x, iResolution.y); }

void mainImage(in vec2 st) {
  vec3 col;

  vec2 uv = st * 1.25;
  vec2 m = iMouse * iResolution.xy / iResolution.xy;

  float md = (distance(m * 1.25, uv / vPos.xy)) / 2.;

  uv.x = abs(uv.x);
  uv.y += tan(.833 * PI) * .5;

  vec2 n = N(.833 * PI);
  float d = dot(uv - vec2(.5, 0.), n);

  uv -= max(0., d) * n * 2.;

  const int steps = 10;
  const float t = 3.;
  float scale = 1.;

  uv.x += .5;
  n = N(md * .666 * PI);

  for (int i = 0; i < steps; i++) {
    uv *= t;
    scale *= t;
    uv.x -= t / 2.;

    uv.x = abs(uv.x) - .5;
    d = dot(uv, n);
    uv -= min(0., d) * n * 2.;
  }

  d = length(uv - vec2(clamp(uv.x, -1., 1.), 0.));
  col += smoothstep(pixel(3.), 0., d / scale);

  // col += smoothstep(0., .5, abs(m).x);

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}

void main() {
  vec2 st = (gl_FragCoord.xy - .5 * iResolution.xy) / iResolution.y;
  vec2 uv = ((vUv - .5) * iResolution.xy) / iResolution.y;

  mainImage(uv);
}