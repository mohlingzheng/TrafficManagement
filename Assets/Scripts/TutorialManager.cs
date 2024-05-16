using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject TutorialPanel;
    public GameObject InputSystem;
    public GameObject DisplaySystem;
    public GameObject Pointer;
    public CameraController CameraController;
    public VehicleGeneration VehicleGeneration;
    public KomtarSceneManager KomtarSceneManager;
    public TimeTrackingManager TimeTrackingManager;
    public int currentStep;
    public int TotalSteps;


    // 0:  Whole Screen

    // 1:  Up Down Left Right
    // 2:  Rotate and Zoom In
    // 3:  Reset Rotate and Zoom

    // 4:  Utilities Panel
    // 5:  How to Switch panel
    // 6:  Select
    // 7:  Build Road
    // 8:  Remove Road

    // 9:  Toggle Congestion
    // 10: Pause Button
    // 11: Have fun

    // *:  Skip tutorial

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = TimeScale.stop;
        KomtarSceneManager.IsPaused = true;
        currentStep = 0;
        TutorialPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        DisplayAccordingToStep();
        ButtonInteractionAccordingToStep();
    }

    private void DisplayAccordingToStep()
    {
        if (CheckStep(0))
        {
            GameObject step = TutorialPanel.transform.Find("Intro").gameObject;
            step.SetActive(true);
        }
        if (CheckStep(1))
        {
            TutorialPanel.transform.Find("Intro").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step1").gameObject.SetActive(true);
        }
        else if (CheckStep(2))
        {
            TutorialPanel.transform.Find("Step1").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step2").gameObject.SetActive(true);
        }
        else if (CheckStep(3))
        {
            TutorialPanel.transform.Find("Step2").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step3").gameObject.SetActive(true);
        }
        else if (CheckStep(4))
        {
            TutorialPanel.transform.Find("Step3").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step4").gameObject.SetActive(true);
        }
        else if (CheckStep(5))
        {
            TutorialPanel.transform.Find("Step4").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step5").gameObject.SetActive(true);
        }
        else if (CheckStep(6))
        {
            TutorialPanel.transform.Find("Step5").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step6").gameObject.SetActive(true);
        }
        else if (CheckStep(7))
        {
            TutorialPanel.transform.Find("Step6").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step7").gameObject.SetActive(true);
        }
        else if (CheckStep(8))
        {
            TutorialPanel.transform.Find("Step7").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step8").gameObject.SetActive(true);
        }
        else if (CheckStep(9))
        {
            TutorialPanel.transform.Find("Step8").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step9").gameObject.SetActive(true);
        }
        else if (CheckStep(10))
        {
            TutorialPanel.transform.Find("Step9").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step10").gameObject.SetActive(true);
        }
        else if (CheckStep(11))
        {
            TutorialPanel.transform.Find("Step10").gameObject.SetActive(false);
            TutorialPanel.transform.Find("Step11").gameObject.SetActive(true);
        }
        else if (CheckStep(12))
        {
            TutorialPanel.transform.Find("Step11").gameObject.SetActive(false);
            //TutorialPanel.transform.Find("Step11").gameObject.SetActive(true);
        }
    }

    private void ButtonInteractionAccordingToStep()
    {
        if (CheckStep(0))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
            else if (Input.GetButtonDown("B"))
            {
                currentStep = 11;
            }
        }
        else if (CheckStep(1))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(2))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(3))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(4))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(5))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(6))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(7))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(8))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(9))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(10))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(11))
        {
            if (Input.GetButtonDown("A"))
            {
                currentStep++;
            }
        }
        else if (CheckStep(12))
        {
            TutorialPanel.SetActive(false);
            EnableNormalSimulation();
            this.enabled = false;
        }
    }

    private void EnableNormalSimulation()
    {
        Pointer.SetActive(true);
        CameraController.enabled = true;
        DisplaySystem.SetActive(true);
        InputSystem.SetActive(true);
        VehicleGeneration.enabled = true;
        KomtarSceneManager.enabled = true;
        TimeTrackingManager.enabled = true;
        Time.timeScale = TimeScale.normal;
        KomtarSceneManager.IsPaused = false;
    }

    private bool CheckStep(int step)
    {
        return (currentStep == step);
    }
}
