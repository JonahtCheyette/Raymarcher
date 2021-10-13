using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDataPasser : BaseShapeDataPasser {
    [Min(0)]
    public float radius;

    protected override void OnValidate() {
        base.OnValidate();
        transform.localScale = Vector3.one * radius * 2;
    }

    protected override void AssignMesh() {
        if(mesh == null) {
            mesh = new Mesh();
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            temp.GetComponent<MeshFilter>().sharedMesh.GetVertices(vertices);
            temp.GetComponent<MeshFilter>().sharedMesh.GetNormals(normals);
            int[] tris = temp.GetComponent<MeshFilter>().sharedMesh.GetTriangles(0);
            DestroyImmediate(temp);
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris;
            mesh.normals = normals.ToArray();
        }
        base.AssignMesh();
    }

    protected override Vector3 GetInfo() {
        return Vector3.right * radius;
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.sphere;
    }
}
