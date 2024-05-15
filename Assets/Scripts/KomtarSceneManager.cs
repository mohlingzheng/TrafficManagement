using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class KomtarSceneManager : MonoBehaviour
{
    Scene currentScene;
    public float timer = 0f;
    public GameObject endSimulationPanel;
    public Button Ok;
    public InputManager inputManager;
    public CameraController cameraController;
    public TimeTrackingManager timeTrackingManager;
    public float gameTime = 300f;
    public static bool IsPaused = false;
    public GameObject PausePanel;
    public Button FirstButton;
    public int currentButton = 0;
    float inputCooldown = 0.1f;
    float lastInputTime = 0f;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        inputManager = FindAnyObjectByType<InputManager>();
        cameraController = FindAnyObjectByType<CameraController>();
        timeTrackingManager = FindAnyObjectByType<TimeTrackingManager>();
        //Time.timeScale = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePauseCondition();
        Timer();
    }

    private void Timer()
    {
        timer += Time.deltaTime;
        if (timer > gameTime)
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
            this.enabled = false;
            PauseTheScene();
        }
    }

    private void HandlePauseCondition()
    {
        if (Input.GetButtonDown("Start"))
        {
            if (!IsPaused)
            {
                PauseTheScene();
            }
            else
            {
                ResumeTheScene();
            }
        }

        if (IsPaused)
        {
            if (Time.realtimeSinceStartup - lastInputTime >= inputCooldown)
            {
                float value = Input.GetAxis("LeftVertical");
                if (value < 0f)
                {
                    currentButton++;
                    currentButton = Mathf.Clamp(currentButton, 0, 2);
                }
                else if (value > 0f)
                {
                    currentButton--;
                    currentButton = Mathf.Clamp(currentButton, 0, 2);
                }

                UpdateButtonsColor(currentButton);

                lastInputTime = Time.realtimeSinceStartup;
            }
        }

        if (IsPaused)
        {
            if (currentButton == 0 && Input.GetButtonDown("A"))
            {
                PausePanel.transform.Find("ButtonPanel").Find("Resume").GetComponent<Button>().onClick.Invoke();
            }
            else if (currentButton == 1 && Input.GetButtonDown("A"))
            {
                PausePanel.transform.Find("ButtonPanel").Find("Restart").GetComponent<Button>().onClick.Invoke();
            }
            else if (currentButton == 2 && Input.GetButtonDown("A"))
            {
                PausePanel.transform.Find("ButtonPanel").Find("MainMenu").GetComponent<Button>().onClick.Invoke();
            }
            else if (Input.GetButtonDown("B"))
            {
                ResumeTheScene();
            }
        }
    }

    public void PauseTheScene()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
    }

    public void ResumeTheScene()
    {
        IsPaused = false;
        Time.timeScale = 1.0f;
        PausePanel.SetActive(false);
    }
    
    private void UpdateButtonsColor(int currentButton)
    {
        if (currentButton == 0)
        {
            ChangeToSelectedColor(PausePanel.transform.Find("ButtonPanel").Find("Resume").gameObject);
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("Restart").gameObject);
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("MainMenu").gameObject);
        }
        else if (currentButton == 1)
        {
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("Resume").gameObject);
            ChangeToSelectedColor(PausePanel.transform.Find("ButtonPanel").Find("Restart").gameObject);
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("MainMenu").gameObject);
        }
        else if (currentButton == 2)
        {
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("Resume").gameObject);
            ChangeToNormalColor(PausePanel.transform.Find("ButtonPanel").Find("Restart").gameObject);
            ChangeToSelectedColor(PausePanel.transform.Find("ButtonPanel").Find("MainMenu").gameObject);
        }
        else
        {
            Debug.Log("Wrong Pause Menu Button");
        }
    }

    private void ChangeToNormalColor(GameObject button)
    {
        button.GetComponent<Image>().color = button.GetComponent<Button>().colors.normalColor;
    }

    private void ChangeToSelectedColor(GameObject button)
    {
        button.GetComponent<Image>().color = button.GetComponent<Button>().colors.selectedColor;
    }
}
