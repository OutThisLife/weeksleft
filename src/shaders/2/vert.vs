#version 300 es
precision highp float;

// ---------------------------------------------------

#define saturate(a) clamp(a, 0., 1.)

// ---------------------------------------------------

uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 modelViewMatrix;
uniform mat3 normalMatrix;
uniform vec3 cameraPosition;
uniform vec4 iResolution;

in vec3 normal;
in vec2 uv;
in vec4 position;

out vec2 vUv;
out vec3 vNormal;
out vec4 vPos;
out vec3 vUvRes;
out vec3 vResolution;

out vec3 vReflect;
out vec3 vRefract[3];
out float vReflectionFactor;

void main() {
  float r = .5;
  vec3 rd = normalize((modelMatrix * position).xyz - cameraPosition);

  vUv = uv;
  vPos = position;
  vNormal = normalMatrix * normal;
  vUvRes = vec3(normalize(iResolution.xy), iResolution.z);
  vResolution = iResolution.xyz;

  vReflect = reflect(rd, vNormal);
  vRefract[0] = refract(rd, vNormal, r);
  vRefract[1] = refract(rd, vNormal, r * 0.99);
  vRefract[2] = refract(rd, vNormal, r * 0.98);
  vReflectionFactor = saturate(.1 + 2. * pow(1. + dot(rd, vNormal), 4.));

  gl_Position = projectionMatrix * modelViewMatrix * position;
}