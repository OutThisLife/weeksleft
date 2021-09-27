float dot2(in vec2 v) { return dot(v, v); }
float dot2(in vec3 v) { return dot(v, v); }
float ndot(in vec2 a, in vec2 b) { return a.x * b.x - a.y * b.y; }

/**
 * Setup utils
 */
vec2 sqFrame(vec2 st) {
  vec2 p = 2. * (gl_FragCoord.xy / st.xy) - 1.;

  p.x *= st.x / st.y;

  return p;
}

vec2 sqFrame(vec2 st, vec3 coord) {
  vec2 p = 2. * (coord.xy / st.xy) - 1.;

  p.x *= st.x / st.y;

  return p;
}

vec3 rayDirection(float fov, vec2 p) {
  vec2 xy = p;
  float z = tan(radians(fov));
  return normalize(vec3(xy, -z));
}

mat4 getCamera(vec3 eye, vec3 center, vec3 up) {
  vec3 f = normalize(center - eye);
  vec3 s = normalize(cross(f, up));
  vec3 u = cross(s, f);

  return mat4(vec4(s, 0.), vec4(u, 0.), vec4(-f, 0.), vec4(0., 0., 0., 1.));
}

/**
 * Noise functions
 */
float random(vec2 st) {
  return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(in vec2 st) {
  vec2 i = floor(st);
  vec2 f = fract(st);

  float a = random(i);
  float b = random(i + vec2(1., 0.));
  float c = random(i + vec2(0., 1.));
  float d = random(i + vec2(1., 1.));

  vec2 u = f * f * (3. - 2. * f);
  return mix(a, b, u.x) + (c - a) * u.y * (1. - u.x) + (d - b) * u.x * u.y;
}

float fbm(in vec2 st, int o) {
  float value = 0.;
  float amplitude = .5;
  float frequency = 0.;

  for (int i = 0; i < o; i++) {
    value += amplitude * noise(st);
    st *= 2.;
    amplitude *= .5;
  }

  return value;
}

/**
 * Vector transformations
 */
mat2 rotate(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

mat2 scale(vec2 scale) { return mat2(scale.x, 0., 0., scale.y); }
