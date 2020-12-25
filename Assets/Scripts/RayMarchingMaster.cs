using UnityEngine;
using System.Collections.Generic;

public class RayMarchingMaster : MonoBehaviour {

    public ComputeShader rayMarchingShader;
    public Light lighting;

    private Camera _camera;
    private ComputeBuffer spheres;
    private ComputeBuffer planes;

    //the texture that will be filled by the shader, then blited to the screen
    private RenderTexture target;

    //called every frame by unity, which automatically passes in what's already rendered as the source and the camera's target (in most cases, the screen) as the destination
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        SetShaderParameters();
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
    }

    private void SetUpScene() {
        Sphere[] tempSpheres = new Sphere[1];
        Plane[] tempPlanes = new Plane[1];

        tempSpheres[0] = new Sphere(Vector3.up, 0.5f, Vector3.one);
        tempPlanes[0] = new Plane(Vector3.up, 0, Vector3.one);

        spheres = new ComputeBuffer(1, 28);
        planes = new ComputeBuffer(1, 28);

        spheres.SetData(tempSpheres);
        planes.SetData(tempPlanes);
    }

    private void SetShaderParameters() {
        rayMarchingShader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        rayMarchingShader.SetBuffer(0, "spheres", spheres);
        rayMarchingShader.SetBuffer(0, "planes", planes);
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
