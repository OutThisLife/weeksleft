#version 300 es

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform vec3 cameraPosition;

in vec3 uv;
in vec4 position;

out vec3 vUv;
out vec4 vWorld;
out vec4 vPos;
out vec3 cPos;

void main() {
  vUv = uv;
  vWorld = position;
  vPos = projectionMatrix * modelViewMatrix * position;
  cPos = cameraPosition;

  gl_Position = vWorld;
}
