using UnityEngine;
using UnityEditor;

//says to use this editor for the IntersectionRayMarchingMaster Script
[CustomEditor(typeof(IntersectionRayMarchingMaster))]
public class IntersectionRayMarchingMasterEditor : Editor {

    SerializedProperty shaderProp;
    SerializedProperty lightingProp;
    bool showIntersections = false;

    void OnEnable() {
        // Fetch the objects from the GameObject script to display in the inspector
        shaderProp = serializedObject.FindProperty("rayMarchingShader");
        lightingProp = serializedObject.FindProperty("lighting");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        //gets the rayMarchingMaster reference
        IntersectionRayMarchingMaster rayMarchingMaster = (IntersectionRayMarchingMaster)target;

        //my code to get OnValidate working
        bool oldAutoUpdate = rayMarchingMaster.autoUpdate;
        IntersectionRayMarchingMaster.IntersectionType[] oldIntersections = new IntersectionRayMarchingMaster.IntersectionType[rayMarchingMaster.intersections.Length];
        rayMarchingMaster.intersections.CopyTo(oldIntersections, 0);

        EditorGUILayout.PropertyField(shaderProp);
        EditorGUILayout.PropertyField(lightingProp);
        rayMarchingMaster.autoUpdate = EditorGUILayout.Toggle("Auto Update", rayMarchingMaster.autoUpdate);

        showIntersections = EditorGUILayout.Foldout(showIntersections, "Intersections");
        if (showIntersections) {
            for (int i = 0; i < rayMarchingMaster.intersections.Length; i++) {
                rayMarchingMaster.intersections[i] = (IntersectionRayMarchingMaster.IntersectionType)EditorGUILayout.EnumPopup($"{rayMarchingMaster.GetShapeName(i)}", rayMarchingMaster.intersections[i]);
            }
        }
        serializedObject.ApplyModifiedProperties();

        //my code to get OnValidate working
        if (oldAutoUpdate != rayMarchingMaster.autoUpdate) {
            rayMarchingMaster.OnValidate();
        } else {
            for (int i = 0; i < oldIntersections.Length; i++) {
                if (oldIntersections[i] != rayMarchingMaster.intersections[i]) {
                    rayMarchingMaster.OnValidate();
                    break;
                }
            }
        }
    }
}
