#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;

out vec4 fragColor;

// ---------------------------------------------------

#define R vUvRes
#define Rpx vResolution
#define PI 3.1415926538
#define TWOPI 6.28318530718
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SMP(v, r) smoothstep(3. / Rpx.y, 0., length(v) - r)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

vec3 hsv(float h, float s, float v) { return hsv(vec3(h, s, v)); }

float hash(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float hash(float s) { return hash(vec2(s, dot(s, s))); }

float hash21(vec2 p) {
  p = fract(p * vec2(123.34, 456.21));
  p += dot(p, p + 45.32);
  return fract(p.x * p.y);
}

float circularIn(float t) { return 1. - sqrt(1. - pow(t, 2.)); }
float cubicInOut(float t) {
  return t < 0.5 ? 4.0 * t * t * t : 0.5 * pow(2.0 * t - 2.0, 3.0) + 1.0;
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 2.);
  vec2 mo = iMouse * (R.xy * 2.);
  vec3 col;

  float t = iTime;
  const int STEPS = 4;

  for (int i = 0; i < STEPS; i++) {
    float fi = float(i) / float(STEPS);
    float w = mix(2., .5, fi);
    float t = fract(w + t / 3.);
    float animFade = 1. - abs(2. * t - 1.);
    float animScale = (w / -2.) - w * t * .5;

    vec2 p = st * w;
    p *= rot(animScale);

    vec2 gv = fract(p) - .5;
    vec2 id = floor(p);

    float dist = distance(mo, p);
    float r = length(p) - .1;

    r = SM(0., 1., r);

    for (int x = -1; x <= 1; x++)
      for (int y = -1; y <= 1; y++) {
        vec2 o = vec2(x, y);

        float h = hash21(w + id + o);
        vec2 o2 = vec2(h, fract(h * 3.));

        vec2 p = p;
        p = rot(1. / animScale) * p + (mo * .1 * cubicInOut(h)) + gv - o - o2 +
            .5;

        float d = length(p) / animFade;
        d /= 1. * abs(cos(atan(p.y, p.x) * 4. - 12. + 12. * sqrt(dot(p, p))));
        d = .1 / d * SM(.15, 0., d);
        d = saturate(1. - max(r, 1. - d));

        col = mix(col, hsv(vec3(1, h, length(gv))), d);
      }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
}
