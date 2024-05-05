using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionOutline : MonoBehaviour
{
    GameObject previousSelected = null;
    void Start()
    {
        previousSelected = transform.Find("Komtar").gameObject;
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != previousSelected)
        {
            previousSelected.GetComponent<UnityEngine.UI.Outline>().enabled = false;
            previousSelected = selected;
            previousSelected.GetComponent<UnityEngine.UI.Outline>().enabled = true;
        }
    }
}
