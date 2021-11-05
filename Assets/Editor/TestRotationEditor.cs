using UnityEngine;
using UnityEditor;

//says to use this editor for the TestRotation Script
[CustomEditor(typeof(TestRotation))]
public class TestRotationEditor : Editor {

    public override void OnInspectorGUI() {
        //gets the TestRotation reference
        TestRotation rotationTester = (TestRotation)target;
        DrawDefaultInspector();

        //creates a button that calls Test when Pressed
        if (GUILayout.Button("Print Rotation")) {
            rotationTester.Test();
        }
    }
}

