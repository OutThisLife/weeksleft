vec3 blendOverlay(vec3 base, vec3 blend) {
  return mix(1. - 2. * (1. - base) * (1. - blend), 2. * base * blend,
             step(base, vec3(.5)));
}

vec3 vignette(vec2 p, float radius) {
  p /= radius;
  p -= vec2(-0.1, -0.1);

  float dist = length(p);
  dist = smoothstep(-.33, .99, 1. - dist);

  vec3 col = mix(vec3(0.33), vec3(1.), dist);
  vec3 noise = vec3(random(p * 1.5), random(p * 2.5), random(p));

  return mix(col, blendOverlay(col, noise), 0.025);
}