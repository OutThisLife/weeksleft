precision mediump float;

#define EPSILON 0.0005
#define MAX_STEPS 255
#define MIN_DIST 0.
#define MAX_DIST 60.
#define AA 1

uniform float iTime;
uniform int iFrame;
uniform vec3 iResolution;

uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrixInverse;

varying vec3 vUv;

// ----------------------------------------------------------------------

vec2 sceneSDF(vec3 p, float s);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

// ----------------------------------------------------------------------

vec2 sceneSDF(vec3 p, float s) {
  vec2 res = vec2(sdPlane(p), 1.);

  {
    vec3 q = p - vec3(-1., 1., 0.);

    float d1 = sdBoundingBox(q, vec3(.5) * s, .03);
    float d2 = sdSphere(q, .3 * s);

    res = opU(res, vec2(d1, 2.));
    res = opU(res, vec2(d2, 3.));
  }

  {
    vec3 q = p - vec3(0, 1., 0.);

    float d = sdOctahedron(q, .2 * s);

    // float disp = clamp((abs((opCheapBend((2. + 1. * sin(iTime * .2)) * q).x *
    //                          opCheapBend((2. + 1. * sin(iTime * .2)) *
    //                          q).x))),
    //                    0., 0.1);

    // d = sdOctahedron(q, .3 + disp);

    res = opU(res, vec2(d, 3.));
  }

  {
    vec3 q = p - vec3(1., 1., 0.);

    float d1 = sdBoundingBox(q, vec3(.5) * s, .03);
    float d2 = sdSphere(q, .3 * s);

    res = opU(res, vec2(d1, 4.));
    res = opU(res, vec2(d2, 3.));
  }

  return res;
}

void reflectionRay(vec3 ro, vec3 rd, inout vec3 col) {
  vec2 res = castSmallRay(ro, rd, 20.);
  float t = res.x;
  float m = res.y;

  if (m > 1.5 && t >= EPSILON) {
    vec3 p = ro + rd * t;

    vec3 nor = calcNormal(p);
    vec3 lig = normalize(vec3(-0.6, 0.7, -0.5));
    vec3 ref = reflect(rd, nor);
    vec3 hal = normalize(lig - rd);

    float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 4.);  // fresnel
    float spe = pow(clamp(dot(ref, hal), 0., .94), 16.);    // specular

    vec3 lin = clamp(.5 + .5 * nor.y, 0., 1.) * vec3(0.);

    lin += 2. * spe;
    lin += 2. * fre;

    col = mix(col, lin, .2);
  }
}

void render(vec3 ro, vec3 rd, inout vec3 col) {
  vec2 res = castRay(ro, rd);
  float t = res.x;
  float m = res.y;

  vec3 p = ro + rd * t;

  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(-.5, .4, -.6));
  vec3 ref = reflect(rd, nor);

  vec3 lin = vec3(0.);

  if (m > 1.5) {
    if (m == 3.) {
      lin += vec3(.9);
    } else if (m == 5.) {
      lin += vec3(.1);
    } else {
      lin += vec3(1., 0., 0.);
    }

    lin *= clamp(.5 + .5 * nor.y, 0., 1.);

    // reflectionRay(p + ro * EPSILON, ref, col);
  } else if (m == 1.) {
    lin += vec3(.9);
    lin += .8 * calcSoftshadow(p, lig, 0.02, 2.5, 1.);
    lin *= mix(vec3(.7, .8, .9), col, fbm(p.xz * 100.) - fbm(p.xz * 10.));
  }

  col = mix(col * lin, vec3(.7, .8, 1.), 1. - exp(-EPSILON * pow(t, 2.2)));
}

void main() {
  vec4 ndc = vec4(vUv.xy - vec2(.5, .33), 1., 1.);

  vec3 ro = cameraPosition;

  vec3 rd =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz;

  vec3 col = vec3(1.);
  render(ro, rd, col);

  // col = pow(col, vec3(1. / 2.2));
  col = clamp(col, 0., 1.);

  gl_FragColor = vec4(col, 1.);
}
