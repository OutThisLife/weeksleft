#version 300 es

precision highp float;

uniform float iGlobalTime;
uniform float iFrame;
uniform vec2 iMouse;
uniform vec3 iResolution;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec3 vUv;
in vec4 vPos;
in vec4 vWorld;
in vec3 cPos;

out vec4 fragColor;

#define ZERO min(0, int(iFrame))
#define PI 3.1415926535898
#define TWOPI 6.2831853071796
#define LOG2 1.442695

// ----------------------------------------------------------------------

const float scale = 4.;

void main() {
  vec4 ndc = vec4(vUv.xyz, 1.) * 2. - 1.;
  vec2 mo = iMouse;
  ndc *= scale;
  mo *= scale;

  vec3 ro = cPos;
  vec4 rd = normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc);

  float w = atan(rd.w, vPos.w / scale);
  ndc /= w;
  mo /= w;

  vec3 col;

  vec2 gv = fract(ndc.xy) - .5;
  vec2 id = floor(gv);

  float d = clamp(step(0., .3 - length(id)), 0., 1.);
  float dist = step(0., .3 - distance(ndc.xy, mo));

  col += vec3(.5) * d;
  col = mix(col, vec3(1., 0., 0.), d * dist);

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(1. / 2.2)), 1.);
}
