# GLSL findings

### To lerp between two states:

```
float t = iTime;
float t0 = fract(t * .1 + .5);
float t1 = fract(t * .1);
float lerp = abs((.5 - t0) / .5);

vec2 p = st;
vec2 rd = -normalize(p) * .4;

float d0 = length(p + t0 * rd);
float d1 = length(p + t1 * rd);

float d = mix(d0, d1, lerp);
```

### Add an HSV bar on the bottom:

```
vec3 col;

float bt = S(uv.y, .02 * R.z);
float x = fract(st.x / 1.5) - .5;
float y = x - (fract(mo.x / 1.5) - .5);

col = mix(col, hsv(vec3(x, 1, 1)) - SMP(y, .001), bt);
```

And then can use `x - y` to generate a nice hue value for other shapes.
