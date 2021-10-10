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

  vec4 ndc = vec4(vUv.xy, 1., 1.);
  vec4 clip = projectionMatrix * modelViewMatrix * position;
  vec4 world =
      normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * ndc);

  ro = cameraPosition / iResolution.z;
  rd = world.xyz / iResolution.z;
  vPos = ndc / atan(world.w, vPos.w * 4.);

  gl_Position = position;
}
