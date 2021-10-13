using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarebonesComputeShaderRenderer : MonoBehaviour {
    public Light lighting;

    private Camera _camera;

    //the texture that will be filled by the shader, then blited to the screen
    private RenderTexture target;

    public ComputeShader shader;

    //called every frame by unity, which automatically passes in what's already rendered as the source and the camera's target (in most cases, the screen) as the destination
    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        shader.SetMatrix("cameraToWorld", _camera.cameraToWorldMatrix);
        shader.SetMatrix("cameraInverseProjection", _camera.projectionMatrix.inverse);
        float[] light = new float[3];
        if (lighting.type == LightType.Directional) {
            light[0] = lighting.transform.forward.x;
            light[1] = lighting.transform.forward.y;
            light[2] = lighting.transform.forward.z;
            shader.SetBool("lightIsPoint", false);
        } else {
            light[0] = lighting.transform.position.x;
            light[1] = lighting.transform.position.y;
            light[2] = lighting.transform.position.z;
            shader.SetBool("lightIsPoint", true);
        }
        shader.SetFloats("light", light);
        Render(source, destination);
    }

    private void Render(RenderTexture source, RenderTexture destination) {
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        shader.SetTexture(0, "result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 4f);
        shader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

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
    }
}
