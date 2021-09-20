precision mediump float;

varying vec2 vUv;
uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;

// clang-format off
#pragma glslify: sqFrame = require('glsl-square-frame')
#pragma glslify: ease = require(glsl-easings/cubic-in-out)
// clang-format on

mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

void main() {
  vec2 st = sqFrame(iResolution);
  float t = ease(abs(sin(iTime)));

  vec2 mt = iMouse.xy;
  float m = length(mt - st);

  vec3 col = vec3(0.);
  vec3 c1 = vec3(1., .2, .4);
  vec3 c2 = vec3(.1);

  float c =
      length(vec2(st.x, -0.1 - st.y * 1.2 + abs(st.x) * (1. - abs(st.x))) *
             5.) -
      0.1;

  vec2 uv = vec2(0., c);
  uv.y -= (iTime * 1.);

  vec2 pos = uv - floor(uv);
  vec2 rot = pos * rotate(m);
  float len = length(pos);

  col = mix(c1, c2, step(c / 3., len)) + c1;
  col /= atan(dot(st, st), pow(st.y, 2.));
  col = mix(col, c1, fract(0.2 * len));
  col += (c2 / c1) * (0.8 * (0.33 * distance(st.y, pow(rot.y, 2.))));

  gl_FragColor = vec4(col, 1.);
}