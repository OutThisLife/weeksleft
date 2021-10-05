#version 300 es

precision mediump float;

in vec3 position;
in vec3 uv;

out vec3 vUv;
out vec4 vPos;

void main() {
  vUv = uv;
  vPos = vec4(position, 1.);

  gl_Position = vPos;
}
