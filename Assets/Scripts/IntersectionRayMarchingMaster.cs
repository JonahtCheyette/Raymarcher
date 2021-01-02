using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionRayMarchingMaster : BaseRayMarchingMaster {
    public enum IntersectionType {
        union, subtraction, intersection
    }

    public IntersectionType[] intersections;

    private ComputeBuffer intersectionBuffer;

    public override void OnValidate() {
        if (intersections.Length != shapes.Length - 1) {
            intersections = new IntersectionType[Mathf.Max(shapes.Length - 1, 0)];
        }
        base.OnValidate();
    }

    public override void UpdateScene() {
        base.UpdateScene();
        if (intersections.Length != 0) {
            if (intersections.Length != intersectionBuffer.count) {
                intersectionBuffer.Release();
                intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionType));
            }
            intersectionBuffer.SetData(intersections);
            rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        } else {
            intersectionBuffer.Release();
            intersectionBuffer = new ComputeBuffer(1, sizeof(IntersectionType));
            intersectionBuffer.SetData(new IntersectionType[1]);
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
        intersectionBuffer = new ComputeBuffer(1, sizeof(IntersectionType));
        rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        intersectionBuffer.Release();

        if (intersections.Length != 0) {
            intersectionBuffer = new ComputeBuffer(intersections.Length, sizeof(IntersectionType));
            intersectionBuffer.SetData(intersections);
            rayMarchingShader.SetBuffer(0, "intersections", intersectionBuffer);
        }
    }
}
