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

vec2 hash(vec2 p) {
  p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
  return -1. + 2. * fract(sin(p) * 43758.5453123);
}

float noise(in vec2 p) {
  const float K1 = .366025404;  // (sqrt(3)-1)/2;
  const float K2 = .211324865;  // (3-sqrt(3))/6;

  vec2 i = floor(p + (p.x + p.y) * K1);
  vec2 a = p - i + (i.x + i.y) * K2;
  float m = step(a.y, a.x);
  vec2 o = vec2(m, 1. - m);

  vec2 b = a - o + K2;
  vec2 c = a - 1. + 2. * K2;
  vec3 h = max(.5 - vec3(dot(a, a), dot(b, b), dot(c, c)), 0.);
  vec3 n = pow(h, vec3(4.)) * vec3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)),
                                   dot(c, hash(i + 1.0)));
  return dot(n, vec3(70.0));
}

float fbm(in vec2 st) {
  const int o = 4;
  float value = 0.;
  float amp = .5;

  for (int i = 0; i < o; i++) {
    value += amp * noise(st);
    amp *= .5;
    st *= mat2(1.6, 1.2, -1.2, 1.6);
  }

  return .5 + .5 * value;
}

vec2 P(vec3 ro, vec3 rd, float s) { return (ro + s * rd).yx; }

void mainImage(in vec2 st) {
  vec3 col = vec3(.3, .5, .9);

  float v = .005;
  float t = iTime;

  vec3 ro = cameraPosition;
  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * vPos).xyz;

  float h = (1. - .1 / sqrt(pow(dot(st, st), 2.)));

  {
    vec2 p = P(ro, rd, 2.);
    vec2 turb = .03 * vec2(noise(p * 20.), noise(p * 20.));
    p += turb;

    float d =
        fbm(vec2(p.x - 5. * sin(iTime * v * 2.), p.y - 3. * sin(iTime * v)));

    float d1 = smoothstep(.5, .8, d);
    col += d1 * vec3(1., 1., 5.);
  }

  {
    vec2 p = P(ro, rd, .5);
    vec2 turb = .05 * vec2(noise(p * 1.), noise(p * 2.));
    p += turb;

    float d = fbm(p + 3. * sin(iTime * v));
    d /= 1.3;

    col += smoothstep(.2, .9, d);
  }

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}

void main() {
  vec2 st = (gl_FragCoord.xy - .5 * iResolution.xy) / iResolution.y;
  vec2 uv = ((vUv - .5) * iResolution.xy) / iResolution.y;

  mainImage(uv);
}