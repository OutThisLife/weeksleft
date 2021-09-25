// clang-format off
#pragma glslify: blend = require(glsl-blend-overlay)
// clang-format on

mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

mat2 scale(vec2 scale) { return mat2(scale.x, 0.0, 0.0, scale.y); }

float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

vec3 opRep(vec3 p, float s) { return mod(p + s * 0.5, s) - s * 0.5; }

float opUnion(float d1, float d2) { return min(d1, d2); }

float opSubtraction(float d1, float d2) { return max(-d1, d2); }

float opIntersection(float d1, float d2) { return max(d1, d2); }

float opSmoothUnion(float d1, float d2, float k) {
  float h = clamp(.5 + .5 * (d2 - d1) / k, 0., 1.);
  return mix(d2, d1, h) - k * h * (1. - h);
}

float opSmoothSubtraction(float d1, float d2, float k) {
  float h = clamp(.5 - .5 * (d2 + d1) / k, 0., 1.);
  return mix(d2, -d1, h) + k * h * (1. - h);
}

float opSmoothIntersection(float d1, float d2, float k) {
  float h = clamp(.5 - .5 * (d2 - d1) / k, 0., 1.);
  return mix(d2, d1, h) + k * h * (1. - h);
}

vec3 vignette(vec2 p, float radius) {
  p /= radius;
  p -= vec2(-0.1, -0.1);

  float dist = length(p);
  dist = smoothstep(-.33, .99, 1. - dist);

  vec3 col = mix(vec3(0.33), vec3(1.), dist);
  vec3 noise = vec3(random(p * 1.5), random(p * 2.5), random(p));

  return mix(col, blend(col, noise), 0.025);
}

float calcSoftshadow(vec3 ro, vec3 rd, float tmin, float tmax, const float k) {
  float res = 1.;
  float t = tmin;

  for (int i = 0; i < 50; i++) {
    float h = map(ro + rd * t);
    res = min(res, k * h / t);
    t += clamp(h, 0.2, 0.20);

    if (res < 0.05 || t > tmax) {
      break;
    }
  }

  return clamp(res, 0., 1.);
}

vec3 calcNormal(vec3 p) {
  const vec3 st = vec3(.001, 0., 0.);

  float x = map(p + st.xyy) - map(p - st.xyy);
  float y = map(p + st.yxy) - map(p - st.yxy);
  float z = map(p + st.yyx) - map(p - st.yyx);

  return normalize(vec3(x, y, z));
}

mat4 viewMatrix(vec3 eye, vec3 center, vec3 up) {
  vec3 f = normalize(center - eye);
  vec3 s = normalize(cross(f, up));
  vec3 u = cross(s, f);

  return mat4(vec4(s, 0.), vec4(u, 0.), vec4(-f, 0.), vec4(0., 0., 0., 1.));
}