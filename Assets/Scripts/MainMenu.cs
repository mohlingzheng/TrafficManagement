using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button StartButton;
    Scene currentScene;
    private void Start()
    {
        Time.timeScale = 1.0f;
        KomtarSceneManager.IsPaused = false;
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in the scene.");
            return;
        }
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "MainMenu" || currentScene.name == "ZoneSelection" || currentScene.name == "Conclusion")
        {
            EventSystem.current.SetSelectedGameObject(StartButton.gameObject);
        }
    }

    public void StartMainMenuScene()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void StartSimulation()
    {
        SceneManager.LoadScene("Scenes/ZoneSelection");
    }

    public void StartKomtarScene()
    {
        SceneManager.LoadScene("Scenes/KomtarScene");
    }

    public void StartConclusionScene()
    {
        SceneManager.LoadScene("Scenes/Conclusion");
    }

    public void QuitSimulation()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
