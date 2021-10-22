#version 300 es

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform mat3 normalMatrix;

in vec3 normal;
in vec2 uv;
in vec4 position;

out vec2 vUv;
out vec3 vNormal;
out vec4 vPos;

void main() {
  vUv = uv;
  vPos = projectionMatrix * modelViewMatrix * position;
  vNormal = normalMatrix * normal;

  gl_Position = position;
}