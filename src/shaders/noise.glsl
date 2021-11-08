const mat2 myt = mat2(.12121212, .13131313, -.13131313, .12121212);
const vec2 mys = vec2(1e4, 1e6);

float hash(float n) { return fract(sin(n) * 753.5453123); }

vec3 hash(vec3 p) {
  return fract(
      sin(vec3(dot(p, vec3(1., 57., 113.)), dot(p, vec3(57., 113., 1.)),
               dot(p, vec3(113., 1., 57.)))) *
      43758.5453);
}

float hash21(vec3 p) {
  p = fract(p * vec3(123.34, 456.21, p.z));
  p += dot(p, p + 45.32);

  return fract(p.x * p.y * p.z);
}

vec2 rhash(vec2 uv) {
  uv *= myt;
  uv *= mys;
  return fract(fract(uv / mys) * uv);
}

float rand(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float rand(float s) { return rand(vec2(s, dot(s, s))); }

float noise(in vec2 p) {
  vec2 i = floor(p);
  vec2 f = fract(p);

  float a = rand(i);
  float b = rand(i + vec2(1., 0.));
  float c = rand(i + vec2(0., 1.));
  float d = rand(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float noise(in vec3 x) {
  vec3 p = floor(x);
  vec3 f = fract(x);
  f = f * f * (3. - 2. * f);

  float n = p.x + p.y * 157. + 113. * p.z;
  return mix(mix(mix(hash(n + 0.), hash(n + 1.), f.x),
                 mix(hash(n + 157.), hash(n + 158.), f.x), f.y),
             mix(mix(hash(n + 113.), hash(n + 114.), f.x),
                 mix(hash(n + 270.), hash(n + 271.), f.x), f.y),
             f.z);
}

float fbm(vec2 p, float t, float amplitude, float s, float a) {
  float mask = length(p), amp = amplitude;

  for (int i = 0; i < 4; i++) {
    p *= R(s, a);
    t += noise(p) * amp;
    amp *= amplitude;
  }

  return t - mask;
}

float fbm(vec2 p) { return fbm(p, .5, .5, 1.6, 1.2); }

float voronoi2d(const vec2 st) {
  vec2 p = floor(st);
  vec2 f = fract(st);

  float res = 0.;

  for (int j = -1; j <= 1; j++) {
    for (int i = -1; i <= 1; i++) {
      vec2 b = vec2(i, j);
      vec2 r = vec2(b) - f + rhash(p + b);

      res += 1. / pow(dot(r, r), 8.);
    }
  }

  return pow(1. / res, .0625);
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