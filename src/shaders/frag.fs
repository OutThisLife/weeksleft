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
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.61803398875
#define TAU 1.618033988749895

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(0., r / Rpx.x, v)
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

vec3 hsv(vec3 c) {
  vec3 p = saturate(abs(mod(c.x * 6. + vec3(0, 4, 2), 6.) - 3.) - 1.);
  p = p * p * (3. - 2. * p);

  return c.z * mix(vec3(1), p, c.y);
}

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) * R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col = hsv(vec3(.3, 1, .2));

  float t = iTime;

  {
    float t = mod(t, 4.);
    vec2 gv = fract(st * 5.) - .5;
    vec2 id = floor(st * 5.);

    float gd = SM(.1, 1., .05 / length(gv));
    col += gd;

    vec2 p = st;
    vec2 rd = -normalize(p);
    vec2 c = vec2(.33);

    float d0 = dot(c - p, rd) / dot(p, rd);
    vec2 pos = c - (d0 * rd + p);

    float d = 1. / dot(pos, pos);

    p = clamp(mix(pos, pos * sqrt(d), d), -4., 4.);
    p = abs(1. - mod(p, 2.));
    p = abs(p) / dot(p, p) - .57;

    col = mix(col, vec3(0), 1. - length(p));
  }

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}
