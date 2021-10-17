#version 300 es

uniform mat3 normalMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;

in vec3 normal;
in vec3 uv;
in vec4 position;

out vec3 vUv;
out vec4 vPos;
out vec3 vNormal;

void main() {
  vUv = uv;
  vPos = modelMatrix * position;
  vNormal = normalMatrix * normal;

  gl_Position = projectionMatrix * modelViewMatrix * position;
}