#version 330

uniform mat4 modelMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 viewMatrix;
uniform mat4 normalMatrix;
uniform vec3 cameraPosition;
uniform mat4 projectionMatrix;

in vec3 pos;
in vec3 normal;
in vec3 uv;

out vec3 vUv;
out vec3 vViewPos;

void main() {
  vec4 mpos = modelViewMatrix * vec4(pos, 1.);
  vec4 viewPos = projectionMatrix * mpos;

  vUv = uv;
  vViewPos = viewPos.xyz;

  gl_Position = viewPos;
}
