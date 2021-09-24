// clang-format off
#pragma glslify: random = require(glsl-random)
#pragma glslify: blend = require(glsl-blend-overlay)
// clang-format on

mat3 yuv2rgb =
    mat3(1.0, 0.0, 1.13983, 1.0, -0.39465, -0.58060, 1.0, 2.03211, 0.0);

mat3 rgb2yuv = mat3(0.2126, 0.7152, 0.0722, -0.09991, -0.33609, 0.43600, 0.615,
                    -0.5586, -0.05639);

mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

mat2 scale(vec2 scale) { return mat2(scale.x, 0.0, 0.0, scale.y); }

vec3 vignetteBackground(vec2 p, float radius) {
  p /= radius;
  p -= vec2(-0.1, -0.1);

  float dist = length(p);
  dist = smoothstep(-.33, .99, 1. - dist);

  vec3 col = mix(vec3(0.33), vec3(1.), dist);
  vec3 noise = vec3(random(p * 1.5), random(p * 2.5), random(p));

  return mix(col, blend(col, noise), 0.025);
}
