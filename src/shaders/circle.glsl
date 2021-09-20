float circle(vec2 p, float radius) { return length(p) - radius; }

// clang-format off
#pragma glslify: export(circle)