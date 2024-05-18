using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DisplayManager : MonoBehaviour
{
    public InputManager inputSystem;
    public GameObject canvas;
    public GameObject detailsPanel;
    
    void Start()
    {
        inputSystem = FindObjectOfType<InputManager>();
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

        if (gameObject == null || Tag.CompareTags(gameObject.transform, Tag.Untagged))
        {
            detailsPanel.SetActive(false);
            return;
        }
        detailsPanel.SetActive(true);
        GetTextMeshProUI(detailsPanel, "Name").text = gameObject.name;
        GameObject lastEnabledAttribute;

        if (Tag.CompareTags(gameObject.transform, Tag.Vehicle))
        {
            GetTextMeshProUI(detailsPanel, "ObjectType").text = gameObject.tag;
            if (gameObject.GetComponent<VehicleMovement>().vehicleType == VehicleType.Light)
                detailsPanel.transform.Find("IconName").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/3d-car");
            else
                detailsPanel.transform.Find("IconName").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/3d-truck");
            GetTextMeshProUI(detailsPanel, "Attribute1").text = "Destination: " + gameObject.GetComponent<VehicleMovement>().goalObject.name;
            GetTextMeshProUI(detailsPanel, "Attribute2").text = "Desired Speed: " + gameObject.GetComponent<VehicleMovement>().desiredSpeed.ToString("F2");
            GetTextMeshProUI(detailsPanel, "Attribute3").text = "Current Speed: " + gameObject.GetComponent<VehicleMovement>().currentSpeed.ToString("F2");
            GetTextMeshProUI(detailsPanel, "Attribute4").text = "Waited Time: " + gameObject.GetComponent<VehicleMovement>().timeWaited.ToString("F2");
            lastEnabledAttribute = detailsPanel.transform.Find("Attributes").Find("Attribute4").gameObject;
            GetTextMeshProUI(detailsPanel, "Attribute1").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute2").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute3").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute4").enabled = true;
        }
        else if (Tag.CompareTags(gameObject.transform, Tag.Goal))
        {
            GetTextMeshProUI(detailsPanel, "ObjectType").text = "Building";
            detailsPanel.transform.Find("IconName").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/3d-house");
            lastEnabledAttribute = detailsPanel.transform.Find("IconName").gameObject;
            GetTextMeshProUI(detailsPanel, "Attribute1").enabled = false;
            GetTextMeshProUI(detailsPanel, "Attribute2").enabled = false;
            GetTextMeshProUI(detailsPanel, "Attribute3").enabled = false;
            GetTextMeshProUI(detailsPanel, "Attribute4").enabled = false;
        }
        else if (Tag.CompareTags(gameObject.transform, Tag.Road_Large, Tag.Road_Small))
        {
            GetTextMeshProUI(detailsPanel, "ObjectType").text = "Road";
            detailsPanel.transform.Find("IconName").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/3d-road");
            GetTextMeshProUI(detailsPanel, "Attribute1").text = "Road Length: " + gameObject.GetComponent<RoadTrafficDensity>().RoadLength.ToString("F2");
            GetTextMeshProUI(detailsPanel, "Attribute2").text = "No. Of Vehicles: " + gameObject.GetComponent<RoadTrafficDensity>().NumberOfCars.ToString("F0");
            GetTextMeshProUI(detailsPanel, "Attribute3").text = "Traffic Density: " + gameObject.GetComponent<RoadTrafficDensity>().TrafficDensity.ToString("F2");
            GetTextMeshProUI(detailsPanel, "Attribute4").text = "Weightage: " + gameObject.GetComponent<RoadTrafficDensity>().WeightValue.ToString("F2");
            lastEnabledAttribute = detailsPanel.transform.Find("Attributes").Find("Attribute4").gameObject;
            GetTextMeshProUI(detailsPanel, "Attribute1").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute2").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute3").enabled = true;
            GetTextMeshProUI(detailsPanel, "Attribute4").enabled = true;
        }
        else
        {
            lastEnabledAttribute = detailsPanel.transform.Find("Attributes").Find("Attribute1").gameObject;
        }

        float newHeight =
               detailsPanel.transform.Find("IconName").GetComponent<RectTransform>().sizeDelta.y +
               detailsPanel.transform.Find("IconName").GetComponent<RectTransform>().localPosition.y * -1;
        if (lastEnabledAttribute.name != "IconName")
        {
            newHeight = newHeight +
               lastEnabledAttribute.GetComponent<RectTransform>().sizeDelta.y +
               lastEnabledAttribute.GetComponent<RectTransform>().localPosition.y * -1 + 20f;
        }
        else
        {
            newHeight -= 40f;
        }
        detailsPanel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(detailsPanel.transform.GetComponent<RectTransform>().sizeDelta.x, Mathf.Abs(newHeight) + 5f);
    }

    private TextMeshProUGUI GetTextMeshProUI(GameObject gameObject, string text)
    {
        Transform transform = gameObject.transform.Find(text);
        TextMeshProUGUI textMeshProUGUI;
        if (transform != null)
        {
            textMeshProUGUI = transform.GetComponent<TextMeshProUGUI>();
            return textMeshProUGUI;
        }
        else
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                transform = gameObject.transform.GetChild(i).Find(text);
                if (transform != null)
                {
                    textMeshProUGUI = transform.GetComponent<TextMeshProUGUI>();
                    return textMeshProUGUI;
                }
            }
            return null;
        }
    }

    public void ShowLoadingPanel()
    {
        canvas.transform.Find("LoadingPanel").gameObject.SetActive(true);
        //Debug.Log("Showing");
        //Time.timeScale = 0f;
    }

    public void HideLoadingPanel()
    {
        canvas.transform.Find("LoadingPanel").gameObject.SetActive(false);
        //Debug.Log("Showing un");
        //Time.timeScale = 1.0f;
    }

}
