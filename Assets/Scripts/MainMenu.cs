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
    public AudioSource audioSource;
    public static float currentVolume;
    public float fixedVolume = 0.2f;
    private void Start()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in the scene.");
            return;
        }
        audioSource = FindAnyObjectByType<AudioSource>();
        audioSource.volume = currentVolume;
        
        currentScene = SceneManager.GetActiveScene();

        Time.timeScale = TimeScale.normal;
        KomtarSceneManager.IsPaused = false;

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

    public void ToggleSound()
    {
        if (audioSource.volume == 0)
        {
            audioSource.volume = fixedVolume;
            currentVolume = audioSource.volume;
        }
        else
        {
            audioSource.volume = 0f;
            currentVolume = audioSource.volume;
        }
    }

    public void QuitSimulation()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
