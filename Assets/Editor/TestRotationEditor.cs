using UnityEngine;
using UnityEditor;

//says to use this editor for the ExpierementalTerrainHandler Script
[CustomEditor(typeof(TestRotation))]
public class TestRotationEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the ExpierementalTerrainHandler reference
        TestRotation rotationTester = (TestRotation)target;
        DrawDefaultInspector();

        //creates a button that calls GenerateChunks when Pressed
        if (GUILayout.Button("Print Rotatiom")) {
            rotationTester.Test();
        }
    }
}

