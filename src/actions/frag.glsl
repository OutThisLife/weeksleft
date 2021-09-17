precision mediump float;

#define PI 3.14159265359
#define TWO_PI 6.28318530718

uniform float uTime;
uniform float size;
uniform vec2 uResolution;

varying vec2 vUv;
varying vec2 vPos;
varying vec4 vColor;
const float SQRT_2 = 1.4142135623730951;
float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5543123);
}

float circle(vec2 uv) {
  return smoothstep(0.1, 0.09, length(uv)) - smoothstep(0.09, 0.08, length(uv));
}

float dot2(in vec2 v) { return dot(v, v); }
float dot2(in vec3 v) { return dot(v, v); }
float ndot(in vec2 a, in vec2 b) { return a.x * b.x - a.y * b.y; }

void main() {
  vec2 st = gl_PointCoord.xy;

  vec3 color = vColor.xyz;

  float a = atan(st.x, st.y) / PI;
  float r = length(st);
  float h = abs(a);
  float d = ((13.0 * h - 22.0 * h * h + 10.0 * h * h * h) / (6.0 - 5.0 * h)) /
            distance(st, vec2(0.25));

  float f = step(r, d) * pow(1. - r / d, 0.25);

  color *= f;

  gl_FragColor = vec4(color, 1.);
}