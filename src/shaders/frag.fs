#version 300 es

precision highp float;

uniform float iGlobalTime;
uniform vec3 iResolution;

in vec3 vUv;
in vec3 vNormal;
in vec3 vNormel;
out vec4 fragColor;

// ----------------------------------------------------------------------

void main() {
  vec3 col;
  vec3 uv = vUv * 2. - 1. / iResolution.z;

  float d = pow(1. - dot(vNormal, vNormel), 8.);

  col += vec3(0., 8, 1.);

  fragColor = vec4(clamp(col, 0., 1.), smoothstep(0., 4., d));
}
