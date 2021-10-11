#version 300 es

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform mat3 normalMatrix;

in vec3 uv;
in vec4 position;
in vec3 normal;
uniform vec3 iResolution;

out vec3 vUv;
out vec3 vNormal;

void main() {
  vUv = uv;
  vNormal = normalize(normalMatrix * normal);

  gl_Position = projectionMatrix * modelViewMatrix * position;
}
