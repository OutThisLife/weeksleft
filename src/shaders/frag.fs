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

  vec3 ro = 1. + .07 * cameraPosition;
  vec3 rd =
      normalize((cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz);

  vec3 col;
  float time = iGlobalTime * .1;

  for (int i = 0; i < 4; i++) {
    float depth = fract(float(i + 13) + time);

    vec3 p = (ro + (float(i + 13 / 3) * rd));
    vec3 gv = fract(abs(p)) - .5;
    vec3 id = floor(gv);

    for (int x = -1; x <= 1; x++) {
      for (int y = -1; y <= 1; y++) {
        vec3 uv = (gv - vec3(x, y, 0));

        float d = length(uv) - .1;
        d += d * (.03 / (dot(cross(gv, uv), gv) - dot(uv, uv)));

        float m = .013 / d;
        m += abs(.004 / d);
        m *= smoothstep(.8, .1, d);

        vec3 nor = normalize(uv - vec3(x, y, 0.));
        float dif = clamp(dot(nor, uv), 0., 1.);
        float amb = clamp(.5 + .5 * nor.y, 0., 1.);

        vec3 lin;
        lin += 1.5 * dif * vec3(.7);
        lin += 10. * amb * vec3(.1, 0., .9);

        col += lin * pow(m, 2.);
      }
    }
  }

  // col = mix(col, vec3(1., 0., 0.), s / 64.);

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(1. / 2.2)), 1.);
}
