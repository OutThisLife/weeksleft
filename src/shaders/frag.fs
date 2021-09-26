#version 330

precision mediump float;

float map(vec3 p);

uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
uniform vec3 iCamera;
uniform vec3 cameraPosition;

in vec3 vUv;
in vec3 vViewPos;

out vec4 fragColor;

// ----------------------------------------------------------------------

const vec3 primary = vec3(1., 0.2, 0.4);

// ----------------------------------------------------------------------

/**
 * SDFs
 */
vec3 opRep(vec3 p, float s) { return mod(p + s * 0.5, s) - s * 0.5; }

float opUnion(float d1, float d2) { return min(d1, d2); }

float opSubtraction(float d1, float d2) { return max(-d1, d2); }

float opIntersection(float d1, float d2) { return max(d1, d2); }

float opSmoothUnion(float d1, float d2, float k) {
  float h = clamp(.5 + .5 * (d2 - d1) / k, 0., 1.);
  return mix(d2, d1, h) - k * h * (1. - h);
}

float opSmoothSubtraction(float d1, float d2, float k) {
  float h = clamp(.5 - .5 * (d2 + d1) / k, 0., 1.);
  return mix(d2, -d1, h) + k * h * (1. - h);
}

float opSmoothIntersection(float d1, float d2, float k) {
  float h = clamp(.5 - .5 * (d2 - d1) / k, 0., 1.);
  return mix(d2, d1, h) + k * h * (1. - h);
}

// The primitives

float sdHeart(vec2 p, float s) {
  return length(pow(dot(p, p) - s, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdCube(vec3 p, vec3 b, float r) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.)) - r;
}

/**
 * Noise functions
 */
float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(in vec2 st) {
  vec2 i = floor(st);
  vec2 f = fract(st);

  float a = random(i);
  float b = random(i + vec2(1., 0.));
  float c = random(i + vec2(0., 1.));
  float d = random(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float fbm(in vec2 st, int o) {
  float value = 0.;
  float amplitude = .5;
  float frequency = 0.;

  for (int i = 0; i < o; i++) {
    value += amplitude * noise(st);
    st *= 2.;
    amplitude *= .5;
  }

  return value;
}

/**
 * Vanity
 */
vec3 blendOverlay(vec3 base, vec3 blend) {
  return mix(1.0 - 2.0 * (1.0 - base) * (1.0 - blend), 2.0 * base * blend,
             step(base, vec3(0.5)));
}

vec3 vignette(vec2 p, float radius) {
  p /= radius;
  p -= vec2(-0.1, -0.1);

  float dist = length(p);
  dist = smoothstep(-.33, .99, 1. - dist);

  vec3 col = mix(vec3(0.33), vec3(1.), dist);
  vec3 noise = vec3(random(p * 1.5), random(p * 2.5), random(p));

  return mix(col, blendOverlay(col, noise), 0.025);
}

/**
 * Vector transformations
 */
mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

mat2 scale(vec2 scale) { return mat2(scale.x, 0., 0., scale.y); }

/**
 * Materials, lighting, et al
 */
vec3 calcNormal(vec3 p) {
  const vec3 st = vec3(.01, 0., 0.);

  float x = map(p + st.xyy) - map(p - st.xyy);
  float y = map(p + st.yxy) - map(p - st.yxy);
  float z = map(p + st.yyx) - map(p - st.yyx);

  return normalize(vec3(x, y, z));
}

float calcSoftshadow(vec3 ro, vec3 rd, float tmin, float tmax, const float k) {
  float res = 1.;
  float t = tmin;

  for (int i = 0; i < 50; i++) {
    float h = map(ro + rd * t);
    res = min(res, k * h / t);
    t += clamp(h, 0.2, 0.20);

    if (res < 0.5 || t > tmax) {
      break;
    }
  }

  return clamp(res, 0., 1.);
}

vec3 faceNormals(vec3 pos) {
  vec3 fdx = dFdx(pos);
  vec3 fdy = dFdy(pos);
  return normalize(cross(fdx, fdy));
}

// ----------------------------------------------------------------------
// ----------------------------------------------------------------------
// ----------------------------------------------------------------------

float map(vec3 p) {
  float d = 1e10;
  float an = sin(iTime);

  {
    float dt = sdSphere(p, 0.1);

    d = min(d, dt);
  }

  return d;
}

vec3 material(vec3 p, vec3 ro, vec3 rd) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vUv);

  float dif = clamp(dot(nor, lig), 0., 1.);
  float amb = .5 + .5 * nor.y;
  float sha = calcSoftshadow(vViewPos, lig, 0.01, 1., 32.0);

  return primary * amb + primary * dif + primary * sha;
}

vec4 render(vec3 ro, vec3 rd) {
  const int steps = 255;
  const float minDepth = 0.001;
  const float maxDepth = 100.;

  vec4 col = vec4(vignette(rd.xy, 1.), 4.);

  float depth = 0.;
  for (int i = 0; i < steps && depth < maxDepth; ++i) {
    vec3 p = ro * depth + rd;
    float dist = map(p);

    if (dist < minDepth) {
      col = vec4(material(p, ro, rd), 1.);
    }

    depth += dist;
  }

  return pow(col, vec4(.7));
}

vec3 rayDirection(float fov, vec2 p) {
  vec2 xy = p;
  float z = tan(radians(fov));
  return normalize(vec3(xy, -z));
}

vec2 sqFrame(vec2 st) {
  vec2 p = 2. * (gl_FragCoord.xy / st.xy) - 1.;

  p.x *= st.x / st.y;

  return p;
}

void main() {
  vec2 st = sqFrame(iResolution);

  vec3 ro = normalize(vec3(0., 0., 1.));
  vec3 rd = rayDirection(75., st);

  fragColor = render(ro, rd);
}