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
        TextMeshProUGUI[] textMeshProUGUIs = detailsPanel.GetComponentsInChildren<TextMeshProUGUI>();
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
            GetSpecificText(textMeshProUGUIs, "Name").text = "";
            GetSpecificText(textMeshProUGUIs, "ObjectType").text = "";
            GetSpecificText(textMeshProUGUIs, "Attribute").text = "";
            detailsPanel.SetActive(false);
            return;
        }
        detailsPanel.SetActive(true);
        GetSpecificText(textMeshProUGUIs, "Name").text = gameObject.name;
        GetSpecificText(textMeshProUGUIs, "ObjectType").text = gameObject.tag;
        if (gameObject.CompareTag("Vehicle"))
        {
            GetSpecificText(textMeshProUGUIs, "Attribute").text = gameObject.GetComponent<VehicleMovement>().currentSpeed.ToString();
        }
        else
        {
            GetSpecificText(textMeshProUGUIs, "Attribute").text = "";
        }
    }

    private TextMeshProUGUI GetSpecificText(TextMeshProUGUI[] textMeshProUGUIs, string text)
    {
        foreach (TextMeshProUGUI textMeshProUGUI in textMeshProUGUIs)
        {
            if (textMeshProUGUI.name == text)
                return textMeshProUGUI;
        }
        return null;
    }

}
