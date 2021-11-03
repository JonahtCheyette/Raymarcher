using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDataPasser : BaseShapeDataPasser {
    protected override Vector3 GetInfo() {
        return transform.localScale / 2;
    }

    protected override void AssignMesh() {
        if (mesh == null) {
            mesh = new Mesh();
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            List<Vector3> vertices = new List<Vector3>();
            temp.GetComponent<MeshFilter>().sharedMesh.GetVertices(vertices);
            int[] tris = temp.GetComponent<MeshFilter>().sharedMesh.GetTriangles(0);
            DestroyImmediate(temp);
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris;
            mesh.RecalculateNormals();
        }
        base.AssignMesh();
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.box;
    }
}
