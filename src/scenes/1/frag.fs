#version 300 es
precision highp float;

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
#define dot2(v) dot(v, v)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))

// ---------------------------------------------------

uniform vec2 iMouse;
uniform float iTime;
uniform sampler2D tex0;
uniform sampler2D tex1;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// ---------------------------------------------------

float hash(float n) { return fract(sin(n) * 753.5453123); }

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec4 col;

  float t = iTime;
  float t0 = fract(t * .1 + .5);
  float t1 = fract(t * .1);
  float lerp = abs((.5 - t0) / .5);

  {
    vec2 p = vUv;
    vec2 q = (p - .5) * vec2(2.5, 1.5);
    vec2 g = fract(p * vec2(50, 25));

    float d0 = SM(-.2, .1, dot(mo, st));
    float d1 = .5 + .5 * sin(t);
    float d2 = 1. - S(.5, max(abs(q.x), abs(q.y)));

    float d = min(d1, d2);
    // d = min(d0, d2);
    d = d1;

    vec2 uv1 = p + rot(PI / 4.) * g * d * .1;
    vec2 uv2 = p + rot(PI / 4.) * g * (1. - d) * .1;

    vec4 t0 = texture(tex0, uv1);
    vec4 t1 = texture(tex1, uv2);

    col = mix(t0, t1, d);
  }

  fragColor = saturate(col);
}
