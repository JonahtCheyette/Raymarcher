using UnityEngine;
using UnityEditor;

public class BaseRayMarchingMaster : MonoBehaviour {
    public ComputeShader rayMarchingShader;
    public Light lighting;

    public bool autoUpdate = false;

    private Camera _camera;

    private ComputeBuffer shapeBuffer;

    protected ShapeData[] shapeData;

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

    private void GetShapes() {
        BaseShapeDataPasser[] shapes = FindObjectsOfType<BaseShapeDataPasser>();
        shapeData = new ShapeData[shapes.Length];
        for (int i = 0; i < shapeData.Length; i++) {
            shapeData[i] = shapes[i].GetShapeData();
        }
    }

    private void Awake() {
        _camera = GetComponent<Camera>();
        SetUpScene();
        SetConstantShaderParameters();
    }

    public virtual void SetUpScene() {
        SetupBuffer();
        InitShapes();
    }

    private void SetupBuffer() {
        if (shapeBuffer == null || !shapeBuffer.IsValid()) {
            shapeBuffer = new ComputeBuffer(1, ShapeData.GetSize());
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
            shapeBuffer.Release();
        }
    }

    private void InitShapes() {
        GetShapes();
        if (shapeData.Length != 0) {
            shapeBuffer = new ComputeBuffer(shapeData.Length, ShapeData.GetSize());
            shapeBuffer.SetData(shapeData);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
        }
    }

    public virtual void SetConstantShaderParameters() {
        rayMarchingShader.SetInt("numShapes", shapeBuffer.count);
    }

    public virtual void SetDynamicShaderParameters() {
        rayMarchingShader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        float[] light = new float[3];
        if (lighting.type == LightType.Directional) {
            light[0] = lighting.transform.forward.x;
            light[1] = lighting.transform.forward.y;
            light[2] = lighting.transform.forward.z;
            rayMarchingShader.SetBool("lightIsPoint", false);
        } else {
            light[0] = lighting.transform.position.x;
            light[1] = lighting.transform.position.y;
            light[2] = lighting.transform.position.z;
            rayMarchingShader.SetBool("lightIsPoint", true);
        }
        rayMarchingShader.SetFloats("light", light);
    }

    public virtual void DestroyBuffer() {
        if (shapeBuffer != null) {
            shapeBuffer.Release();
        }
    }

    private void OnDisable() {
        DestroyBuffer();
    }

    private void OnApplicationQuit() {
        DestroyBuffer();
    }

    private void OnEnable() {
        DestroyBuffer();
        SetupBuffer();
        InitShapes();
    }

    public virtual void OnValidate() {
        if (autoUpdate && Application.isPlaying && shapeBuffer != null) {
            UpdateScene();
        }
    }

    public virtual void UpdateScene() {
        GetShapes();
        if (shapeData.Length != 0) {
            if (shapeData.Length != shapeBuffer.count) {
                shapeBuffer.Release();
                shapeBuffer = new ComputeBuffer(shapeData.Length, ShapeData.GetSize());
            }
            shapeBuffer.SetData(shapeData);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
            rayMarchingShader.SetInt("numShapes", shapeBuffer.count);
        } else {
            shapeBuffer.Release();
            shapeBuffer = new ComputeBuffer(1, ShapeData.GetSize());
            shapeBuffer.SetData(new ShapeData[1]);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
            rayMarchingShader.SetInt("numShapes", shapeBuffer.count);
        }
    }
}