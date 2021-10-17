#version 300 es
precision highp float;

uniform vec3 iResolution;

in vec3 vUv;
in vec3 vNormal;
in vec4 vPos;

out vec4 fragColor;

#define PI 3.1415926538
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)

float rand(vec3 p) {
  return fract(sin(dot(p, vec3(12.9898, 78.233, 140.20))) * 43758.5453123);
}

void main() {
  vec3 col;

  vec3 st = vUv * iResolution.z;
  st *= 2. - 1.;

  vec3 p = st * 44.;
  p -= vec3(.49 / iResolution.z);

  vec3 gv = fract(p) - .1;
  vec3 fl = .1 + floor(gv);
  float len = length(p);

  {
    float i = dot(fl.x, fl.y);

    float y = saturate(1. / i / len) + .1;
    float x = cos(i * PHI) * y;

    col += vec3(saturate(1. - x));
  }

  {
    vec3 nor = normalize(vNormal);
    nor = refract(vNormal, gv, .05);

    float d = saturate(pow(dot(nor.x, nor.y), .5));
    col += vec3(1., 0., 0.) * (rand(gv / 2.) * d);
  }

  fragColor = vec4(pow(col, vec3(1. / 2.2)), 1.);
}