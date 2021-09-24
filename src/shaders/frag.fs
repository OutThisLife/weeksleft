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

vec3 opRep(vec3 p, float s) { return mod(p + s * 0.5, s) - s * 0.5; }

float opUnion(float d1, float d2) { return min(d1, d2); }

float opSubtraction(float d1, float d2) { return max(-d1, d2); }

float opIntersection(float d1, float d2) { return max(d1, d2); }

float opSmoothUnion(float d1, float d2, float k) {
  float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
  return mix(d2, d1, h) - k * h * (1.0 - h);
}

float opSmoothSubtraction(float d1, float d2, float k) {
  float h = clamp(0.5 - 0.5 * (d2 + d1) / k, 0.0, 1.0);
  return mix(d2, -d1, h) + k * h * (1.0 - h);
}

float opSmoothIntersection(float d1, float d2, float k) {
  float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
  return mix(d2, d1, h) + k * h * (1.0 - h);
}

float onion(float d, float r) { return abs(d) - r; }

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdCube(vec3 p, vec3 b, float r) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0)) - r;
}

float sdOctahedron(vec3 p, float s) {
  p = abs(p);
  float m = p.x + p.y + p.z - s;
  vec3 q;
  if (3.0 * p.x < m)
    q = p.xyz;
  else if (3.0 * p.y < m)
    q = p.yzx;
  else if (3.0 * p.z < m)
    q = p.zxy;
  else
    return m * 0.57735027;

  float k = clamp(0.5 * (q.z - q.y + s), 0.0, s);
  return length(vec3(q.x, q.y - s + k, q.z - k));
}

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

vec3 calcNormal(vec3 p) {
  const vec3 st = vec3(.001, 0., 0.);

  float x = map(p + st.xyy) - map(p - st.xyy);
  float y = map(p + st.yxy) - map(p - st.yxy);
  float z = map(p + st.yyx) - map(p - st.yyx);

  return normalize(vec3(x, y, z));
}

float calcSoftshadow(vec3 ro, vec3 rd, float tmin, float tmax, const float k) {
  float res = 1.0;
  float t = tmin;

  for (int i = 0; i < 50; i++) {
    float h = map(ro + rd * t);
    res = min(res, k * h / t);
    t += clamp(h, 0.02, 0.20);

    if (res < 0.005 || t > tmax) {
      break;
    }
  }

  return clamp(res, 0.0, 1.0);
}

vec3 material(vec3 p, vec3 ro, vec3 rd) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(1.0, 0.8, -0.));

  float dif = clamp(dot(nor, lig), 0., 1.);
  float amb = .5 + 1. * nor.y;
  float sha = calcSoftshadow(pos, lig, 0.001, 1.0, 32.0);

  vec3 prim = #f36;

  return prim * amb + prim * dif * sha;
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