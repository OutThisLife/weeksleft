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

float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}
void main() {
  vec4 mpos = modelViewMatrix * vec4(pos, 1.);
  vec4 viewPos = projectionMatrix * mpos;

  vUv = uv;
  vViewPos = viewPos.xyz;

  gl_Position = modelMatrix * vec4(pos, 1.);
}
