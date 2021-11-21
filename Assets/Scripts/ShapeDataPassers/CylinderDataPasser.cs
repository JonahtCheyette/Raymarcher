﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderDataPasser : BaseShapeDataPasser {
    [Min(0)]
    public float height = 2;
    [Min(0)]
    public float radius = 0.5f;

    protected override void OnEnable() {
        base.OnEnable();
        OnValidate();
    }

    protected override void OnValidate() {
        base.OnValidate();
        transform.localScale = new Vector3(radius * 2, height / 2, radius * 2);
    }

    protected override void AssignMesh() {
        if (mesh == null) {
            mesh = new Mesh();
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
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
        return new Vector3(height / 2f, radius);
    }

    protected override ShapeType GetShapeType() {
        return ShapeType.cylinder;
    }
}
