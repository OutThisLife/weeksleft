#define PI 3.1415926538
#define TWOPI PI * 2.
#define PHI 2.399963229728653

#define hash(a) fract(sin(a) * 43758.5453)
#define hue(h) (.6 + .6 * cos(h + vec3(0, 23, 21)))
#define R(a, b) mat2(a, -b, b, a)
#define S(a, b) step(a, b)
#define saturate(a) clamp(a, 0., 1.)
#define SM(a, b, c) smoothstep(a, b, c)