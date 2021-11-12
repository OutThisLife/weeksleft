vec2 st = (vUv * 2. - 1.) * R.xy;
vec2 uv = gl_FragCoord.xy / Rpx.xy;
vec2 mo = iMouse * R.xy;

vec3 ro = cameraPosition;
vec3 rd =
    normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * vPos).xyz;