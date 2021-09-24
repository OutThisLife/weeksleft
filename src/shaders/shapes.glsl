float circle(vec2 p, float radius) { return length(p) - radius; }

float square(vec2 p, float s) { return max(abs(p.x), abs(p.y)) - s; }

float heart(vec2 p, float s) {
  return (pow(dot(p, p) - s, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}
