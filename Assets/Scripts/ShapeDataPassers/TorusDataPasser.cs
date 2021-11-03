using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorusDataPasser : BaseShapeDataPasser {
    [Min(0)]
    public float ringRadius;
    [Min(0)]
    public float outerCircleRadius;

    private int[] meshTriangles;

    protected override void AssignMesh() {
        mesh = CreateMesh();
        base.AssignMesh();
    }

    private Mesh CreateMesh() {
        Mesh mesh = new Mesh();

        //vertices
        int numSegmentsRing = 50;
        int numSegmentsOuterCircle = 50;
        Vector3[] vertices = new Vector3[numSegmentsRing * numSegmentsOuterCircle];

        float ringAngleIncrement = 360f / numSegmentsRing;
        float outerCircleAngleIncrement = 360f / numSegmentsOuterCircle;
        for (int i = 0; i < numSegmentsRing; i++) {
            float ringTheta = i * ringAngleIncrement;
            Vector3 outerCircleCenter = Quaternion.AngleAxis(ringTheta, Vector3.up) * Vector3.left * ringRadius;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, outerCircleCenter).normalized;
            Vector3 outerCircleOffset = outerCircleCenter.normalized * outerCircleRadius;
            for (int j = 0; j < numSegmentsOuterCircle; j++) {
                float outerCircleTheta = j * outerCircleAngleIncrement;
                vertices[i * numSegmentsRing + j] = outerCircleCenter + Quaternion.AngleAxis(outerCircleTheta, rotationAxis) * outerCircleOffset;
            }
        }

        //triangles
        if (meshTriangles == null || mesh.triangles.Length == 0 || meshTriangles.Length != vertices.Length * 6) {
            int numTriangles = vertices.Length * 2;
            meshTriangles = new int[numTriangles * 3];
            for (int i = 0; i < vertices.Length; i++) {
                //how to get to the next vertex on the same section of outer circle
                //if you're the last vertex on that section of ring, you have to subtract to get to the first one
                int nextVertexOnOuterCircleOffset = (i + 1) % numSegmentsOuterCircle == 0 ? -(numSegmentsOuterCircle - 1) : 1;

                //what you have to add to get from one outer circle to the next
                //simply the number of vertexes on an outer circle, unless this is the last circle, in which case you subtract the index of the first index on the circle
                int nextCircleOffset = (i / numSegmentsOuterCircle) == numSegmentsRing - 1 ? -((numSegmentsRing - 1) * numSegmentsOuterCircle) : numSegmentsOuterCircle;

                meshTriangles[i * 6] = i;
                meshTriangles[i * 6 + 1] = i + nextCircleOffset + nextVertexOnOuterCircleOffset;
                meshTriangles[i * 6 + 2] = i + nextCircleOffset;

                meshTriangles[i * 6 + 3] = i;
                meshTriangles[i * 6 + 4] = i + nextVertexOnOuterCircleOffset;
                meshTriangles[i * 6 + 5] = i + nextCircleOffset + nextVertexOnOuterCircleOffset;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    protected override Vector3 GetInfo() {
        return new Vector3(ringRadius, outerCircleRadius);
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.torus;
    }
}
