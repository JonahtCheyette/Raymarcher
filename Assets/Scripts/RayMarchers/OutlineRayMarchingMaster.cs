using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineRayMarchingMaster : BaseRayMarchingMaster {
    [Range(0, 1)]
    public float outlineStrength;

    [Min(0)]
    public float outlineSize;

    public Color inner;
    public Color outer;

    public override void SetDynamicShaderParameters() {
        base.SetDynamicShaderParameters();
        rayMarchingShader.SetFloat("outlineSize", outlineSize);
        rayMarchingShader.SetFloat("outlineStrength", outlineStrength);
        rayMarchingShader.SetFloats("inner", new float[] { inner.r, inner.g, inner.b });
        rayMarchingShader.SetFloats("outer", new float[] { outer.r, outer.g, outer.b });
    }
}
