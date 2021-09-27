// https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm

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

float sdHeart(vec2 p, float s) {
  return length(pow(dot(p, p) - s, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdBox(vec3 p, vec3 b, float r) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.)) - r;
}

float sdCone(vec3 p, vec2 c) {
  float q = length(p.xy);
  return dot(c, vec2(q, p.z));
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