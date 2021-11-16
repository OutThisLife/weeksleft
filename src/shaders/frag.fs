#version 300 es
precision highp float;

uniform vec2 iMouse;
uniform float iTime;
uniform float iFrame;
uniform sampler2D iChannel0;

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
#define dot2(v) dot(v, v)

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

  float t = iTime;
  float t0 = fract(t * .3 + .5);
  float t1 = fract(t * .3);
  float lerp = abs((.5 - t0) / .5);

  vec3 col;

  {
    vec2 p = st * 2.;
    float fog = SM(.1, -.01, abs(p.y + .25));

    col += vec3(0, .1, .2);

    // Grid
    if (p.y <= -.25) {
      p.y = 4. / abs(p.y + .2);
      p.x *= p.y;

      vec2 s = vec2(p.y, p.y * p.y * .2) * .005;
      p += vec2(0, t * 2.);
      p = abs(fract(p) - .5);

      vec2 l = SM(s, vec2(0), p);
      l += SM(s * 5., vec2(0), p) * .5;

      float d = length(l);

      col = mix(col, vec3(1, .5, 1), d);
    } else {
      col += vec3(1, .2, 1);

      float id = min(p.y * 4.5 - .5, 1.);
      p.y -= 1.1 - .51;

      vec2 sun = p, fuji = p;

      // Sun
      {
        vec2 p = sun += vec2(.7, .3);

        float r = length(p);
        float v = SM(.3, .29, r);
        float b = SM(.7, 0., length(p - vec2(0, .1)));

        float c = 3. * sin((p.y + t * .1) * 1e2);
        c += clamp(p.y * 17. + 1., -6., 6.);
        c = saturate(c);

        float d = saturate(v * c) + b * .5;

        col = mix(col, vec3(1, .4, .1), p.y * 2. + .2) * d;
      }

      // Fuji
      {
        float d, w;

        {
          float r1 = 1.75 + pow(p.y * p.y, 2.1);
          float r2 = .2;
          float he = .5;

          vec2 p = fuji += vec2(-.7 + sun.y * 0., .5);
          vec2 k1 = vec2(r2, he);
          vec2 k2 = vec2(r2 - r1, 2. * he);

          p.x = abs(p.x);

          vec2 a = vec2(p.x - min(p.x, p.y < 0. ? r1 : r2), abs(p.y) - he);
          vec2 b = p - k1 + k2 * saturate(dot(k1 - p, k2) / dot2(k2));

          d = (b.x < 0. && a.y < 0.) ? -1. : 1.;
          d = sign(max(b.x, b.y));
          d *= sqrt(min(dot2(a), dot2(b)));
        }

        {
          w = p.y + sin(p.x * 20.) * .05 + .2;
          w = SM(0., .01, w);
        }

        col = mix(col, mix(vec3(0, 0, .2), vec3(1, 0, .8), id), S(d, 0.));
        col = mix(col, vec3(1, .5, 1), w * S(d, 0.));
        col = mix(col, vec3(1, .5, 1), 1. - SM(0., .01, abs(d)));

        col += mix(vec3(1, .12, .8), vec3(0, 0, .2), saturate(p.y * 3.5 + 3.)) *
               S(0., d);
      }

      // Cloud(s)
      {
        vec2 p = p;
        p.x = mod(p.x, 4.) - 2.;

        float t = t * .5;
      }
    }

    col = mix(col, vec3(1, .5, .7), pow(fog, 3.));
  }

  fragColor = vec4(saturate(col), 1);
}
