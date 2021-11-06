using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltTheComputer : ModifierRayMarchingMaster {
    [Min(0)]
    public float smoothing;

    [Range(0, 1)]
    public float outlineStrength;

    [Min(0)]
    public float outlineSize;

    public Color outlineInnerColor;
    public Color outlineOuterColor;

    public IntersectionRayMarchingMaster.IntersectionType[] intersections;

    private ComputeBuffer intersectionBuffer;

    protected override void ResetShapeList() {
        base.ResetShapeList();
        if (shapes != null && intersections.Length != shapes.Length - 1) {
            intersections = new IntersectionRayMarchingMaster.IntersectionType[Mathf.Max(shapes.Length - 1, 0)];
        }
    }

    public override void UpdateShapeList(BaseShapeDataPasser shape, bool remove) {
        UpdateShapes(shape, remove);
        if (shapes != null && intersections.Length != shapes.Length - 1) {
            intersections = new IntersectionRayMarchingMaster.IntersectionType[Mathf.Max(shapes.Length - 1, 0)];
        }
        if (shapes != null && modifiers.Length != shapes.Length) {
            modifiers = new Modifier[Mathf.Max(shapes.Length, 0)];
        }
        UpdateScene();
    }

    public override void SetDynamicShaderParameters() {
        base.SetDynamicShaderParameters();
        rayMarchingShader.SetFloat("outlineSize", outlineSize);
        rayMarchingShader.SetFloat("outlineStrength", outlineStrength);
        rayMarchingShader.SetFloats("inner", new float[] { outlineInnerColor.r, outlineInnerColor.g, outlineInnerColor.b });
        rayMarchingShader.SetFloats("outer", new float[] { outlineOuterColor.r, outlineOuterColor.g, outlineOuterColor.b });
        rayMarchingShader.SetFloat("smoothing", smoothing);
    }

    public override void UpdateScene() {
        base.UpdateScene();
        if (intersections.Length != 0) {
            if (intersections.Length != intersectionBuffer.count) {
                intersectionBuffer.Release();
                intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionRayMarchingMaster.IntersectionType));
            }
            intersectionBuffer.SetData(intersections);
            rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        } else {
            intersectionBuffer.Release();
            intersectionBuffer = new ComputeBuffer(1, sizeof(IntersectionRayMarchingMaster.IntersectionType));
            intersectionBuffer.SetData(new IntersectionRayMarchingMaster.IntersectionType[1]);
            rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        }
    }

    public override void DestroyBuffer() {
        base.DestroyBuffer();
        if (intersectionBuffer != null) {
            intersectionBuffer.Release();
        }
    }

    public override void SetUpScene() {
        base.SetUpScene();
        intersectionBuffer = new ComputeBuffer(1, sizeof(IntersectionRayMarchingMaster.IntersectionType));
        rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        intersectionBuffer.Release();

        if (intersections.Length != 0) {
            intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionRayMarchingMaster.IntersectionType));
            intersectionBuffer.SetData(intersections);
            rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        }
    }
}
