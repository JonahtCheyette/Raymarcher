using UnityEngine;
using UnityEditor;

//says to use this editor for the IntersectionRayMarchingMaster Script
[CustomEditor(typeof(MeltTheComputer))]
public class MeltTheComputerEditor : Editor {

    SerializedProperty shaderProp;
    SerializedProperty lightingProp;

    bool showIntersections = false;
    bool showModifiers = false;

    void OnEnable() {
        // Fetch the objects from the GameObject script to display in the inspector
        shaderProp = serializedObject.FindProperty("rayMarchingShader");
        lightingProp = serializedObject.FindProperty("lighting");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        //gets the rayMarchingMaster reference
        MeltTheComputer rayMarchingMaster = (MeltTheComputer)target;

        //my code to get OnValidate working
        bool oldAutoUpdate = rayMarchingMaster.autoUpdate;
        float oldSmoothing = rayMarchingMaster.smoothing;
        float oldOutlineStrength = rayMarchingMaster.outlineStrength;
        float oldOulineSize = rayMarchingMaster.outlineSize;
        Color oldOutlineInnerColor = rayMarchingMaster.outlineInnerColor;
        Color oldOutlineOuterColor = rayMarchingMaster.outlineOuterColor;
        IntersectionRayMarchingMaster.IntersectionType[] oldIntersections = new IntersectionRayMarchingMaster.IntersectionType[rayMarchingMaster.intersections.Length];
        rayMarchingMaster.intersections.CopyTo(oldIntersections, 0);
        ModifierRayMarchingMaster.Modifier[] oldModifiers = new ModifierRayMarchingMaster.Modifier[rayMarchingMaster.modifiers.Length];
        rayMarchingMaster.modifiers.CopyTo(oldModifiers, 0);

        EditorGUILayout.PropertyField(shaderProp);
        EditorGUILayout.PropertyField(lightingProp);
        rayMarchingMaster.autoUpdate = EditorGUILayout.Toggle("Auto Update", rayMarchingMaster.autoUpdate);
        rayMarchingMaster.smoothing = Mathf.Max(EditorGUILayout.FloatField("Smoothing", rayMarchingMaster.smoothing), 0);
        rayMarchingMaster.outlineStrength = EditorGUILayout.Slider("Outline Strength", rayMarchingMaster.outlineStrength, 0, 1);
        rayMarchingMaster.outlineSize = Mathf.Max(EditorGUILayout.FloatField("Outline Size", rayMarchingMaster.outlineSize), 0);
        rayMarchingMaster.outlineInnerColor = EditorGUILayout.ColorField("Outline Inner Color", rayMarchingMaster.outlineInnerColor);
        rayMarchingMaster.outlineOuterColor = EditorGUILayout.ColorField("Outline Outer Color", rayMarchingMaster.outlineOuterColor);

        showIntersections = EditorGUILayout.Foldout(showIntersections, "Intersections");
        if (showIntersections) {
            for (int i = 0; i < rayMarchingMaster.intersections.Length; i++) {
                rayMarchingMaster.intersections[i] = (IntersectionRayMarchingMaster.IntersectionType)EditorGUILayout.EnumPopup($"{rayMarchingMaster.GetShapeName(i)} & {rayMarchingMaster.GetShapeName(i + 1)}", rayMarchingMaster.intersections[i]);
            }
        }

        showModifiers = EditorGUILayout.Foldout(showModifiers, "Modifiers");
        if (showModifiers) {
            for (int i = 0; i < rayMarchingMaster.modifiers.Length; i++) {
                EditorGUILayout.LabelField(rayMarchingMaster.GetShapeName(i));
                rayMarchingMaster.modifiers[i].elongation = EditorGUILayout.Vector3Field("Elongation", rayMarchingMaster.modifiers[i].elongation);
                rayMarchingMaster.modifiers[i].rounding = Mathf.Max(EditorGUILayout.FloatField("Rounding", rayMarchingMaster.modifiers[i].rounding), 0);
                if (EditorGUILayout.Toggle("Onion", rayMarchingMaster.modifiers[i].doOnioning == 1)) {
                    rayMarchingMaster.modifiers[i].doOnioning = 1;
                } else {
                    rayMarchingMaster.modifiers[i].doOnioning = 0;
                }
                rayMarchingMaster.modifiers[i].layerThickness = Mathf.Max(EditorGUILayout.FloatField("Onion Layer Thickness", rayMarchingMaster.modifiers[i].layerThickness), 0.01f);
            }
        }

        serializedObject.ApplyModifiedProperties();

        //my awful code to get OnValidate working
        if (oldAutoUpdate != rayMarchingMaster.autoUpdate || oldSmoothing != rayMarchingMaster.smoothing || oldOutlineStrength != rayMarchingMaster.outlineStrength || oldOulineSize != rayMarchingMaster.outlineSize || oldOutlineInnerColor != rayMarchingMaster.outlineInnerColor || oldOutlineOuterColor != rayMarchingMaster.outlineOuterColor) {
            rayMarchingMaster.OnValidate();
        } else {
            bool OnValidateCalled = false;
            for (int i = 0; i < oldIntersections.Length; i++) {
                if (oldIntersections[i] != rayMarchingMaster.intersections[i]) {
                    rayMarchingMaster.OnValidate();
                    OnValidateCalled = true;
                    break;
                }
            }
            if (!OnValidateCalled) {
                for (int i = 0; i < oldModifiers.Length; i++) {
                    if (oldModifiers[i].doOnioning != rayMarchingMaster.modifiers[i].doOnioning || oldModifiers[i].elongation != rayMarchingMaster.modifiers[i].elongation || oldModifiers[i].rounding != rayMarchingMaster.modifiers[i].rounding || oldModifiers[i].layerThickness != rayMarchingMaster.modifiers[i].layerThickness) {
                        rayMarchingMaster.OnValidate();
                        break;
                    }
                }
            }
        }
    }
}
