#define R vUvRes
#define R2 vResolution
#define PI 3.1415926538
#define TWOPI 6.2831853076
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
// #define SM(a, b, c) smoothstep(a, b, c)
#define SM(v, r) smoothstep(3. / R2.y, 0., length(v) - r)
#define hue(h) (.6 + .6 * cos((6.3 * h) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))