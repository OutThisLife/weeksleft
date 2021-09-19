precision mediump float;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform vec2 iResolution;

attribute vec3 position;
attribute vec2 uv;

varying vec2 vUv;
varying vec2 res;

void main() {
  vUv = uv;
  res = iResolution;

  gl_Position =
      (projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0));
}