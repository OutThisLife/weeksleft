#version 300 es
precision mediump float;

uniform vec3 iResolution;
uniform vec2 iMouse;
uniform float iTime;

in vec2 vUv;
out vec4 fragColor;

#define PI 3.1415926538
#define PHI 2.399963229728653
#define saturate(a) clamp(a, 0., 1.)

vec2 N(float a) { return vec2(sin(a), cos(a)); }

void mainImage(vec2 st) {
  vec3 col;

  {
    float y = smoothstep(.1, .5, fract((st.y) + .25));
    float x = smoothstep(0., .05, abs(abs(st.x) - (y * .05)));
    float m = smoothstep(.05 * (1. - y), 0., x);

    // col += m;
  }

  vec2 uv = st;
  uv.x = abs(uv.x);

  vec2 nor = N((5. / 6.) * PI);

  float scale = 1.;
  const int steps = 10;

  uv.x += .5;

  for (int i = 0; i < steps; i++) {
    uv *= 3.;
    scale *= 3.;
    uv.x -= 1.5;

    uv.x = abs(uv.x) - .5;
    uv -= nor * min(0., dot(uv, nor)) * 2.;
  }

  float d = length(uv - vec2(clamp(uv.x, -1., 1.), 0.));

  col += smoothstep(.005 / iResolution.z, 0., d / scale);

  fragColor = vec4(saturate(pow(col, vec3(1. / 2.2))), 1.);
}

void main() {
  vec2 st = (gl_FragCoord.xy - .5 * iResolution.xy) / iResolution.y;
  vec2 uv = vUv - .5;

  mainImage(uv);
}