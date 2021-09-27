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
out vec3 vPos;
out vec3 vNormal;

void main() {
  vec4 pos = vec4(position, 1.);
  vec4 cpt = projectionMatrix * modelViewMatrix * pos;

  vUv = uv;
  vPos = cpt.xyz;
  vNormal = (normalMatrix * vec4(normal, 1.)).xyz;

  gl_Position = pos;
}
