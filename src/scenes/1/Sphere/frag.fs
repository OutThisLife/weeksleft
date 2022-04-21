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
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

// ---------------------------------------------------

uniform vec2 iMouse;
uniform float iTime;
uniform samplerCube tCube;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

in vec3 vReflect;
in vec3 vRefract[3];
in float vReflectionFactor;

out vec4 fragColor;

// ---------------------------------------------------

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;
  vec2 mo = iMouse * R.xy;

  vec3 col;

  {
    vec4 refl = texture(tCube, vec3(-vReflect.x, vReflect.yz));

    col.r = texture(tCube, vRefract[0]).r;
    col.g = texture(tCube, vRefract[1]).g;
    col.b = texture(tCube, vRefract[2]).b;

    col = mix(col, refl.rgb, vReflectionFactor);
  }

  fragColor = vec4(saturate(col), 1);
}
