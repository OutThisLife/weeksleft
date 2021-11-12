vec2 sceneSDF(vec3 p, float s);

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

float smin(float a, float b, float k) {
  float h = clamp(0.5 + 0.5 * (a - b) / k, 0.0, 1.0);
  return mix(a, b, h) - k * h * (1.0 - h);
}

/**
 * Setup utils
 */
vec2 sqFrame(vec2 st, float s) {
  vec2 p = 2. * (gl_FragCoord.xy / st.xy) - 1.;
  p *= (st.x / st.y) * s;

  return p;
}

vec2 scale(vec2 p, float s) { return (p * s) - (s / 2.); }

float aastep(float s, float d) {
#ifdef GL_OES_standard_derivatives
  float w = length(vec2(dFdx(d), dFdy(d))) * 0.70710678118654757;
  return smoothstep(s - w, s + w, d);
#else
  return step(s, d);
#endif
}

/**
 * Vector transformations
 */

mat2 scale(vec2 p) { return mat2(p.x, 0., 0., p.y); }

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

vec3 rot(vec3 v, vec3 u, float a) {
  float c = cos(a);
  float s = sin(a);
  return v * c + cross(u, v) * s + u * dot(u, v) * (1. - c);
}

/**
 * Materials, lighting, et al
 */
#ifdef MAX_STEPS
vec3 castRay(vec3 ro, vec3 rd) {
  float tmax = MAX_DIST;
  float t = EPSILON;
  float m = -1.;
  float s = 0.;

  for (int i = 0; i < MAX_STEPS; i++) {
    vec2 h = sceneSDF(ro + (t * rd), 1.);

    if (abs(h).x <= EPSILON || t >= tmax) {
      break;
    }

    t += max(EPSILON, h.x);
    m = h.y;
    s++;
  }

  t = min(tmax, t);
  return vec3(t, max(-1. * float(t >= tmax), m), s / float(MAX_STEPS));
}

float calcSoftshadow(vec3 ro, vec3 rd, float tmin, float tmax) {
  float res = 1.;
  float t = tmin;
  float ph = 1e10;

  for (int i = 0; i < 16; i++) {
    float h = sceneSDF(ro + (t * rd), 1.).x;

    if (abs(h) <= EPSILON || t >= tmax) {
      break;
    }

    float s = saturate(8. * h / t);

    res = min(res, s * s * (3. - 2. * s));
    t += clamp(h, .02, .10);
  }

  return saturate(res);
}

float calcAO(vec3 p, vec3 nor) {
  float occ = 0.;
  float sca = 1.;

  for (int i = 0; i < 5; i++) {
    float h = .001 + .15 * float(i) / 4.;
    float d = sceneSDF(p + h * nor, 1.).x;

    occ += (h - d) * sca;
    sca *= 0.95;
  }

  return saturate(1. - 1.5 * occ);
}

vec3 calcNormal(vec3 p, float e) {
  vec3 eps = vec3(e, 0., 0.);

  return normalize(
      vec3(sceneSDF(p + eps.xyy, 1.).x - sceneSDF(p - eps.xyy, 1.).x,
           sceneSDF(p + eps.yxy, 1.).x - sceneSDF(p - eps.yxy, 1.).x,
           sceneSDF(p + eps.yyx, 1.).x - sceneSDF(p - eps.yyx, 1.).x));
}

vec3 calcNormal(vec3 pos) { return calcNormal(pos, EPSILON); }
#endif