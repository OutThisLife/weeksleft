#version 300 es

precision highp float;

uniform float iGlobalTime;
uniform sampler2D difTex;
uniform sampler2D speTex;
uniform sampler2D bumpTex;
uniform sampler2D cloudTex;

in vec3 vUv;
in vec3 vNormal;
out vec4 fragColor;

// ----------------------------------------------------------------------

void main() {
  vec4 col = vec4(1.);
  float d = clamp(
      (2. + .3 * abs(sin(iGlobalTime))) - 1. / dot(vNormal.xy, vNormal.xy), 0.,
      1.);

  col = texture(difTex, vUv.xy);
  col = mix(col, vec4(vec3(#30D5C8), 1.), d * d);

  fragColor = col;
}
