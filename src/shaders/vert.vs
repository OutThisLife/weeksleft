precision highp float;

attribute vec3 position;
attribute vec3 uv;

varying vec3 vUv;
varying vec4 vPos;

void main() {
  vUv = uv;
  vPos = vec4(position, 1.);

  gl_Position = vPos;
}
