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
#define rot2(a, b) mat2(cos(a), -sin(b), sin(b), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

float hash(float n) { return fract(sin(n) * 753.5453123); }

float snoise(vec3 uv, float res) {
  const vec3 s = vec3(1e0, 1e2, 1e3);

  uv *= res;

  vec3 uv0 = floor(mod(uv, res)) * s;
  vec3 uv1 = floor(mod(uv + vec3(1.), res)) * s;

  vec3 f = fract(uv);
  f = f * f * (3.0 - 2.0 * f);

  vec4 v = vec4(uv0.x + uv0.y + uv0.z, uv1.x + uv0.y + uv0.z,
                uv0.x + uv1.y + uv0.z, uv1.x + uv1.y + uv0.z);

  vec4 r = fract(sin(v * 1e-1) * 1e3);
  float r0 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

  r = fract(sin((v + uv1.z - uv0.z) * 1e-1) * 1e3);
  float r1 = mix(mix(r.x, r.y, f.x), mix(r.z, r.w, f.x), f.y);

  return mix(r0, r1, f.z) * 2. - 1.;
}

float noise(vec2 uv, float s) {
  float n;

  for (int i = 0; i < 7; i++) {
    float v = pow(2., float(i));

    n += (1.5 / v) * snoise(vec3(uv + vec2(1) * (float(i) / 17.), 1), v * s);
  }

  return saturate((1. - n) * .5) * 2.;
}

// ---------------------------------------------------

float ridge(vec2 p, float t) {
  vec2 q = p + t * -normalize(p);
  float a = atan(q.y, q.x), l = sqrt(length(q));

  return fract(3. * a + 5. - 5. * l) - .5;
}

float cloud(vec2 p, float t, vec2 rd) { return noise(p + t * rd, 7.); }

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;
  float t = iTime;
  const float ts = .3;

  float bt = S(st.y, -.6);
  float x = fract(st.x / 1.5) - .5;
  float y = x - (fract(mo.x / 1.5) - .5);
  float h = x - y;

  {
    vec2 p = st;
    vec3 c1 = hsv(vec3(h, -1, .5)), c2 = hsv(vec3(h, 1, 1));

    float dc = 1. - length(p) * 2.;
    float pdc = pow(dc, 3.5);
    float t1 = fract(t * ts + .5);
    float t2 = fract(t * ts);
    float lerp = abs((.5 - t1) / .5);

    vec2 rd = -normalize(p) * abs(ts);
    float d = mix(cloud(p, t1, rd), cloud(p, t2, rd), lerp) * pdc;

    col = saturate(mix(col, mix(c1, c2, pdc), d));
  }

  {
    // col *= hsv(vec3(x - y, 1, 1));
    col = mix(col, hsv(vec3(x, 1, 1)) - SMP(y, .001), bt);
  }

  fragColor = vec4(saturate(col), 1);
}
