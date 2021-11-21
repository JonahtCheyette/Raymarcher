using UnityEngine;
using UnityEditor;

//says to use this editor for the ModifierRayMarchingMaster Script
[CustomEditor(typeof(ModifierRayMarchingMaster))]
public class ModifierRayMarchingMasterEditor : Editor {

    SerializedProperty shaderProp;
    SerializedProperty lightingProp;
    bool showModifiers = false;

    void OnEnable() {
        // Fetch the objects from the GameObject script to display in the inspector
        shaderProp = serializedObject.FindProperty("rayMarchingShader");
        lightingProp = serializedObject.FindProperty("lighting");
    }

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        serializedObject.Update();

        //gets the rayMarchingMaster reference
        ModifierRayMarchingMaster rayMarchingMaster = (ModifierRayMarchingMaster)target;

        //my code to get OnValidate working
        bool oldAutoUpdate = rayMarchingMaster.autoUpdate;
        ModifierRayMarchingMaster.Modifier[] oldModifiers = new ModifierRayMarchingMaster.Modifier[rayMarchingMaster.modifiers.Length];
        rayMarchingMaster.modifiers.CopyTo(oldModifiers, 0);

        EditorGUILayout.PropertyField(shaderProp);
        EditorGUILayout.PropertyField(lightingProp);
        rayMarchingMaster.autoUpdate = EditorGUILayout.Toggle("Auto Update", rayMarchingMaster.autoUpdate);

        showModifiers = EditorGUILayout.Foldout(showModifiers, "Modifiers");
        if (showModifiers) {
            for (int i = 0; i < rayMarchingMaster.modifiers.Length; i++) {
                EditorGUILayout.LabelField(rayMarchingMaster.GetShapeName(i));
                rayMarchingMaster.modifiers[i].elongation = EditorGUILayout.Vector3Field("Elongation", rayMarchingMaster.modifiers[i].elongation);
                rayMarchingMaster.modifiers[i].rounding = Mathf.Max(EditorGUILayout.FloatField("Rounding", rayMarchingMaster.modifiers[i].rounding), 0);
            }
        }
        serializedObject.ApplyModifiedProperties();
        
        //my code to get OnValidate working
        if (oldAutoUpdate != rayMarchingMaster.autoUpdate) {
            rayMarchingMaster.OnValidate();
        } else {
            for (int i = 0; i < oldModifiers.Length; i++) {
                if (oldModifiers[i].elongation != rayMarchingMaster.modifiers[i].elongation || oldModifiers[i].rounding != rayMarchingMaster.modifiers[i].rounding) {
                    rayMarchingMaster.OnValidate();
                    break;
                }
            }
        }
    }
}
