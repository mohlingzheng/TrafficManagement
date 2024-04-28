using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button StartButton;
    private void Start()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in the scene.");
            return;
        }
        EventSystem.current.SetSelectedGameObject(StartButton.gameObject);
    }

    public void StartSimulation()
    {
        SceneManager.LoadScene("Scenes/ZoneSelection");
    }

    public void StartKomtarScene()
    {
        SceneManager.LoadScene("Scenes/KomtarScene");
    }

    public void QuitSimulation()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
