#version 300 es

precision highp float;

in vec3 vNormal;
in vec3 vNormel;
out vec4 fragColor;

// ----------------------------------------------------------------------

void main() {
  vec3 col = vec3(0., 8, 1.);
  float d = pow(1. - dot(vNormal, vNormel), 8.);

  fragColor = vec4(clamp(col, 0., 1.), smoothstep(0., 4., d));
}
