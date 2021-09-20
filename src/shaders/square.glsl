float square(vec2 p, float s) { return max(abs(p.x), abs(p.y)) - s; }

// clang-format off
#pragma glslify: export(square)