using UnityEngine;
using UnityEditor;

public class BaseRayMarchingMaster : MonoBehaviour {

    public enum ShapeType {
        box, sphere, plane, torus, cylinder
    }

    public ComputeShader rayMarchingShader;
    public Light lighting;

    public bool autoUpdate = false;

    private Camera _camera;

    private ComputeBuffer shapeBuffer;

    public Shape[] shapes;

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
        SetupBuffer();
        InitShapes();
    }

    private void SetupBuffer() {
        shapeBuffer = new ComputeBuffer(1, Shape.GetSize());
        rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
        shapeBuffer.Release();
    }

    private void InitShapes() {
        if(shapes.Length != 0) {
            shapeBuffer = new ComputeBuffer(shapes.Length, Shape.GetSize());
            shapeBuffer.SetData(shapes);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
        }
    }

    public virtual void SetConstantShaderParameters() {
        
    }

    public virtual void SetDynamicShaderParameters() {
        rayMarchingShader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        float[] light = new float[3];
        light[0] = lighting.transform.position.x;
        light[1] = lighting.transform.position.y;
        light[2] = lighting.transform.position.z;
        rayMarchingShader.SetFloats("light", light);
    }

    private void DestroyBuffer() {
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

    private void OnValidate() {
        if (autoUpdate && Application.isPlaying && shapeBuffer != null) {
            UpdateScene();
        }
    }

    private void UpdateScene() {
        if (shapes.Length != 0) {
            if (shapes.Length != shapeBuffer.count) {
                shapeBuffer.Release();
                shapeBuffer = new ComputeBuffer(shapes.Length, Shape.GetSize());
            }
            shapeBuffer.SetData(shapes);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
        } else {
            shapeBuffer.Release();
            shapeBuffer = new ComputeBuffer(1, Shape.GetSize());
            shapeBuffer.SetData(new Shape[1]);
            rayMarchingShader.SetBuffer(0, "shapes", shapeBuffer);
        }
    }

    [System.Serializable]
    public struct Shape {
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
}