using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineRayMarchingMaster : BaseRayMarchingMaster {
    [Range(0, 1)]
    public float outlineStrength;

    [Min(0)]
    public float outlineSize;

    public override void SetDynamicShaderParameters() {
        base.SetDynamicShaderParameters();
        rayMarchingShader.SetFloat("outlineSize", outlineSize);
        rayMarchingShader.SetFloat("outlineStrength", outlineStrength);
    }
}
