#version 300 es

uniform mat3 normalMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;
uniform vec3 cameraPosition;

in vec3 normal;
in vec3 uv;
in vec4 position;

out vec3 vNormal;
out vec3 vNormel;
out vec3 vUv;

void main() {
  vUv = uv;
  vNormal = normalize(normalMatrix * normal);
  vNormel = normalize(normalMatrix * cameraPosition);

  gl_Position = projectionMatrix * modelViewMatrix * position;
}
