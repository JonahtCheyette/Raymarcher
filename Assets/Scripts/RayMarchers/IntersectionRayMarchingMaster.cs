using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionRayMarchingMaster : BaseRayMarchingMaster {
    public enum IntersectionType {
        union, subtraction, intersection
    }

    public IntersectionType[] intersections;

    private ComputeBuffer intersectionBuffer;

    protected override void ResetShapeList() {
        base.ResetShapeList();
        if (shapes != null && intersections.Length != shapes.Length) {
            intersections = new IntersectionType[Mathf.Max(shapes.Length, 0)];
        }
    }

    public override void UpdateShapeList(BaseShapeDataPasser shape, bool remove) {
        UpdateShapes(shape, remove);
        if (shapes != null && intersections.Length != shapes.Length) {
            intersections = new IntersectionType[Mathf.Max(shapes.Length, 0)];
        }
        UpdateScene();
    }

    public override void UpdateScene() {
        base.UpdateScene();
        if (intersections.Length != 0) {
            if (intersectionBuffer == null) {
                intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionType));
            } else if (!intersectionBuffer.IsValid() || intersections.Length != intersectionBuffer.count) {
                intersectionBuffer.Release();
                intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionType));
            }
            intersectionBuffer.SetData(intersections);
        } else {
            intersectionBuffer.Release();
            intersectionBuffer = new ComputeBuffer(1, sizeof(IntersectionType));
            intersectionBuffer.SetData(new IntersectionType[1]);
        }
        rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
    }

    public override void DestroyBuffer() {
        base.DestroyBuffer();
        if (intersectionBuffer != null) {
            intersectionBuffer.Release();
        }
    }
}
