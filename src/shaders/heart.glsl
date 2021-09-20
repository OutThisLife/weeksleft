float heart(vec2 p, float s) {
  p *= .8;
  p.y = -0.1 - p.y * 1.2 + abs(p.x) * (1.0 - abs(p.x));

  return length(p) - s;
}

// clang-format off
#pragma glslify: export(heart)