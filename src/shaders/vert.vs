#version 300 es

precision mediump float;

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform mat3 normalMatrix;

in vec3 uv;
in vec3 normal;
in vec4 position;

out vec3 vUv;
out vec4 vPos;

void main() {
  vUv = uv;
  vPos = projectionMatrix * modelViewMatrix * position;

  gl_Position = position;
}
