#version 300 es

precision highp float;

#define EPSILON 0.0005
#define MAX_STEPS 255
#define MIN_DIST 0.
#define MAX_DIST 60.
#define AA 1

#define PI 3.1415926535898
#define TWOPI 6.2831853071796
#define LOG2 1.442695

uniform float iTime;
uniform int iFrame;
uniform vec3 iResolution;

uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec3 vUv;
in vec4 vPos;
out vec4 fragColor;

// ----------------------------------------------------------------------

vec2 sceneSDF(vec3 p, float s);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

const vec3 palette[] =
    vec3[](vec3(.7, .8, .9),  // Sky
           vec3(1.),          // Floor
           vec3(0.0001, 0., 0.001), vec3(0.0001, 0., 0.001), vec3(#f36));

vec2 sceneSDF(vec3 p, float s) {
  vec2 res = vec2(sdPlane(p, vec3(0., 1., 0.), 0.), 1.);

  {
    vec3 q = p - vec3(-1.7, 1.25, -1.);

    float d1 = sdSphere(q, 1. * s);
    float d2 = sdTriPrism(q, vec2(.5, 0.01), 3. * s);

    float d = opSmoothSubtraction(d1, d2, .6);

    float d3 = sdTriPrism(q, vec2(.5, 0.01), 1. * s);
    float d4 = sdOctahedron(opRepLim(q, 2., vec3(.08, .08, 0.)), .1 * s);

    d = opUnion(opExtrusion(vec3(0., 0., .04), d3, d4), d);

    res = opU(res, vec2(d, 2.));
  }

  {
    vec3 q = p - vec3(2., 2., 0.);
    q = rotate(q, vec3(1., 0., 0.), 1.5);

    float d1 = opOnion(opOnion(opOnion(sdSphere(q, 1. * s), .5), .25), .15);
    d1 = max(d1, q.y);

    q += vec3(0., 2., 0.);
    q = rotate(q, vec3(0., 0., 1.), 3.5);

    float d2 =
        opOnion(opOnion(opOnion(sdSphere(q, 1. * s), .5), .5), .01 + fbm(p.zy));
    d2 = max(d2, q.y);

    float d = opSmoothIntersection(d1, d2, .5);

    res = opU(res, vec2(d, 3.));
  }

  {
    vec3 q = p - vec3(0.1, 4., -2.);

    q = rotate(q, vec3(0., 0., 1.), 1.);
    q.xy = rotate(q.xy, 5.);

    float d1 = sdSphere(q, 1. * s);
    float d2 = sdSphere(q - vec3(0., .3, 0.), 1. * s);

    float d = opExtrusion(q, d2, d1);

    res = opU(res, vec2(d, 4.));
  }

  return res;
}

vec3 getColor(vec3 p, vec3 ro, vec3 rd, int id) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(.4, 1., 2.));
  vec3 ref = reflect(rd, nor);
  vec3 hal = normalize(lig - rd);

  float ndotl = abs(dot(-rd, nor));
  float rim = pow(1. - ndotl, 4.);

  // lighting
  float occ = calcAO(p, nor);                        // ambient occlusion
  float amb = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));  // ambient
  float dif = clamp(dot(nor, lig), 0., 1.);          // diffuse

  // backlight
  float bac = clamp(dot(nor, normalize(vec3(-lig.x, 0., -lig.z))), 0., 1.) *
              clamp(1. - nor.y, 0., 1.);
  float dom = smoothstep(-0.1, 0.1, ref.y);               // dome light
  float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 8.);  // fresnel
  float spe = pow(clamp(dot(ref, hal), 0., .94), 30.);    // specular

  dif *= calcSoftshadow(p, lig, 0.02, 2.5, 8.);

  vec3 lin = vec3(0.);

  vec3 c = id < 0 ? vec3(0.1) : palette[id];

  if (id < 0) {
    lin += .25 * dif * c;
    lin += .5 * spe * c * dif;

    return lin;
  }

  if (id == 1) {
    lin += 0.01 * dif * c;
    lin += 0.01 * dom * c;
    lin += 0.01 * refract(-rd, nor, .1);

    return lin;
  }

  lin += 1.30 * dif * c;
  lin += 0.40 * amb * c * occ;
  lin += 0.25 * fre * c * occ;
  lin -= 0.1 * bac;

  lin += refract(-rd, nor, .85) * smoothstep(0., .1, rim);

  return lin;
}

void reflectionRay(vec3 ro, vec3 rd, inout vec3 col) {
  vec2 res = castSmallRay(ro, rd, 20.);
  float t = res.x;
  float m = res.y;

  if (m > 1. && t >= EPSILON && t < MAX_DIST) {
    vec3 p = ro + (t * rd);

    col = mix(col, getColor(p, ro, rd, -1), .05);
  }
}

void main() {
  vec4 ndc = vec4(vUv.xy - vec2(.5, .33), 1., 1.);
  vec3 ro = cameraPosition;
  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz;

  vec3 col = vec3(1.);
  vec3 res = castRay(ro, rd);
  float t = res.x, m = res.y;

  if (t >= EPSILON && t < MAX_DIST) {
    vec3 p = ro + (t * rd);
    vec3 nor = calcNormal(p);

    if (m == 1.) {
      col = vec3(1.) * checkers(p, 5.);
    }

    col *= getColor(p, ro, rd, int(m));
    reflectionRay(p + ro * EPSILON, reflect(rd, nor), col);
  }

  col = mix(col, vec3(0.), 1. - exp2(-EPSILON * pow(t, 2.5)));

  if (t >= MAX_DIST) {
    vec2 st = ndc.xy / tan(ndc.y - ndc.z);
    vec2 dots = fract(100. * st) - .5;

    float r = 1. - .264 * sin(50. * distance(dots, ndc.yx) + (iTime * 1.5));
    float d = smoothstep(r - r * .08, r, length(dots));

    col = vec3(1.) * d;
  }

  fragColor = vec4(pow(clamp(col, 0., 1.), vec3(1. / 2.2)), 1.);
}
