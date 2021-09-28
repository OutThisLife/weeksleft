#version 330

precision mediump float;

uniform float iTime;
uniform int iFrame;
uniform vec3 iResolution;

uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec3 vUv;

out vec4 fragColor;

// ----------------------------------------------------------------------

#define ZERO (min(iFrame, 0))

const vec3 primary = vec3(1., .2, .4);
vec4 map(vec3 p, float s);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

vec4 map(vec3 p, float s) {
  vec4 res = vec4(1.);

  {
    float d = sdSphere(p - vec3(-.4, .5, 0.), .25 * s);

    res = opU(res, vec4(d, vec3(0., 0., 1.)));
  }

  {
    float d = sdHeart(p - vec3(.4, .5, 0.), .25 * s);

    res = opU(res, vec4(d, vec3(0., 1., 0.)));
  }

  return res;
}

void inverseRender(vec3 ro, vec3 rd, inout vec4 col) {
  float t = 0.;
  float tmax = 20.;

  for (int i = 0; i < 64 && t <= tmax; i++) {
    vec3 p = (ro + t * rd);
    vec4 h = map(p, (0.3 + (.1 * sin(iTime)) + .2));

    if (abs(h).x < (0.0005 * t)) {
      vec3 nor = calcNormal(p);
      float ndotl = abs(dot(-rd, nor));
      float rim = pow(1. - ndotl, 3.);

      col = vec4(mix(refract(nor, rd, .85), vec3(1.), rim), 1.);

      break;
    }

    t += h.x;
  }
}

void render(vec3 ro, vec3 rd, inout vec4 col) {
  float t = 0.;
  float tmax = 20.;

  float tp1 = (0. - ro.y) / rd.y;

  if (tp1 > 0.) {
    tmax = min(tmax, tp1);
  }

  for (int i = 0; i < 255 && t <= tmax; i++) {
    vec3 p = ro + t * rd;
    vec4 h = map(p, 1.);

    if (abs(h).x < (0.0005 * t)) {
      vec3 nor = calcNormal(p);
      vec3 lig = normalize(vec3(.9, .4, -.4));
      vec3 ref = reflect(rd, nor);
      vec3 hal = normalize(lig - rd);

      float ndotl = abs(dot(-rd, nor));
      float rim = pow(1. - ndotl, 4.);

      float occ = calcAO(p, nor);                        // ambient occlusion
      float amb = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));  // ambient
      float dif = clamp(dot(nor, lig), 0., 1.);          // diffuse

      float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 4.);  // fresnel
      float spe = pow(clamp(dot(ref, hal), 0., .94), 16.);    // specular

      vec3 lin = vec3(.9);

      dif *= calcSoftshadow(p, lig, .02, 2.5, 8.);

      lin *= mix(lin, nor, rim) + rim;
      lin += 1.3 * dif;
      lin += 2. * spe * dif;
      lin += 2. * fre * dif;
      lin += .4 * amb * occ;

      col *= vec4(lin, 1.);

      inverseRender(p, refract(rd, nor, .9), col);
      break;
    }

    t += h.x;
  }

  if (t >= tmax) {
    vec3 p = vec3(0., 1., 0.);
    vec3 planePoint = rayPlaneIntersection(ro, rd, vec4(p, 0.));

    col.xyz *= mix(.9, .98, calcSoftshadow(planePoint, p, .02, 3., 7.));
  }
}

void main() {
  vec2 st = sqFrame(iResolution.xy);
  vec4 ndc = vec4(st.xy, 1., 1.);

  vec3 ro =
      vec3(cameraPosition.x, max(0.1, cameraPosition.y), cameraPosition.z);

  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz;

  vec4 col = vec4(1);
  render(ro, rd, col);

  // col = pow(col, vec4(vec3(.4545), 1.));

  fragColor = clamp(col, 0., 1.);
}
