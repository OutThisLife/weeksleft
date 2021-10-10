#version 300 es

precision highp float;

uniform float iGlobalTime;
uniform float iFrame;
uniform vec2 iMouse;
uniform vec3 iResolution;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrix;
uniform mat4 cameraProjectionMatrixInverse;
uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;

in vec3 vUv;
in vec4 vPos;
in vec3 ro;
in vec3 rd;

out vec4 fragColor;

#define ZERO min(0, int(iFrame))
#define PI 3.1415926535898
#define TWOPI 6.2831853071796
#define LOG2 1.442695

#define EPSILON .0005
#define MAX_STEPS 255
#define MIN_DIST 0.
#define MAX_DIST 60.
#define AA 1

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './noise.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

vec2 sceneSDF(vec3 p, float s) {
  vec2 res = vec2(sdPlane(p, vec3(0., 1., 0.), 1.5), 0.);
  float an = iGlobalTime * 1.5;

  {
    vec3 q = p - vec3(0., 0., 0.);
    float d = sdSphere(q, 1. * s);

    res = opU(res, vec2(d, 2.));
  }

  return res;
}

void main() {
  vec3 col = vec3(1.);

  vec2 st = (-iResolution.xy + 2. * gl_FragCoord.xy) / iResolution.xy;
  vec3 mo = vec3(iMouse, 1.);

  vec3 res = castRay(ro, rd);
  float t = res.x, m = res.y, id = res.z;

  float b = float(t >= EPSILON && t < MAX_DIST);
  float k = b * float(m > 1.), v = b * (1. - k);

  vec3 p = b * (ro + (t * rd));

  {
    vec2 q = (st / 1.5);

    float d = 1. - checkers(p, iResolution.z);
    vec3 lin = vec3(.1);
    float f = 2. / (1. + pow(t * d, 2.));

    col = mix(col * f, lin * v, f);
  }

  {
    vec3 nor = calcNormal(p);
    vec3 lig = normalize(vec3(1., 1., -1.));

    float occ = calcAO(p, nor);
    float amb = saturate(.5 + .5 * nor.y);
    float dif = saturate(dot(nor, lig));
    float mouseDist = distance(mo.xy, cross(normalize(p), nor).xy);

    dif *= calcSoftshadow(p, lig, 0.02, 2.5);

    vec3 lin, c = vec3(1.);
    lin += 1.30 * dif * c;
    lin += 0.40 * amb * c * occ;

    col = mix(col, lin, k);
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1.);
}
