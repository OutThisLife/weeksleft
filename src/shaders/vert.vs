#version 300 es

uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat4 normalMatrix;
uniform vec3 cameraPosition;
uniform mat4 cameraWorldMatrix;
uniform mat4 cameraProjectionMatrix;
uniform mat4 cameraProjectionMatrixInverse;

in vec3 uv;
in vec4 position;
uniform vec3 iResolution;

out vec3 vUv;
out vec4 vPos;
out vec3 ro;
out vec3 rd;

void main() {
  vUv = (uv * 2. - 1.) / iResolution.z;
  vPos = projectionMatrix * modelViewMatrix * position;

  vec4 ndc = vec4(vUv.xy, 1., 1.);

  ro = cameraPosition / iResolution.z;
  rd = normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc).xyz /
       iResolution.z;

  gl_Position = position;
}
