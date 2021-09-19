precision mediump float;

varying vec2 vUv;

uniform float iTime;
uniform vec2 iResolution;
uniform vec2 iMouse;
varying vec2 res;

#define PI 3.14159265359
#define TWO_PI 6.28318530718

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

void main() {
  float aspectRatio = res.x / res.y;

  vec2 st = vUv - 0.5;
  st.y += 0.13;
  st *= aspectRatio;

  vec2 m = iMouse.xy / aspectRatio;
  float mpos = length(m - st);

  float t = (PI * (iTime - .75) / 2.) / 1.4;

  vec4 col = vec4(0., 0., 0., 1.);
  vec4 c1 = vec4(vec3(1., 0.2, 0.4), 1.);
  vec4 c2 = c1 * 1.;
  vec4 c3 = c1 * 0.5;

  for (int i = 13; i > 0; --i) {
    float n = float(i + 1);
    float o = (n + (1. - acos(sin(iTime * 2. - n / 2.)))) / (n * 0.6);
    float h = step(0., heart(st / o, vec2(0.), n * 0.01));

    if (mod(n - 1., 4.) == 0.0) {
      col += mix(c3, vec4(0), h);
    } else {
      col += mix(c3 * light(st, vec2(0.), n * 0.05), vec4(0.), h);
    }
  }

  // col *= mix(vec4(0.1, 0.1, 0.1, 1.), col, col);

  gl_FragColor = col;
}