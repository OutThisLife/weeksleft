// https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

vec4 opU(vec4 d1, vec4 d2) { return (d1.x < d2.x) ? d1 : d2; }
vec2 opU(vec2 d1, vec2 d2) { return (d1.x < d2.x) ? d1 : d2; }

vec3 opRep(vec3 p, float s) { return mod(p + s * .5, s) - s * .5; }

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

float smin(float a, float b, float k) {
  float res = exp(-k * a) + exp(-k * b);
  return -log(res) / k;
}

float sdHeart(vec3 p, float s) {
  mat3 m_z = mat3(cos(3.14), -sin(3.14), 0, sin(3.14), cos(3.14), 0, 0, 0, 1);

  p = m_z * p;

  return sqrt(length(p) * length(p) +
              pow(p.x * p.x + 0.1125 * p.z * p.z, .33) * p.y) -
         s;
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdBox(vec3 p, vec3 b, float r) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.)) - r;
}

float sdTorus(vec3 p, vec2 t) {
  vec2 q = vec2(length(p.xz) - t.x, p.y);
  return length(q) - t.y;
}

float sdBoundingBox(vec3 p, vec3 b, float e) {
  p = abs(p) - b;
  vec3 q = abs(p + e) - e;

  return min(
      min(length(max(vec3(p.x, q.y, q.z), 0.)) +
              min(max(p.x, max(q.y, q.z)), 0.),
          length(max(vec3(q.x, p.y, q.z), 0.)) +
              min(max(q.x, max(p.y, q.z)), 0.)),
      length(max(vec3(q.x, q.y, p.z), 0.)) + min(max(q.x, max(q.y, p.z)), 0.));
}

float sdPlane(vec3 p) { return p.y; }

float sdEllipsoid(in vec3 p, in vec3 r) {
  return (length(p / r) - 1.) * min(min(r.x, r.y), r.z);
}

float sdTriPrism(vec3 p, vec2 h, float r) {
  vec3 q = abs(p);
  return max(q.z - h.y, max(q.x * .866025 + p.y * .5, -p.y) - h.x * r);
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
  vec3 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
  return length(pa - ba * h) - r;
}

float sdCylinder(vec3 p, vec2 h) {
  vec2 d = abs(vec2(length(p.xz), p.y)) - h;
  return min(max(d.x, d.y), 0.) + length(max(d, 0.));
}

float sdCone(vec3 p, vec2 c) {
  float q = length(p.xy);
  return dot(c, vec2(q, p.z));
}

vec2 iBox(in vec3 ro, in vec3 rd, in vec3 rad) {
  vec3 m = 1. / rd;
  vec3 n = m * ro;
  vec3 k = abs(m) * rad;
  vec3 t1 = -n - k;
  vec3 t2 = -n + k;

  return vec2(max(max(t1.x, t1.y), t1.z), min(min(t2.x, t2.y), t2.z));
}

float sdOctahedron(vec3 p, float s) {
  p = abs(p);
  float m = p.x + p.y + p.z - s;
  vec3 q;
  if (3. * p.x < m) {
    q = p.xyz;
  } else if (3. * p.y < m) {
    q = p.yzx;
  } else if (3. * p.z < m) {
    q = p.zxy;
  } else {
    return m * 0.57735027;
  }

  float k = clamp(.5 * (q.z - q.y + s), 0., s);
  return length(vec3(q.x, q.y - s + k, q.z - k));
}

float lineSegment(in vec2 p, vec2 a, vec2 b) {
  vec2 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
  return length(pa - ba * h);
}

/**
 * Materials, lighting, et al
 */

vec4 castRay(vec3 ro, vec3 rd) {
  vec4 res = vec4(-1.);

  float tmin = 0.;
  float tmax = 20.;
  float t = tmin;
  vec3 m = vec3(-1.);

  float tp1 = (0. - ro.y) / rd.y;

  if (tp1 > 0.) {
    tmax = min(tmax, tp1);
  }

  if (t > -.5) {
    for (int i = 0; i < 255; i++) {
      vec4 h = map(ro + rd * t, 1.);

      if (abs(h).x < (0.0005 * t) || t >= tmax) {
        break;
      }

      t += h.x;
      m = h.yzw;
    }
  }

  if (t > tmax) {
    m = vec3(-1.);
  }

  return vec4(t, m);
}

float calcAO(in vec3 pos, in vec3 nor) {
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

    if (res <= 0.001 || t >= tmax) {
      break;
    }
  }

  return clamp(res, 0., 1.);
}

vec3 calcNormal(vec3 p) {
  vec2 e = vec2(1.0, -1.0) * 0.5773 * 0.0005;

  return normalize(e.xyy * map(p + e.xyy, 1.).x + e.yyx * map(p + e.yyx, 1.).x +
                   e.yxy * map(p + e.yxy, 1.).x + e.xxx * map(p + e.xxx, 1.).x);
}

vec3 rayPlaneIntersection(vec3 ro, vec3 rd, vec4 plane) {
  float t = -(dot(ro, plane.xyz) + plane.w) / dot(rd, plane.xyz);
  return ro + t * rd;
}