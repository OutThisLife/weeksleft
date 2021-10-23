vec3 ro = cameraPosition;
vec3 rd =
    normalize(cameraWorldMatrix * cameraProjectionMatrixInverse * vPos).xyz;