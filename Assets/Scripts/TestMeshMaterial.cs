using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshMaterial : MonoBehaviour {
    public Color col = Color.black;
    public int move;

    public MeshFilter filter;

    private void OnValidate() {
        UnityEditor.EditorApplication.delayCall += test;
    }

    private void test() {
        UnityEditor.EditorApplication.delayCall -= test;
        Mesh mesh = new Mesh();

        //vertices
        int numSegmentsRing = 50;
        int numSegmentsOuterCircle = 50;
        Vector3[] vertices = new Vector3[numSegmentsRing * numSegmentsOuterCircle];

        float ringAngleIncrement = 360f / numSegmentsRing;
        float outerCircleAngleIncrement = 360f / numSegmentsOuterCircle;
        for (int i = 0; i < numSegmentsRing; i++) {
            float ringTheta = i * ringAngleIncrement;
            Vector3 outerCircleCenter = Quaternion.AngleAxis(ringTheta, Vector3.up) * Vector3.left * 2;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, outerCircleCenter).normalized;
            Vector3 outerCircleOffset = outerCircleCenter.normalized * 0.5f;
            for (int j = 0; j < numSegmentsOuterCircle; j++) {
                float outerCircleTheta = j * outerCircleAngleIncrement;
                vertices[i * numSegmentsRing + j] = outerCircleCenter + Quaternion.AngleAxis(outerCircleTheta, rotationAxis) * outerCircleOffset;
            }
        }

        //triangles
        int numTriangles = vertices.Length * 2;
        int[] meshTriangles = new int[numTriangles * 3];
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

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        Color[] cols = new Color[mesh.vertices.Length];

        for (int i = 0; i < cols.Length; i++) {
            cols[i] = col;
        }

        mesh.colors = cols;

        filter.sharedMesh = mesh;
    }
}
