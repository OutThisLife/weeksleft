vec3 blendOverlay(vec3 base, vec3 blend) {
  return mix(1. - 2. * (1. - base) * (1. - blend), 2. * base * blend,
             step(base, vec3(.5)));
}

vec3 vignette(vec2 p, float radius) {
  p /= radius;
  p -= vec2(-.1);

  float dist = length(p);
  dist = smoothstep(-.33, .99, 1. - dist);

  vec3 col = mix(vec3(.33), vec3(1.), dist);
  vec3 noise = vec3(rand(p * 1.5), rand(p * 2.5), rand(p));

  return mix(col, blendOverlay(col, noise), .025);
}

void getColor(vec3 p, vec3 ro, vec3 rd) {
  vec3 nor = calcNormal(p);
  vec3 lig = normalize(vec3(.4, 1., 2.));
  vec3 ref = reflect(rd, nor);
  vec3 hal = normalize(lig - rd);

  float ndotl = abs(dot(-rd, nor));
  float rim = pow(1. - ndotl, 4.);

  // lighting
  float occ = calcAO(p, nor);                        // ambient occlusion
  float amb = sqrt(clamp(.5 + .5 * nor.y, 0., 1.));  // ambient
  float dif = clamp(dot(nor, lig), 0., 1.);          // diffuse

  // backlight
  float bac = clamp(dot(nor, normalize(vec3(-lig.x, 0., -lig.z))), 0., 1.) *
              clamp(1. - nor.y, 0., 1.);
  float dom = smoothstep(-0.1, 0.1, ref.y);               // dome light
  float fre = pow(clamp(1. + dot(nor, rd), 0., 1.), 8.);  // fresnel
  float spe = pow(clamp(dot(ref, hal), 0., .94), 30.);    // specular
}