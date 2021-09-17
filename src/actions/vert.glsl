precision mediump float;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform float size;
attribute vec3 position;
attribute vec2 uv;
attribute vec4 color;

varying vec4 vColor;
varying vec2 vUv;
varying vec2 vPos;

void main() {
  vec4 worldPosition = modelMatrix * vec4(position, 1.0);

  gl_PointSize = size;
  gl_Position = projectionMatrix * viewMatrix * worldPosition;

  vUv = uv;
  vPos = gl_Position.xy;
  vColor = color;
}