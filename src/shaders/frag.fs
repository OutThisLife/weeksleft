precision mediump float;

#define EPSILON 0.001
#define MAX_STEPS 255
#define MAX_DIST 80.
#define AA 1

uniform float iTime;
uniform int iFrame;
uniform vec3 iResolution;

uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

varying vec3 vUv;

// ----------------------------------------------------------------------

float an = abs(.5 * (iTime - 10.));
const vec3 primary = vec3(1., .2, .4);
const vec3 secondary = vec3(0., 0., .933);
const vec3 gold = vec3(.831, .686, .216);

vec4 map(vec3 p, float s);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

vec4 map(vec3 p, float s) {
  vec4 res = vec4(1e10);

  for (int i = 0; i <= 1; i++) {
    vec3 q = p - vec3(i == 0 ? .8 : -.8, .5, 0.);

    float d1 = sdBox(q, vec3(.25) * s);

    float d2 =
        sdTriPrism(rotate(q, vec3(0., 0., 1.), 2. * an), vec2(.5), .2 * s);

    float d3 =
        sdTriPrism(rotate(q, vec3(0., 0., -1.), 2. * an) - vec3(0., 0., .2),
                   vec2(0.5, 0.1), .08 * s);

    float d4 =
        sdTriPrism(rotate(q, vec3(0., 0., 1.), 2. * an) - vec3(0., 0., .2),
                   vec2(0.5), .01 * s);

    float d = opSmoothSubtraction(
        d4, opUnion(opSmoothSubtraction(d2, d1, .01), d3), .01);

    res = opU(res, vec4(d, primary));
  }

  // for (int i = 0; i <= 1; i++) {
  //   float n = float(i);
  //   vec3 q = p;

  //   q = opRepLim(q - vec3(0, .5, 0.), 1.5 + n,
  //                vec3(.1, .1 + (0.01 * n), 0. + (0.2 * n)));

  //   if (i == 1) {
  //     q = rotate(q, vec3(-1., 1., 0.), 2. * an);
  //   }

  //   float d = sdOctahedron(q, 0.1 * s);

  //   float d1 = sdOctahedron(q, .09 * s);
  //   float d2 = sdBoundingBox(q, vec3(.1) * s, .01 * s);

  //   if (i == 1) {
  //     d2 = sdSphere(q, .025 * s);

  //     d = opSmoothSubtraction(d2, d1, 0.09);
  //     res = opU(res, vec4(d, gold));
  //   } else {
  //     float disp = smoothstep(.15, d,
  //                             length(vec3(sin((2. + (1. * sin(an))) * q.x),
  //                                         sin((2. + (1. * sin(an))) * q.y),
  //                                         sin((2. + (1. * cos(an))) *
  //                                         q.z))));
  //     d = opUnion(d1 + disp, d2);
  //     res = opU(res, vec4(d, vec3(1.)));
  //   }
  // }

  return res;
}

vec3 getColor(vec3 p, vec3 ro, vec3 rd, vec3 col, bool r) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(.9, .4, -.4));
  vec3 ref = reflect(rd, nor);
  vec3 hal = normalize(lig - rd);

  float ndotl = abs(dot(-rd, nor));
  float rim = pow(1. - ndotl, 4.);

  float occ = calcAO(p, nor);                        // ambient occlusion
  float amb = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));  // ambient
  float dif = clamp(dot(nor, lig), 0., 1.);          // diffuse

  float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 8.);  // fresnel
  float spe = pow(clamp(dot(ref, hal), 0., .94), 30.);    // specular

  vec3 lin = col * amb;
  lin += .2 * dif * occ;
  lin += 2. * spe * dif;
  lin += 2. * fre * dif;

  if (r) {
    lin += sin(mix(refract(nor, rd, .85), vec3(1.), rim));
  }

  return lin;
}

void reflectionRay(vec3 ro, vec3 rd, inout vec4 col) {
  float t = castRay(ro, rd);

  vec3 p = ro + t * rd;
  col = mix(col, vec4(getColor(p, ro, rd, vec3(1.), false), 1.), .2);
}

void render(vec3 ro, vec3 rd, inout vec4 col) {
  float t = castRay(ro, rd);

  vec3 p = ro + t * rd;
  vec4 h = map(p, 1.);

  if (t >= MAX_DIST) {
    vec3 p = vec3(0., 1., 0.);
    vec3 planePoint = rayPlaneIntersection(ro, rd, vec4(p, 0.));
    col.xyz *= vec3(.002, .001, .02) +
               mix(.95, .99, calcSoftshadow(planePoint, p, .02, 2.5, 8.));
  } else if (t > EPSILON) {
    col = vec4(getColor(p, ro, rd, h.yzw, false), 1.);
    // reflectionRay(p + rd * EPSILON, reflect(rd, calcNormal(p)), col);
  }
}

void main() {
  vec4 ndc = vec4(vUv.xy - vec2(.5, .33), 1., 1.);

  vec3 ro = vec3(cameraPosition.x, max(.1, cameraPosition.y), cameraPosition.z);

  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz;

  vec4 col = vec4(1.);

  for (int i = 0; i < AA; i++) {
    render(ro, rd, col);
  }

  // col *= pow(col, vec4(vec3(.4545), 1.));

  gl_FragColor = clamp(col, 0., 1.);
}
