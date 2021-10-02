
float dot2(vec2 v) { return dot(v, v); }
float dot2(vec3 v) { return dot(v, v); }
float ndot(vec2 a, vec2 b) { return a.x * b.x - a.y * b.y; }

float length2(vec3 p) {
  p = p * p;

  return sqrt(p.x + p.y + p.z);
}

float length6(vec3 p) {
  p = p * p * p;
  p = p * p;

  return pow(p.x + p.y + p.z, 1. / 6.);
}

float length8(vec3 p) {
  p = p * p;
  p = p * p;
  p = p * p;

  return pow(p.x + p.y + p.z, 1. / 8.);
}

/**
 * Setup utils
 */
vec2 sqFrame(vec2 st) {
  vec2 p = 2. * (gl_FragCoord.xy / st.xy) - 1.;

  p.x *= st.x / st.y;

  return p;
}

float aastep(float s, float d) {
#ifdef GL_OES_standard_derivatives
  float w = length(vec2(dFdx(d), dFdy(d))) * 0.70710678118654757;
  return smoothstep(s - w, s + w, d);
#else
  return step(s, d);
#endif
}

/**
 * Noise functions
 */

float hash(float n) { return fract(sin(n) * 753.5453123); }

float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(in vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);

  float a = random(i);
  float b = random(i + vec2(1., 0.));
  float c = random(i + vec2(0., 1.));
  float d = random(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float noise(in vec3 x) {
  vec3 p = floor(x);
  vec3 f = fract(x);
  f = f * f * (3.0 - 2.0 * f);

  float n = p.x + p.y * 157.0 + 113.0 * p.z;
  return mix(mix(mix(hash(n + 0.0), hash(n + 1.0), f.x),
                 mix(hash(n + 157.0), hash(n + 158.0), f.x), f.y),
             mix(mix(hash(n + 113.0), hash(n + 114.0), f.x),
                 mix(hash(n + 270.0), hash(n + 271.0), f.x), f.y),
             f.z);
}

float fbm(in vec2 st) {
  const int o = 1;
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
 * Vector transformations
 */

mat2 scale(vec2 scale) { return mat2(scale.x, 0., 0., scale.y); }

mat2 rotation2d(float angle) {
  float s = sin(angle);
  float c = cos(angle);
  return mat2(c, -s, s, c);
}

mat4 rotation3d(vec3 axis, float angle) {
  axis = normalize(axis);
  float s = sin(angle);
  float c = cos(angle);
  float oc = 1.0 - c;

  return mat4(
      oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s,
      oc * axis.z * axis.x + axis.y * s, 0.0, oc * axis.x * axis.y + axis.z * s,
      oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s, 0.0,
      oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s,
      oc * axis.z * axis.z + c, 0.0, 0.0, 0.0, 0.0, 1.0);
}

vec2 rotate(vec2 v, float angle) { return rotation2d(angle) * v; }

vec3 rotate(vec3 v, vec3 axis, float angle) {
  return (rotation3d(axis, angle) * vec4(v, 1.0)).xyz;
}

/**
 * Materials, lighting, et al
 */

float castRay(vec3 ro, vec3 rd) {
  float t = EPSILON;
  float tmax = MAX_DIST;

  for (int i = 0; i < MAX_STEPS; i++) {
    vec4 h = map(ro + rd * t, 1.);

    // if (abs(h).x < (0.0005 * t) || t >= tmax) {

    if (abs(h).x <= (EPSILON * t) || t >= tmax) {
      if (t >= tmax) {
        t = tmax;
      }

      break;
    }

    t += max(EPSILON, h.x);
  }

  return t;
}

float calcAO(vec3 pos, vec3 nor) {
  float occ = 0.;
  float sca = 1.;

  for (int i = 0; i < 5; i++) {
    float hr = .01 + .12 * float(i) / 4.;
    float dd = map(nor * hr + pos, 1.).x;

    occ += -(dd - hr) * sca;
    sca *= .95;
  }

  return clamp(1. - 3. * occ, 0., 1.);
}

float calcSoftshadow(vec3 ro, vec3 rd, float tmin, float tmax, const float k) {
  float tp = (k - ro.y) / rd.y;

  if (tp > .0) {
    tmax = min(tmax, tp);
  }

  float res = 1.;
  float t = tmin;

  for (int i = 0; i < 24; i++) {
    float h = map(ro + rd * t, 1.).x;
    float s = clamp(k * h / t, 0., 1.);

    res = min(res, s * s * (3. - 2. * s));
    t += clamp(h, .02, .2);

    if (res <= EPSILON || t >= tmax) {
      break;
    }
  }

  return clamp(res, 0., 1.);
}

vec3 calcNormal(vec3 p) {
  vec2 e = vec2(1.0, -1.0) * 0.5773 * EPSILON;

  return normalize(e.xyy * map(p + e.xyy, 1.).x + e.yyx * map(p + e.yyx, 1.).x +
                   e.yxy * map(p + e.yxy, 1.).x + e.xxx * map(p + e.xxx, 1.).x);
}

vec3 rayPlaneIntersection(vec3 ro, vec3 rd, vec4 plane) {
  float t = -(dot(ro, plane.xyz) + plane.w) / dot(rd, plane.xyz);
  return ro + t * rd;
}