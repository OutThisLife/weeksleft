float sdHeart(vec2 p, float s) {
  return length(pow(dot(p, p) - s, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}

float sdSphere(vec3 p, float r) { return length(p) - r; }

float sdCube(vec3 p, vec3 b, float r) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.) + length(max(d, 0.)) - r;
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