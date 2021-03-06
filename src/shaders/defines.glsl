#define R vUvRes
#define Rpx vResolution
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.399963229728653

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(3. / Rpx.y, 0., length(v) - r)
#define rot(a) mat2(cos(a), -sin(a), sin(a), cos(a))

mat3 yuv2rgb =
    mat3(1.0, 0.0, 1.28033, 1.0, -0.21482, -0.38059, 1.0, 2.12798, 0.0);

mat3 rgb2yuv = mat3(0.2126, 0.7152, 0.0722, -0.09991, -0.33609, 0.43600, 0.615,
                    -0.5586, -0.05639);
