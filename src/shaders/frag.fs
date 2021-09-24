#version 300 es
precision mediump float;

// clang-format off
#pragma glslify: import('./lib.glsl')
#pragma glslify: sqFrame = require('glsl-square-frame')
// clang-format on

uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iCamera;
uniform vec2 iMouse;

in vec3 pos;
out vec4 fragColor;

const int steps = 255;
const float minDepth = 0.001;
const float maxDepth = 100.;

// ----------------------------------------------------------------------

vec2 opU(vec2 d1, vec2 d2) { return (d1.x < d2.x) ? d1 : d2; }

float intersectSDF(float d1, float d2) { return max(d1, d2); }

float unionSDF(float d1, float d2) { return min(d1, d2); }

float differenceSDF(float d1, float d2) { return max(d1, -d2); }

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdCube(vec3 p, vec3 r) {
  vec3 d = abs(p) - r;
  return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.));
}

vec2 map(vec3 p) {
  vec2 res = vec2(1e10, 0.);

  float s = 0.923;
  float a = sdSphere(p / s, 1.3) * s;
  float b = sdCube(p / s, vec3(1.1)) * s;
  float c = sdSphere(p, 1.05);

  res = opU(res, vec2(unionSDF(intersectSDF(b, a), c), 1.));

  return res;
}

vec3 calcNormal(vec3 p) {
  const vec3 st = vec3(.001, 0., 0.);

  float x = map(p + st.xyy).x - map(p - st.xyy).x;
  float y = map(p + st.yxy).x - map(p - st.yxy).x;
  float z = map(p + st.yyx).x - map(p - st.yyx).x;

  return normalize(vec3(x, y, z));
}

vec3 material(vec3 p, vec3 ro, vec3 rd) {
  vec3 col;

  vec3 N = calcNormal(p);
  vec3 V = normalize(ro - p);

  vec3 light = normalize(vec3(-.5, .4, -.6));
  vec3 L = normalize(p);

  vec3 R = normalize(reflect(-L, N));

  float dotLN = max(0., dot(L, N));
  float dotRV = max(0., dot(R, V));

  col += (#f36 * dotLN) + (#f36 * pow(dotRV, 20.));

  return col;
}

mat4 viewMatrix(vec3 eye, vec3 center, vec3 up) {
  vec3 f = normalize(center - eye);
  vec3 s = normalize(cross(f, up));
  vec3 u = cross(s, f);

  return mat4(vec4(s, 0.), vec4(u, 0.), vec4(-f, 0.), vec4(0., 0., 0., 1.));
}

vec3 render(vec3 ro, vec3 rd) {
  mat4 view = viewMatrix(ro, vec3(0.), vec3(0., 1., 0.));
  vec3 st = (view * vec4(rd, 0.)).xyz;

  vec3 col = vignetteBackground(sqFrame(iResolution), 1.);

  float depth = 0.;
  for (int i = 0; i < steps; ++i) {
    if (depth > maxDepth) {
      break;
    }

    vec3 p = ro + depth * st;

    float dist = map(p).x;

    if (dist < minDepth) {
      col = material(p, ro, rd);
    }

    depth += dist;
  }

  return vec3(clamp(col, 0., 1.));
}

vec3 rayDirection(float fov, vec2 p) {
  vec2 xy = p;
  float z = tan(radians(fov));
  return normalize(vec3(xy, -z));
}

void main() {
  vec2 st = sqFrame(iResolution);

  vec3 ro = vec3(8., 5., 7.0);
  vec3 rd = rayDirection(75., st);

  vec3 col;

  col = render(ro, rd);
  col = pow(col, vec3(0.4545));

  fragColor = vec4(col, 1.);
}