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

#define C(v, a, b) clamp(v, a, b)
#define saturate(a) C(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SMP(v, r) SM(3. / Rpx.y, 0., length(v) - r)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

float snoise(vec3 p, float res) {
  const vec3 s = vec3(1e0, 1e2, 1e3);

  p *= res;

  vec3 uv0 = floor(mod(p, res)) * s;
  vec3 uv1 = floor(mod(p + vec3(1), res)) * s;

  vec3 f = fract(p);
  f = f * f * (3. - 2. * f);

  vec4 v = vec4(uv0.x + uv0.y + uv0.z, uv1.x + uv0.y + uv0.z,
                uv0.x + uv1.y + uv0.z, uv1.x + uv1.y + uv0.z);

  vec4 r = fract(sin(v * 1e-1) * 1e3);
  float r0 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

  r = fract(sin((v + uv1.z - uv0.z) * 1e-1) * 1e3);
  float r1 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

  return mix(r0, r1, f.z) * 2. - 1.;
}

float noise(vec2 p, float res) {
  float n;

  for (int i = 0; i < 7; i++) {
    float fi = float(i), v = pow(2., fi);
    n += (1.5 / v) * snoise(vec3(p + 1. * (fi / 17.), 1), v * res);
  }

  return saturate((1. - n) * .5) * 2.;
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * (R.xy * 1.);
  vec2 mo = iMouse * (R.xy * 1.);
  vec3 col;

  float t = iTime;

  // Vars
  {

    vec2 p = st;
    vec2 dir = normalize(p) * -.4;

    float dc = 1. - (length(p) * 2.);
    float pdc = pow(dc, 1.5);

    float phase0 = fract(t * .3 + .5);
    float phase1 = fract(t * .3);
    float lerp = abs((.5 - phase0) / .5);

    vec2 p0 = p + phase0 * dir;
    vec2 p1 = p + phase1 * dir;

    {
      const float s = 7.;

      float d0 = max(noise(p0, s), noise(p0 * 1.2, s));
      float d1 = max(noise(p1, s), noise(p1 * 1.4, s));

      float d = mix(d0, d1, lerp);
      float dc = pow(dc, s);

      col += hsv(vec3(.6, .3, pdc)) * d * pdc;
      col = mix(col, hsv(vec3(phase0, .1, dc * 2.3)), dc);
    }
  }

  fragColor = vec4(pow(saturate(col), vec3(1. / 2.2)), 1);
  fragColor = vec4(col, 1);
}
