#version 300 es

in vec2 uv;
in vec4 position;

out vec2 vUv;

void main() {
  vUv = uv;
  gl_Position = position;
}