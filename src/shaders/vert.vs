#version 330

uniform mat4 modelMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 viewMatrix;
uniform mat4 normalMatrix;
uniform vec3 cameraPosition;
uniform mat4 projectionMatrix;

in vec3 position;
in vec3 normal;
in vec3 uv;

out vec3 vUv;
out vec4 vPos;
out mat4 vModelViewMatrix;
out mat4 vProjectionMatrix;

void main() {
  vUv = uv;
  vPos = vec4(position, 1.);
  vModelViewMatrix = modelViewMatrix;
  vProjectionMatrix = projectionMatrix;

  gl_Position = vPos;
}
