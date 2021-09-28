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
#define AA 2

const vec3 primary = vec3(1., .2, .4);
vec2 map(vec3 p);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

vec2 map(vec3 p) {
  vec2 res = vec2(sdPlane(p), 1.);

  {
    float d = sdSphere(p - vec3(-.4, .5, 0.), .25);
    res = opU(res, vec2(d, 2.5));
  }

  {
    float d = sdHeart(p - vec3(.4, .5, 0.), .25);
    res = opU(res, vec2(d, 2.5));
  }

  return res;
}

vec4 render(vec3 ro, vec3 rd) {
  vec2 res = castRay(ro, rd);
  float t = res.x;
  float m = res.y;

  vec4 fog = vec4(.8, .4, .9, 1.);
  vec4 col = fog + rd.y;

  if (m >= -.5) {
    vec3 p = ro + t * rd;

    vec3 nor = calcNormal(p);
    vec3 lig = normalize(vec3(ro.xy, ro.z + 1.));
    vec3 ref = reflect(rd, nor);
    vec3 hal = normalize(lig - rd);

    col = vec4(primary, 1.) * (m - 1.);

    // Floor
    if (m < 1.5) {
      float f = mod(floor(3. * p.z) + floor(3. * p.x), 2.);

      col = .5 + .1 * f * vec4(1.);
    }

    // Lighting
    float occ = calcAO(p, nor);                        // ambient occlusion
    float amb = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));  // ambient
    float dif = clamp(dot(nor, lig), 0., 1.);          // diffuse

    float bac = clamp(dot(nor, hal), 0., 1.) * clamp(1. - p.y, 0., 1.);
    float dom = smoothstep(-.1, .1, ref.y);                 // dome
    float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 4.);  // fresnel
    float spe = pow(clamp(dot(ref, hal), 0., .94), 16.);    // specular

    vec3 lin = vec3(0.);
    dif *= calcSoftshadow(p, lig, .02, 2.5, 8.);
    dom *= calcSoftshadow(p, ref, .02, 2.5, 8.);

    lin += 1.3 * dif;
    lin += 2. * spe * fog.xyz * dif;

    lin += .4 * amb * occ;
    lin += .2 * dom * occ;
    lin += .2 * bac * occ;
    lin += .5 * fre * occ;

    if (m >= 1.5) {
      lin += fre * (nor / sin(fbm(p.xy, int(t)) + cross(hal, ref)));
    }

    col *= vec4(lin, 1.);

    // Fog
    col = mix(col, fog, 1. - exp(-0.0002 * pow(t, 3.)));
  }

  return vec4(clamp(col, 0., 1.));
}

void main() {
  vec4 col = vec4(0.);

  for (int m = 0; m < AA; m++)
    for (int n = 0; n < AA; n++) {
      vec2 o = vec2(float(m), float(n)) / float(AA);
      vec2 st = sqFrame(iResolution.xy + o);
      vec4 ndc = vec4(st.xy, 1., 1.);

      vec3 ro =
          vec3(cameraPosition.x, max(0.1, cameraPosition.y), cameraPosition.z);

      vec3 rd =
          normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc)
              .xyz;

      col += pow(render(ro, rd), vec4(vec3(.4545), 0.));
    }

  col /= float(AA * AA);

  fragColor = col;
}
