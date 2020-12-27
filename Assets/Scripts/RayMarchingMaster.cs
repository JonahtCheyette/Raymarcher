using UnityEngine;
using System.Collections.Generic;

public class RayMarchingMaster : MonoBehaviour {

    public ComputeShader rayMarchingShader;
    public Light lighting;

    private Camera _camera;

    private ComputeBuffer spheres;
    private ComputeBuffer planes;
    private ComputeBuffer boxes;
    private ComputeBuffer tori;
    private ComputeBuffer cylinders;

    public bool renderSpheres;
    public bool renderPlanes;
    public bool renderBoxes;
    public bool renderTori;
    public bool renderCylinders;

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
        InitSpheres();
        InitPlanes();
        InitBoxes();
        InitTori();
        InitCylinders();
    }

    private void InitSpheres() {
        if (renderSpheres) {
            Sphere[] tempSpheres = new Sphere[1];
            tempSpheres[0] = new Sphere(Vector3.up, 0.5f, Vector3.one);
            spheres = new ComputeBuffer(1, 28);
            spheres.SetData(tempSpheres);
            rayMarchingShader.SetBuffer(0, "spheres", spheres);
        } else {
            spheres = new ComputeBuffer(1, 28);
            rayMarchingShader.SetBuffer(0, "spheres", spheres);
        }
    }

    private void InitPlanes() {
        if (renderPlanes) {
            Plane[] tempPlanes = new Plane[1];
            tempPlanes[0] = new Plane(Vector3.up, 0, Vector3.one);
            planes = new ComputeBuffer(1, 28);
            planes.SetData(tempPlanes);
            rayMarchingShader.SetBuffer(0, "planes", planes);
        } else {
            planes = new ComputeBuffer(1, 28);
            rayMarchingShader.SetBuffer(0, "planes", planes);
        }
    }

    private void InitBoxes() {
        if (renderBoxes) {
            Box[] tempBoxes = new Box[1];
            tempBoxes[0] = new Box(Vector3.up, Vector3.one, new Vector4(0, 0.4f, 0, 0.9f), Vector3.one);
            boxes = new ComputeBuffer(1, 52);
            boxes.SetData(tempBoxes);
            rayMarchingShader.SetBuffer(0, "boxes", boxes);
        } else {
            boxes = new ComputeBuffer(1, 52);
            rayMarchingShader.SetBuffer(0, "boxes", boxes);
        }
    }

    private void InitTori() {
        if (renderTori) {
            Torus[] tempTori = new Torus[1];
            tempTori[0] = new Torus(Vector3.up, new Vector2(0.2f, 0.1f), new Vector4(0, 0, 0, 1), Vector3.one);
            tori = new ComputeBuffer(1, 48);
            tori.SetData(tempTori);
            rayMarchingShader.SetBuffer(0, "tori", tori);
        } else {
            tori = new ComputeBuffer(1, 48);
            rayMarchingShader.SetBuffer(0, "tori", tori);
        }
    }

    private void InitCylinders() {
        if (renderCylinders) {
            Cylinder[] tempCylinders = new Cylinder[1];
            tempCylinders[0] = new Cylinder(Vector3.up, 5, 1, new Vector4(-0.5f, 0, 0, 0.9f), Vector3.one);
            cylinders = new ComputeBuffer(1, 48);
            cylinders.SetData(tempCylinders);
            rayMarchingShader.SetBuffer(0, "cylinders", cylinders);
        } else {
            cylinders = new ComputeBuffer(1, 48);
            rayMarchingShader.SetBuffer(0, "cylinders", cylinders);
        }
    }

    private void SetConstantShaderParameters() {

    }

    private void SetDynamicShaderParameters() {
        rayMarchingShader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        rayMarchingShader.SetBool("renderSpheres", renderSpheres);
        rayMarchingShader.SetBool("renderPlanes", renderPlanes);
        rayMarchingShader.SetBool("renderBoxes", renderBoxes);
        rayMarchingShader.SetBool("renderTori", renderTori);
        rayMarchingShader.SetBool("renderCylinders", renderCylinders);
        float[] light = new float[3];
        light[0] = lighting.transform.position.x;
        light[1] = lighting.transform.position.y;
        light[2] = lighting.transform.position.z;
        rayMarchingShader.SetFloats("light", light);
    }

    private void OnDisable() {
        if (spheres != null) {
            spheres.Release();
        }

        if (planes != null) {
            planes.Release();
        }

        if (boxes != null) {
            boxes.Release();
        }

        if (tori != null) {
            tori.Release();
        }

        if (cylinders != null) {
            cylinders.Release();
        }
    }
}

public struct Sphere {
    Vector3 position;
    float radius;
    Vector3 albedo;

    public Sphere(Vector3 pos, float r, Vector3 color) {
        position = pos;
        radius = r;
        albedo = color;
    }
}

public struct Plane {
    float distAlongAxis;
    Vector3 normal;
    Vector3 albedo;

    public Plane(Vector3 axis, float dist, Vector3 color) {
        normal = axis;
        distAlongAxis = dist;
        albedo = color;
    }
}

public struct Box {
    Vector3 position;
    Vector3 size;
    Vector4 rotation;
    Vector3 albedo;

    public Box(Vector3 pos, Vector3 s, Vector4 rot, Vector3 color) {
        position = pos;
        size = s;
        rotation = rot;
        albedo = color;
    }
}

public struct Torus {
    Vector3 position;
    Vector2 dimensions;
    Vector4 rotation;
    Vector3 albedo;

    public Torus(Vector3 pos, Vector2 sizes, Vector4 rot, Vector3 color) {
        position = pos;
        dimensions = sizes;
        rotation = rot;
        albedo = color;
    }
}

public struct Cylinder {
    Vector3 position;
    float height;
    float radius;
    Vector4 rotation;
    Vector3 albedo;

    public Cylinder(Vector3 pos, float h, float r, Vector4 rot, Vector3 color) {
        position = pos;
        height = h;
        radius = r;
        rotation = rot;
        albedo = color;
    }
}
