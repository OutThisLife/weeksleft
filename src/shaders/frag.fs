#version 300 es
precision mediump float;

// clang-format off
#pragma glslify: import('./lib.glsl')
#pragma glslify: import('./shapes.glsl')
#pragma glslify: sqFrame = require('glsl-square-frame')
// clang-format on

uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iCamera;
uniform vec2 iMouse;

in vec3 pos;
out vec4 fragColor;

const int steps = 255;
const float minDepth = 0.01;
const float maxDepth = 100.;

// ----------------------------------------------------------------------

float map(vec3 p) {
  float d = 1e10;
  float an = sin(iTime);

  {
    vec3 q = p - vec3(1.2, -2., -1.3);

    float d1 = sdSphere(q - vec3(0., .5 + .3 * an, 0.), .55);
    float d2 = sdCube(q, vec3(0.6, 0.2, 0.6), 0.2);

    float dt = opSmoothUnion(d1, d2, .25);

    d = min(d, dt);
  }

  {
    vec3 q = p - vec3(-1.2, -2., -1.3);

    float d1 = sdSphere(q - vec3(0., .5, .2), .3 + .3 * an) - 0.12;
    float d2 = sdCube(q, vec3(0.6, .2, .2), .4);

    float dt = opSmoothSubtraction(d1, d2, .25);

    d = min(d, dt);
  }

  return d;
}

vec3 material(vec3 p, vec3 ro, vec3 rd) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(1.0, 0.8, -0.));

  float dif = clamp(dot(nor, lig), 0., 1.);
  float amb = .5 + 1. * nor.y;
  float sha = calcSoftshadow(pos, lig, 0.01, 1., 32.0);

  vec3 prim = #f36;

  return prim * amb + prim * dif * sha;
}

vec3 render(vec3 ro, vec3 rd) {
  mat4 view = viewMatrix(ro, vec3(0.), vec3(0., 1., 0.));
  vec3 st = (view * vec4(rd, 0.)).xyz;

  vec3 col = vec3(0.);

  float depth = 0.;
  for (int i = 0; i < steps; ++i) {
    if (depth > maxDepth) {
      break;
    }

    vec3 p = ro + depth * st;
    float dist = map(p);

    if (dist < minDepth) {
      col = sqrt(material(p, ro, rd));
    }

    depth += dist;
  }

  return col;
}

vec3 rayDirection(float fov, vec2 p) {
  vec2 xy = p;
  float z = tan(radians(fov));
  return normalize(vec3(xy, -z));
}

void main() {
  vec2 st = sqFrame(iResolution);

  vec3 ro = vec3(0., 5., 5.0);
  vec3 rd = rayDirection(75., st);

  vec3 col = render(ro, rd);
  col = pow(col, vec3(.7));

  fragColor = vec4(col, 1.);
}