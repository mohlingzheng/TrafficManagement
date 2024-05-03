using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KomtarSceneManager : MonoBehaviour
{
    Scene currentScene;
    public float timer = 0f;
    public GameObject endSimulationPanel;
    public Button Ok;
    public InputManager inputManager;
    public CameraController cameraController;
    public TimeTrackingManager timeTrackingManager;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        inputManager = FindAnyObjectByType<InputManager>();
        cameraController = FindAnyObjectByType<CameraController>();
        timeTrackingManager = FindAnyObjectByType<TimeTrackingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3f)
        {
            Button[] buttons = FindObjectsOfType<Button>();
            foreach (Button b in buttons)
            {
                b.interactable = false;
            }
            endSimulationPanel.SetActive(true);
            Ok.interactable = true;
            inputManager.RemoveInputModeChoice();
            inputManager.enabled = false;
            cameraController.enabled = false;
            EventSystem.current.SetSelectedGameObject(Ok.gameObject);
        }
    }
}
