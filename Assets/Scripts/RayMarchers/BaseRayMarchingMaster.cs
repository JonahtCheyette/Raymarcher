using UnityEngine;
using System.Linq;

public class BaseRayMarchingMaster : MonoBehaviour {
    public ComputeShader rayMarchingShader;
    public Light lighting;

    public bool autoUpdate = true;

    private Camera _camera;

    private ComputeBuffer shapeBuffer;
    protected BaseShapeDataPasser[] shapes;

    //the texture that will be filled by the shader, then blited to the screen
    private RenderTexture target;

    //keeps weird errors from popping up
    private bool afterOnEnfabledCalled;

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

    public virtual void UpdateShapeList(BaseShapeDataPasser shape, bool remove) {
        UpdateShapes(shape, remove);
        UpdateScene();
    }

    protected void UpdateShapes(BaseShapeDataPasser shape, bool remove) {
        if (shapes == null) {
            shapes = new BaseShapeDataPasser[0];
        }
        int shapeIndex = -1;
        for (int i = 0; i < shapes.Length; i++) {
            if (shapes[i] == shape) {
                shapeIndex = i;
                break;
            }
        }
        if (remove) {
            if (shapes.Length > 0) {
                if (shapeIndex != -1) {
                    BaseShapeDataPasser[] copyList = shapes;
                    shapes = new BaseShapeDataPasser[copyList.Length - 1];
                    int index = 0;
                    for (int i = 0; i < copyList.Length - 1; i++) {
                        if (i == shapeIndex) {
                            index++;
                        }
                        shapes[i] = copyList[index];
                        index++;
                    }
                }
            }
        } else {
            if (shapeIndex != -1) {
                BaseShapeDataPasser[] copyList = shapes;
                shapes = new BaseShapeDataPasser[copyList.Length + 1];
                copyList.CopyTo(shapes, 0);
                shapes[copyList.Length] = shape;
            }
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
        BaseShapeDataPasser[] unorderedShapes = FindObjectsOfType<BaseShapeDataPasser>();
        shapes = unorderedShapes;
        if(shapeBuffer != null || shapeBuffer.IsValid()) {
            shapeBuffer.Release();
        }
        if (shapes.Length != 0) {
            shapeBuffer = new ComputeBuffer(shapes.Length, ShapeData.GetSize());
            shapeBuffer.SetData(shapes.Select(x => x.GetShapeData()).ToArray());
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
        if (lighting != null) {
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
        } else {
            light[0] = -1;
            light[1] = 0;
            light[2] = 0;
            rayMarchingShader.SetBool("lightIsPoint", false);
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
        afterOnEnfabledCalled = false;
    }

    private void OnApplicationQuit() {
        DestroyBuffer();
    }

    private void OnEnable() {
        DestroyBuffer();
        SetUpScene();
        afterOnEnfabledCalled = true;
    }

    public virtual void OnValidate() {
        ResetShapeList();
        if (autoUpdate && afterOnEnfabledCalled) {
            UpdateScene();
        }
    }

    protected virtual void ResetShapeList() {
        BaseShapeDataPasser[] unorderedShapes = FindObjectsOfType<BaseShapeDataPasser>();
        shapes = unorderedShapes;
    }

    public virtual void UpdateScene() {
        if (shapes != null && shapes.Length != 0) {
            if(shapeBuffer == null) {
                shapeBuffer = new ComputeBuffer(shapes.Length, ShapeData.GetSize());
            } else if (!shapeBuffer.IsValid() || shapes.Length != shapeBuffer.count) {
                shapeBuffer.Release();
                shapeBuffer = new ComputeBuffer(shapes.Length, ShapeData.GetSize());
            }
            shapeBuffer.SetData(shapes.Select(x => x.GetShapeData()).ToArray());
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

    public string GetShapeName(int shapeIndex) {
        if(shapeIndex >= 0 && shapeIndex < shapes.Length) {
            return shapes[shapeIndex].gameObject.name;
        }
        return "";
    }
}