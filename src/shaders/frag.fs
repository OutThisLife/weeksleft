#version 300 es
precision mediump float;

uniform vec3 iResolution;
uniform vec2 iMouse;
uniform float iTime;
uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec2 vUv;
in vec3 vNormal;
in vec4 vPos;

out vec4 fragColor;

#define PI 3.1415926538
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)

// ------------------------------------------------------

vec2 hash(vec2 p) {
  p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
  return -1.0 + 2.0 * fract(sin(p) * 43758.5453123);
}

float N(float t) { return fract(sin(t * 12345.564) * 7658.76); }

float noise(in vec2 p) {
  const float K1 = 0.366025404;  // (sqrt(3)-1)/2;
  const float K2 = 0.211324865;  // (3-sqrt(3))/6;
  vec2 i = floor(p + (p.x + p.y) * K1);
  vec2 a = p - i + (i.x + i.y) * K2;
  vec2 o = (a.x > a.y)
               ? vec2(1.0, 0.0)
               : vec2(0.0, 1.0);  // vec2 of = 0.5 + 0.5*vec2(sign(a.x-a.y),
                                  // sign(a.y-a.x));
  vec2 b = a - o + K2;
  vec2 c = a - 1.0 + 2.0 * K2;
  vec3 h = max(0.5 - vec3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
  vec3 n =
      h * h * h * h *
      vec3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));
  return dot(n, vec3(70.0));
}

mat2 rot(float s, float a) { return mat2(s, -a, a, s); }

float pat(vec2 p, float t, float amplitude, float s, float a) {
  float mask = length(p), amp = amplitude;
  vec2 n = p;

  for (int i = 0; i < 8; i++) {
    n *= rot(s, a);
    t += noise(n) * amp;
    amp *= amplitude;
  }

  return t - mask;
}

vec3 mainImage(in vec2 st) {
  vec3 col;

  vec3 nor = normalize(vNormal);
  vec3 ro = cameraPosition;
  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * vPos).xyz;
  vec2 mo = iMouse / 4.;

  float t = iTime * .3;

  {
    vec2 p = st * 2.5;
    vec2 pt =
        vec2(fract(mo.x + p.x - (t / 10.)), fract(mo.y + p.y - (t / 15.)));

    float d = pat(p, .5, .4, noise(10. * pt), 4.);

    col += vec3(1., .0, .4) * smoothstep(0., .9, d);
  }

  {
    vec2 p = abs(st);

    float d = pat(p, .09, .8, 1.6, noise(p + p));

    col += vec3(0., 1., .5) * smoothstep(-.1, .7, d);
  }

  return col;
}

void main() {
  vec2 st = (gl_FragCoord.xy - .5 * iResolution.xy) / iResolution.y;
  vec2 uv = ((vUv - .5) * iResolution.xy) / iResolution.y;

  fragColor = vec4(saturate(mainImage(uv)), 1.);
}