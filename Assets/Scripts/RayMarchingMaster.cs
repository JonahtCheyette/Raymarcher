using UnityEngine;
using System.Collections.Generic;

public class RayMarchingMaster : MonoBehaviour {

    public ComputeShader rayMarchingShader;
    public Light lighting;

    private Camera _camera;

    private ComputeBuffer sphereBuffer;
    private ComputeBuffer planeBuffer;
    private ComputeBuffer boxBuffer;
    private ComputeBuffer torusBuffer;
    private ComputeBuffer cylinderBuffer;

    public Sphere[] spheres;
    public Plane[] planes;
    public Box[] boxes;
    public Torus[] tori;
    public Cylinder[] cylinders;

    //the texture that will be filled by the shader, then blited to the screen
    private RenderTexture target;

    //called every frame by unity, which automatically passes in what's already rendered as the source and the camera's target (in most cases, the screen) as the destination
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        SetDynamicShaderParameters();
        Render(source, destination);
    }

    private void Render(RenderTexture source, RenderTexture destination) {
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        rayMarchingShader.SetTexture(0, "result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 4f);
        rayMarchingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        Graphics.Blit(target, destination);
    }

    private void InitRenderTexture() {
        if (target == null || target.width != Screen.width || target.height != Screen.height) {
            // Release render texture if we already have one
            if (target != null) {
                target.Release();
            }

            // Get a render target for Ray Tracing
            target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }

    private void Awake() {
        _camera = GetComponent<Camera>();
        SetUpScene();
        SetConstantShaderParameters();
    }

    private void SetUpScene() {
        SetupBuffers();
        InitSpheres();
        InitPlanes();
        InitBoxes();
        InitTori();
        InitCylinders();
    }

    private void SetupBuffers() {
        sphereBuffer = new ComputeBuffer(1, 28);
        rayMarchingShader.SetBuffer(0, "spheres", sphereBuffer);
        sphereBuffer.Release();

        planeBuffer = new ComputeBuffer(1, 28);
        rayMarchingShader.SetBuffer(0, "planes", planeBuffer);
        planeBuffer.Release();

        boxBuffer = new ComputeBuffer(1, 52);
        rayMarchingShader.SetBuffer(0, "boxes", boxBuffer);
        boxBuffer.Release();

        torusBuffer = new ComputeBuffer(1, 48);
        rayMarchingShader.SetBuffer(0, "tori", torusBuffer);
        torusBuffer.Release();

        cylinderBuffer = new ComputeBuffer(1, 48);
        rayMarchingShader.SetBuffer(0, "cylinders", cylinderBuffer);
        cylinderBuffer.Release();
    }

    private void InitSpheres() {
        if(spheres.Length != 0) {
            sphereBuffer = new ComputeBuffer(spheres.Length, 28);
            sphereBuffer.SetData(spheres);
            rayMarchingShader.SetBuffer(0, "spheres", sphereBuffer);
        }
    }

    private void InitPlanes() {
        if (planes.Length != 0) {
            planeBuffer = new ComputeBuffer(planes.Length, 28);
            planeBuffer.SetData(planes);
            rayMarchingShader.SetBuffer(0, "planes", planeBuffer);
        }
    }

    private void InitBoxes() {
        if (boxes.Length != 0) {
            boxBuffer = new ComputeBuffer(boxes.Length, 52);
            boxBuffer.SetData(boxes);
            rayMarchingShader.SetBuffer(0, "boxes", boxBuffer);
        }
    }

    private void InitTori() {
        if (tori.Length != 0) {
            torusBuffer = new ComputeBuffer(tori.Length, 48);
            torusBuffer.SetData(tori);
            rayMarchingShader.SetBuffer(0, "tori", torusBuffer);
        }
    }

    private void InitCylinders() {
        if (cylinders.Length != 0) {
            cylinderBuffer = new ComputeBuffer(cylinders.Length, 48);
            cylinderBuffer.SetData(cylinders);
            rayMarchingShader.SetBuffer(0, "cylinders", cylinderBuffer);
        }
    }

    private void SetConstantShaderParameters() {
        
    }

    private void SetDynamicShaderParameters() {
        rayMarchingShader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        float[] light = new float[3];
        light[0] = lighting.transform.position.x;
        light[1] = lighting.transform.position.y;
        light[2] = lighting.transform.position.z;
        rayMarchingShader.SetFloats("light", light);
    }

    private void OnDisable() {
        if (sphereBuffer != null) {
            sphereBuffer.Release();
        }

        if (planeBuffer != null) {
            planeBuffer.Release();
        }

        if (boxBuffer != null) {
            boxBuffer.Release();
        }

        if (torusBuffer != null) {
            torusBuffer.Release();
        }

        if (cylinderBuffer != null) {
            cylinderBuffer.Release();
        }
    }
}

[System.Serializable]
public struct Sphere {
    public Vector3 position;
    public float radius;
    public Vector3 albedo;

    public Sphere(Vector3 pos, float r, Vector3 color) {
        position = pos;
        radius = r;
        albedo = color;
    }
}

[System.Serializable]
public struct Plane {
    public float distAlongAxis;
    public Vector3 normal;
    public Vector3 albedo;

    public Plane(Vector3 axis, float dist, Vector3 color) {
        normal = axis;
        distAlongAxis = dist;
        albedo = color;
    }
}

[System.Serializable]
public struct Box {
    public Vector3 position;
    public Vector3 size;
    public Vector4 rotation;
    public Vector3 albedo;

    public Box(Vector3 pos, Vector3 s, Vector4 rot, Vector3 color) {
        position = pos;
        size = s;
        rotation = rot;
        albedo = color;
    }
}

[System.Serializable]
public struct Torus {
    public Vector3 position;
    public Vector2 dimensions;
    public Vector4 rotation;
    public Vector3 albedo;

    public Torus(Vector3 pos, Vector2 sizes, Vector4 rot, Vector3 color) {
        position = pos;
        dimensions = sizes;
        rotation = rot;
        albedo = color;
    }
}

[System.Serializable]
public struct Cylinder {
    public Vector3 position;
    public float height;
    public float radius;
    public Vector4 rotation;
    public Vector3 albedo;

    public Cylinder(Vector3 pos, float h, float r, Vector4 rot, Vector3 color) {
        position = pos;
        height = h;
        radius = r;
        rotation = rot;
        albedo = color;
    }
}
