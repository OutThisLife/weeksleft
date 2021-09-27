#version 330

precision mediump float;

float map(vec3 p);

// clang-format off
#pragma glslify: import './lib.glsl'
#pragma glslify: import './shapes.glsl'
// clang-format on

uniform float iTime;
uniform vec3 iResolution;
uniform vec2 iMouse;
uniform vec3 cameraPosition;

in vec3 vUv;
in vec3 vPos;
in vec3 vNormal;

out vec4 fragColor;

// ----------------------------------------------------------------------

const vec3 primary = vec3(1., 0.2, 0.4);
const int steps = 255;
const float minDepth = 0.01;
const float maxDepth = 100.;

float map(vec3 p) {
  float d = 1e10;
  float an = sin(iTime);

  {
    vec3 q = p;
    float dt = sdSphere(q, 1.);

    d = min(d, dt);
  }

  return d;
}

vec3 calcNormal(vec3 p) {
  return normalize(vec3(
      map(vec3(p.x + 0.0001, p.y, p.z)) - map(vec3(p.x - 0.0001, p.y, p.z)),
      map(vec3(p.x, p.y + 0.0001, p.z)) - map(vec3(p.x, p.y - 0.0001, p.z)),
      map(vec3(p.x, p.y, p.z + 0.0001)) - map(vec3(p.x, p.y, p.z - 0.0001))));
}

vec3 material(vec3 p, vec3 ro, vec3 rd) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(rd.zxy);

  float dif = clamp(dot(nor, lig), 0., 1.);
  float amb = .5 + .5 * nor.y;

  return primary * amb + primary * dif;
}

vec4 render(vec3 ro, vec3 rd) {
  vec4 col = vec4(0.);

  float depth = 0.;
  for (int i = 0; i < steps && depth <= maxDepth; ++i) {
    vec3 p = ro + rd * depth;
    float dist = map(p);

    if (dist <= minDepth) {
      col = vec4(material(p, ro, rd), 1.);
    }

    depth += dist;
  }

  return col;
}

void main() {
  vec2 st = sqFrame(iResolution.xy);

  vec3 ro = normalize(vNormal);
  vec3 rd = normalize(vPos);

  fragColor = render(ro, rd);
}