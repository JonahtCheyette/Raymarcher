using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PlaneDataPasser : BaseShapeDataPasser {
    public float distanceAlongNormal;

    protected override void OnValidate() {
        base.OnValidate();
        transform.position = transform.up * distanceAlongNormal;
        transform.localScale = Vector3.one * 1000000f;
    }

    protected override void AssignMesh() {
        if (mesh == null) {
            mesh = new Mesh();
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            List<Vector3> vertices = new List<Vector3>();
            temp.GetComponent<MeshFilter>().sharedMesh.GetVertices(vertices);
            int[] tris = temp.GetComponent<MeshFilter>().sharedMesh.GetTriangles(0);
            DestroyImmediate(temp);
            mesh.vertices = vertices.ToArray();
            mesh.triangles = tris;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++) {
                verts[i] = Quaternion.AngleAxis(90f, Vector3.right) * verts[i];
            }
            mesh.vertices = verts;
            mesh.RecalculateNormals();
        }
        base.AssignMesh();
    }

    private void Update() {
        if (transform.hasChanged) {
            OnValidate();
            transform.hasChanged = false;
        }
    }

    protected override Vector3 GetInfo() {
        return Vector3.right * distanceAlongNormal;
    }

    protected override Vector3 GetPosition() {
        return transform.up;
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.plane;
    }
}
