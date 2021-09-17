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

void main() {

  gl_PointSize = size;
  gl_Position =
      projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0);

  vUv = uv;
  vColor = color;
}