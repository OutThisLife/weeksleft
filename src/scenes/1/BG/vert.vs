#version 300 es

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform mat3 normalMatrix;
uniform vec4 iResolution;

in vec3 normal;
in vec2 uv;
in vec4 position;

out vec2 vUv;
out vec3 vNormal;
out vec4 vPos;
out vec3 vUvRes;
out vec3 vResolution;

void main() {
  vUv = uv;
  vPos = position;
  vNormal = normalMatrix * normal;
  vUvRes = vec3(normalize(iResolution.xy), iResolution.z);
  vResolution = iResolution.xyz;

  gl_Position = projectionMatrix * modelViewMatrix * position;
}