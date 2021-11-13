float easeOutQuad(float t) { return -1. * t * (t - 2.); }

float easeOutCubic(float t) { return (t = t - 1.) * t * t + 1.; }

float easeInCubic(float t) { return t * t * t; }

float easeOutQuad(float t) { return -1. * t * (t - 2.); }

float easeInOutExpo(float t) {
  if (t == 0. || t == 1.) {
    return t;
  }

  if ((t *= 2.) < 1.) {
    return .5 * pow(2., 10. * (t - 1.));
  }

  return .5 * (-pow(2., -10. * (t - 1.)) + 2.);
}

float easeInOutCubic(float t) {
  if ((t *= 2.) < 1.) {
    return .5 * t * t * t;
  }

  return .5 * ((t -= 2.) * t * t + 2.);
}

float clockWipe(vec2 p, float t) {
  float a = atan(-p.x, -p.y);
  return float(t * TWOPI > a + PI);
}

float linearStep(float a, float b, float t) { return (t - a) / (b - a); }

float linearstepUpDown(float a0, float a1, float b0, float b1, float t) {
  return linearStep(a0, a1, t) - linearStep(b0, b1, t);
}

float stepUpDown(float a, float b, float t) { return S(a, t) - S(b, t); }