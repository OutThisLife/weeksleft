precision mediump float;

uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;

// clang-format off
#pragma glslify: sqFrame = require('glsl-square-frame')
// clang-format on

vec2 pattern(float a) {
  vec2 uv = vec2(0., a);
  uv.y = clamp(uv.y - fract(iTime), -15., 15.);

  vec2 pos = uv - floor(uv);
  return vec2(a, length(pos));
}

void main() {
  vec2 st = sqFrame(iResolution);
  float ms = iTime;
  float t = abs(sin(ms));

  vec2 mt = iMouse.xy;
  float m = length(mt - st);

  vec3 col = vec3(0.);
  vec3 c1 = vec3(1., .2, .4);
  vec3 c2 = vec3(.1);

  vec2 heart = pattern(
      length(vec2(st.x, -0.1 - st.y * 1.2 + abs(st.x) * (1. - abs(st.x))) *
             5.) -
      0.1);

  col = mix(c1, c2, step(heart.x / 3., heart.y)) + c1;
  col /= atan(dot(st, st), pow(st.y, 2.));
  col = mix(col, c1, fract(0.2 * heart.y));

  gl_FragColor = vec4(col, 1.);
}