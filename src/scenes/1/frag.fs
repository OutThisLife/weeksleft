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

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// ---------------------------------------------------

float avg(vec4 p) { return (p.r + p.g + p.b) * .07; }
float avg(vec3 p) { return avg(vec4(p, 1.)); }

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

    const vec4 colorA = vec4(.912, .191, .652, 1);
    const vec4 colorB = vec4(1, .777, .3, 1);

    vec4 t0 = texture(tex0, p), t1 = texture(tex0, 1. - p.yx);
    float d = S(p.x, .5);

    col = mix(colorA, colorB, p.y);
    col += avg(t0 + t1) * mix(t1.rbba, t0, d);
  }

  fragColor = saturate(col);
}
