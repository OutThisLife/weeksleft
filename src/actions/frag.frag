precision mediump float;

#define PI 3.14159265359
#define TWO_PI 6.28318530718

uniform float uTime;
uniform float size;
uniform vec2 uResolution;

varying vec2 vUv;
varying vec4 vColor;

float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5543123);
}

float circle(vec2 position, float radius) { return length(position) - radius; }

float square(vec2 position, float s) {
  return max(abs(position.x), abs(position.y)) - s;
}

float heart(vec2 p, float s) {
  float k = dot(p, p) - s;
  return (pow(k, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}

void main() {
  vec2 st = 1. - gl_PointCoord.xy - .5;
  st *= uResolution.x / uResolution.y;

  vec3 col = vColor.xyz;

  float r = 0.1;

  col = mix(col, vec3(0.), step(0., heart(st, r * 2.)));
  col *= mix(col, vec3(1.), step(0., heart(st, 0.08)));

  col +=
      mix(vec3(0.3), vec3(0.), smoothstep(-0.25, 0.5, circle(st - 0.2, 0.01)));

  gl_FragColor = vec4(col, 1.);
}