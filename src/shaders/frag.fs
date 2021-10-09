#version 300 es

precision highp float;

uniform float iGlobalTime;
uniform float iFrame;
uniform vec2 iMouse;
uniform vec3 iResolution;

uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

#define ZERO min(0, int(iFrame))
#define ap iResolution.x / iResolution.y;

in vec3 vUv;
in vec4 vPos;

out vec4 fragColor;

// ----------------------------------------------------------------------

#define PI 3.1415926535898
#define TWOPI 6.2831853071796
#define LOG2 1.442695
#define EPSILON .001

// ----------------------------------------------------------------------

mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }

void main() {
  vec2 st = (-iResolution.xy + 2. * gl_FragCoord.xy) / iResolution.xy;
  vec4 ndc = vec4(vUv.xy * 2. - 1., 1., 1.);

  vec2 mo = iMouse;

  vec3 ro = cameraPosition;
  vec4 idk = normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc);
  vec3 rd = idk.xyz / idk.w;

  vec3 col;
  float time = iGlobalTime * .1;

  col = fract(rd) - .5;

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(1. / 2.2)), 1.);
}
