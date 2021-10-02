// https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

vec2 opU(vec2 d1, vec2 d2) { return (d1.x < d2.x) ? d1 : d2; }
vec4 opU(vec4 d1, vec4 d2) { return (d1.x < d2.x) ? d1 : d2; }

vec3 opRep(vec3 p, float s) { return mod(p + s * .5, s) - s * .5; }

vec3 opRepLim(vec3 p, float s, vec3 l) {
  return p - s * clamp(floor(p / s) + .5, -l, l);
}

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

float opOnion(float d, float s) { return abs(d) - s; }

float opDisplace(float d1, float d2, float d) { return d1 + (d2 + d); }

float opExtrusion(vec3 p, float d, float h) {
  vec2 w = vec2(d, abs(p.z) - h);
  return min(max(w.x, w.y), 0.) + length(max(w, 0.));
}

vec2 opRevolution(vec3 p, float o) { return vec2(length(p.xz) - o, p.y); }

float smin(float a, float b, float k) {
  float res = exp(-k * a) + exp(-k * b);
  return -log(res) / k;
}

vec3 opCheapBend(vec3 p) {
  const float k = 10.0;
  float c = cos(k * p.x);
  float s = sin(k * p.x);
  mat2 m = mat2(c, -s, s, c);

  return vec3(m * p.xy, p.z);
}

vec3 opTwist(vec3 p, const float k) {
  float c = cos(k * p.y);
  float s = sin(k * p.y);
  mat2 m = mat2(c, -s, s, c);

  return vec3(m * p.xz, p.y);
}

vec3 opSymX(vec3 p) {
  p.x = abs(p.x);
  return p;
}

vec3 opSymXZ(vec3 p) {
  p.xz = abs(p.xz);
  return p;
}

float invert(float m) { return 1. / m; }

vec3 opTx(vec3 p, mat4 m) { return (vec4(p, 1.) * m).xyz; }

vec3 opScale(vec3 p, float s) { return (p / s) * s; }

// Shapes

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

float sdBox(vec3 p, vec3 b) {
  vec3 d = abs(p) - b;
  return length(max(d, 0.)) + min(max(d.x, max(d.y, d.z)), 0.);
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

float sdEllipsoid(vec3 p, vec3 r) {
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

float checkerboard(vec3 p) {
  return mod(floor(5. * p.z) + floor(5. * p.x), 2.);
}

vec2 iBox(vec3 ro, vec3 rd, vec3 rad) {
  vec3 m = 1. / rd;
  vec3 n = m * ro;
  vec3 k = abs(m) * rad;
  vec3 t1 = -n - k;
  vec3 t2 = -n + k;

  return vec2(max(max(t1.x, t1.y), t1.z), min(min(t2.x, t2.y), t2.z));
}

float sdOctahedron(vec3 p, float s) {
  p = abs(p);
  return (p.x + p.y + p.z - s) * .57735027;
}

float lineSegment(vec2 p, vec2 a, vec2 b) {
  vec2 pa = p - a, ba = b - a;
  float h = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
  return length(pa - ba * h);
}
