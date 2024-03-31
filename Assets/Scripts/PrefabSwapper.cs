using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabSwapper : EditorWindow
{
    public GameObject newPrefab;

    [MenuItem("Tools/Prefab Swapper")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSwapper>("Prefab Swapper");
    }

    void OnGUI()
    {
        newPrefab = (GameObject)EditorGUILayout.ObjectField("New Prefab", newPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Prefab"))
        {
            if (newPrefab != null)
            {
                foreach (GameObject selectedObject in Selection.gameObjects)
                {
                    // Destroy the current game object and instantiate the new prefab
                    GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
                    newObject.transform.position = selectedObject.transform.position;
                    newObject.transform.rotation = selectedObject.transform.rotation;
                    newObject.transform.localScale = selectedObject.transform.localScale;

                    // Preserve the hierarchy and components of the old object
                    newObject.transform.parent = selectedObject.transform.parent;
                    newObject.name = selectedObject.name;

                    // Destroy the old object
                    Undo.DestroyObjectImmediate(selectedObject);
                }
            }
            else
            {
                Debug.LogError("Please select a new prefab.");
            }
        }
    }
}
