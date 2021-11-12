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

float snoise(vec3 p, float res) {
  const vec3 s = vec3(1e0, 1e2, 1e3);

  p *= res;

  vec3 uv0 = floor(mod(p, res)) * s;
  vec3 uv1 = floor(mod(p + vec3(1.), res)) * s;

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

float noise(vec2 uv, float s) {
  float d;

  for (int i = 0; i < 7; i++) {
    float v = pow(2., float(i));

    d += (1.5 / v) * snoise(vec3(uv + (float(i) / 17.), 1), v * s);
  }

  return saturate((1. - d) * .5) * 2.;
}

// ---------------------------------------------------

float ridge(vec2 p, float t) {
  vec2 q = p + t * -normalize(p);
  float a = atan(q.y, q.x), l = sqrt(length(q));

  return fract(3. * a + 5. - 5. * l) - .5;
}

float cloud(vec2 p, float s) { return noise(p, s); }

vec2 iPlane(vec2 ro, vec2 rd, vec2 po, vec2 pd) {
  float d = dot(po - ro, pd) / dot(rd, pd);
  return d * rd + ro;
}

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;
  float t = iTime;

  float bt = S(uv.y, .02 * R.z);
  float x = fract(st.x / 1.5) - .5;
  float y = x - (fract(mo.x / 1.5) - .5);
  float h = x - y;

  {
    vec2 p = st;
    vec2 rd = -normalize(p) * .5;

    vec2 c = vec2(.25);
    vec2 nor = normalize(c - (p / 10. + length(p)));

    vec2 pos = iPlane(c * p, rd, c, nor);
    float dd = 1. / dot(pos, pos);
    pos = mix(c, pos * sqrt(dd), pow(dd, 2.));

    p += pos;

    {
      float dc = 1. - length(p) * 2.;
      float pdc = pow(dc, 3.5);
      float t1 = fract(t * .1 + .5);
      float t2 = fract(t * .1);
      float lerp = abs((.5 - t1) / .5);

      float d = mix(cloud(p + t1 * rd, 7.), cloud(p + t2 * rd, 7.), lerp) * pdc;

      vec3 c1 = hsv(vec3(h, 1, 1)), c2 = hsv(vec3(h, -1, 1));

      col = saturate(mix(col, mix(c1, c2, pdc), d));
    }
  }

  col = mix(col, hsv(vec3(x, 1, 1)) - SMP(y, .001), bt);
  fragColor = vec4(saturate(col), 1.);
}
