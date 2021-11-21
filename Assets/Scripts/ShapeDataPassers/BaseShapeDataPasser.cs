using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShapeDataPasser : MonoBehaviour {
    public Color color = Color.white;

    protected Mesh mesh;
    protected MeshFilter filter;

    protected virtual Vector3 GetPosition() {
        return transform.position;
    }

    protected virtual Vector4 GetRotation() {
        return new Vector4(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    protected virtual Vector3 GetColor() {
        return new Vector3(color.r, color.g, color.b);
    }

    protected virtual Vector3 GetInfo() {
        print($"ShapeInfo not provided on shape {name}");
        return Vector3.zero;
    }

    protected virtual ShapeType GetShapeType() {
        print($"ShapeTupe not provided on shape {name}");
        return ShapeType.box;
    }

    public ShapeData GetShapeData() {
        ShapeData shape = new ShapeData();
        shape.position = GetPosition();
        shape.rotation = GetRotation();
        shape.albedo = GetColor();
        shape.shapeInfo = GetInfo();
        shape.shapeType = GetShapeType();
        return shape;
    }

    protected virtual void OnValidate() {
        if (transform.gameObject.activeInHierarchy) {
            if (filter == null) {
                filter = GetComponent<MeshFilter>();
            }

            BaseRayMarchingMaster[] raymarchers = FindObjectsOfType<BaseRayMarchingMaster>();
            foreach (BaseRayMarchingMaster raymarcher in raymarchers) {
                if (raymarcher.enabled) {
                    raymarcher.OnValidate();
                }
            }

            //this has the exact same effect as putting AssingMesh() in the onvalidate call, we just don't get annoying error messages
            UnityEditor.EditorApplication.delayCall += AssignMesh;
        }
    }

    protected virtual void AssignMesh() {
        UnityEditor.EditorApplication.delayCall -= AssignMesh;
        if (mesh != null && filter != null) {
            Color[] cols = new Color[mesh.vertexCount];
            for (int i = 0; i < cols.Length; i++) {
                cols[i] = color;
            }
            mesh.colors = cols;
            if (Application.isPlaying) {
                filter.mesh = mesh;
            } else {
                filter.sharedMesh = mesh;
            }
        }
    }

    protected virtual void OnEnable() {
        BaseRayMarchingMaster[] raymarchers = FindObjectsOfType<BaseRayMarchingMaster>();
        foreach (BaseRayMarchingMaster raymarcher in raymarchers) {
            if (raymarcher.enabled) {
                raymarcher.UpdateShapeList(this, false);
            }
        }
    }

    private void OnDisable() {
        BaseRayMarchingMaster[] raymarchers = FindObjectsOfType<BaseRayMarchingMaster>();
        foreach (BaseRayMarchingMaster raymarcher in raymarchers) {
            if (raymarcher.enabled) {
                raymarcher.UpdateShapeList(this, true);
            }
        }
    }

    private void OnDestroy() {
        OnDisable();
    }

    private void Update() {
        if (transform.hasChanged) {
            BaseRayMarchingMaster[] raymarchers = FindObjectsOfType<BaseRayMarchingMaster>();
            foreach (BaseRayMarchingMaster raymarcher in raymarchers) {
                if (raymarcher.enabled) {
                    raymarcher.OnValidate();
                }
            }
            transform.hasChanged = false;
        }
    }
}

public enum ShapeType {
    box, sphere, plane, torus, cylinder, capsule
}

public struct ShapeData {
    public ShapeType shapeType;
    //normal in the case of the plane
    public Vector3 position;
    public Vector4 rotation;
    public Vector3 shapeInfo;
    public Vector3 albedo;

    public static int GetSize() {
        return sizeof(ShapeType) + 13 * sizeof(float);
    }
}