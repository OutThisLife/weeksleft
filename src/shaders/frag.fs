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

// clang-format off
#pragma glslify: import { scale } from './lib.glsl'
#pragma glslify: import { sdPlane } from './shapes.glsl'
#pragma glslify: import './noise.glsl'
// clang-format on

// ----------------------------------------------------------------------

void main() {
  vec3 col;

  vec4 ndc = vec4(vUv.xy - vec2(.5), 1., 1.);
  vec3 ro = cameraPosition;
  vec3 mo = vec3(iMouse, ro.z);
  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz;

  vec3 p = (sdPlane(ro, rd, 0.) / dot(rd, normalize(ro))) * rd + ro;
  float intensity = 1. / dot(p, p);
  p += cross(p, rd);

  float s = 0., fade = 1.;
  ro += iGlobalTime / 3.;
  rd = mix(rd, p, -intensity);

  for (int i = 0; i < 14; i++) {
    vec3 p2 = ro + (rd * s);
    vec3 v = abs(1. - mod(p2, 2.));
    float pa, a = pa = 0.;

    for (int n = 0; n < 10; n++) {
      v = abs(v) / dot(v, v) - 1.;
      a += abs(length(v) - pa);
      pa = length(v);
    }

    a *= pow(a, 2.);

    if (i >= 5) {
      fade *= 1. - max(0., 1. - a * fbm(v.xy));
    }

    col = mix(col, vec3(s, pow(s, 2.), pow(s, 3.)), a * .0001 * fade);
    col = mix(col, vec3(0.), smoothstep(intensity, 0., length(p)));

    fade *= .5;
    s += .2;
  }

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(1. / 2.2)), 1.);
}
