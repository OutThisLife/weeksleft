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

float circle(vec2 p, float radius) { return length(p) - radius; }

float square(vec2 p, vec2 offset, float s) {
  p.x += offset.x;
  p.y += offset.y;
  return max(abs(p.x), abs(p.y)) - s;
}

float heart(vec2 p, vec2 offset, float s) {
  p.x += offset.x;
  p.y += offset.y;

  float k = dot(p, p) - s;
  return (pow(k, 3.) - pow(p.x, 2.) * pow(p.y, 3.));
}

float light(vec2 p, vec2 offset, float s) {
  p.x += offset.x;
  p.y += offset.y;

  return smoothstep(-0.5, 0.5, length(p) - s);
}

mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

void main() {
  float aspect = uResolution.x / uResolution.y;
  vec2 st = vUv - 0.5;
  st.y += 0.1;

  vec4 col = vec4(0.);
  vec4 base = vec4(vec3(1., 0.2, 0.4), 1.);

  float r = 0.1;
  float a = step(0., heart(st, vec2(0.), r));
  float b = step(0., heart(st, vec2(0.), r + 0.005));
  float c = light(st, vec2(0.2, -0.3), .1);
  c *= light(st, vec2(-0.2, -0.1), .1);

  col = mix(base, vec4(1.) - b, a);
  col += mix(vec4(.5), vec4(0.), clamp(c, a, 1.));

  for (int i = 0; i < 5; i++) {
    float n = float(i + 1);
    float o = (1. + (n * 1.)) + (1. - cos(sin(uTime * 2. - n / 2.)));
    float h =
        step(0., heart(vec2(st.x / o * 6.5, st.y / o * 6.5), vec2(0.), r));

    col += mix(base, vec4(0.), h);
  }

  gl_FragColor = col;
}