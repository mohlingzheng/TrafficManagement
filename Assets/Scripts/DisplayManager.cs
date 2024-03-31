using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    public InputManager inputSystem;
    public GameObject detailsPanel;
    public TextMeshProUGUI name;
    public TextMeshProUGUI objectType;
    public TextMeshProUGUI detail;
    
    void Start()
    {
        inputSystem = GameObject.FindObjectOfType<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowDetailOfSelectedObject();
    }

    void ShowDetailOfSelectedObject()
    {
        GameObject gameObject;
        if (inputSystem.selection)
        {
            gameObject = inputSystem.selection.gameObject;
        }
        else
        {
            gameObject = inputSystem.pointedGameObject;
        }

        if (gameObject == null)
        {
            name.text = "";
            objectType.text = "";
            detailsPanel.SetActive(false);
            return;
        }
        detailsPanel.SetActive(true);
        name.text = gameObject.name;
        objectType.text = gameObject.tag;
    }
}
