using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierRayMarchingMaster : BaseRayMarchingMaster {

    public Modifier[] modifiers;

    private ComputeBuffer modifierBuffer;

    protected override void ResetShapeList() {
        base.ResetShapeList();
        if (shapes != null && modifiers.Length != shapes.Length) {
            modifiers = new Modifier[Mathf.Max(shapes.Length, 0)];
        }
    }

    public override void UpdateShapeList(BaseShapeDataPasser shape, bool remove) {
        UpdateShapes(shape, remove); 
        if (shapes != null && modifiers.Length != shapes.Length) {
            modifiers = new Modifier[Mathf.Max(shapes.Length, 0)];
        }
        UpdateScene();
    }

    public override void UpdateScene() {
        base.UpdateScene();
        if (modifiers.Length != 0) {
            if (!modifierBuffer.IsValid() || modifiers.Length != modifierBuffer.count) {
                modifierBuffer.Release();
                modifierBuffer = new ComputeBuffer(modifiers.Length, Modifier.getSize());
            }
            modifierBuffer.SetData(modifiers);
            rayMarchingShader.SetBuffer(0, "modifiers", modifierBuffer);
        } else {
            modifierBuffer.Release();
            modifierBuffer = new ComputeBuffer(1, Modifier.getSize());
            modifierBuffer.SetData(new Modifier[1]);
            rayMarchingShader.SetBuffer(0, "modifiers", modifierBuffer);
        }
    }

    public override void DestroyBuffer() {
        base.DestroyBuffer();
        if (modifierBuffer != null) {
            modifierBuffer.Release();
        }
    }

    public override void SetUpScene() {
        base.SetUpScene();
        modifierBuffer = new ComputeBuffer(1, Modifier.getSize());
        rayMarchingShader.SetBuffer(0, "modifiers", modifierBuffer);
        modifierBuffer.Release();

        if (modifiers.Length != 0) {
            modifierBuffer = new ComputeBuffer(modifiers.Length, Modifier.getSize());
            modifierBuffer.SetData(modifiers);
            rayMarchingShader.SetBuffer(0, "modifiers", modifierBuffer);
        }
    }
    
    [System.Serializable]
    public struct Modifier {
        public Vector3 elongation;
        public float rounding;
        public int doOnioning;
        public float layerThickness;

        public static int getSize() {
            return sizeof(float) * 5 + sizeof(int);
        }
    }
}
