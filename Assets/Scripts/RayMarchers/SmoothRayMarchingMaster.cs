using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothRayMarchingMaster : BaseRayMarchingMaster {
    [Min(0)]
    public float smoothing;

    public override void SetDynamicShaderParameters() {
        base.SetDynamicShaderParameters();
        rayMarchingShader.SetFloat("smoothing", smoothing);
    }
}
