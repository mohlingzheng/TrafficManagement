//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(BuildingSpawnPoint))]
//public class BuildingSpawnPointEditor : Editor
//{
//    void OnSceneGUI()
//    {
//        BuildingSpawnPoint spawnPoint = (BuildingSpawnPoint)target;

//        // Draw a line from the spawn point to a point above it
//        Handles.color = Color.green;
//        Handles.DrawLine(spawnPoint.transform.position, spawnPoint.transform.position + Vector3.up * 5f);
//    }

//    public override void OnInspectorGUI()
//    {
//        // Draw the default inspector GUI for the target script
//        DrawDefaultInspector();

//        // Add custom GUI elements below
//        GUILayout.Label("Custom Inspector Content", EditorStyles.boldLabel);
//        GUILayout.Label("This is a custom inspector GUI for BuildingSpawnPoint.");
//    }
//}
