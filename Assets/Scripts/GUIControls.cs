using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIControls : MonoBehaviour
{
    [SerializeField]
#pragma warning disable
    MaterialCreator materialCreator;
    [SerializeField]
#pragma warning disable
    Text TimeGUIText;

    private void Start()
    {
        UpdateGUIText();
    }

    // Add or subtract the given time from current time
    public void AddTime(int time)
    {
        materialCreator.percentTimePoint += time;
        materialCreator.LoadTextureAtPoint(materialCreator.percentTimePoint);
        UpdateGUIText();
    }

    // Set current time to given time
    public void ChangeTime()
    {
        Int32.TryParse(TimeGUIText.text, out materialCreator.percentTimePoint);
        materialCreator.LoadTextureAtPoint(materialCreator.percentTimePoint);
        UpdateGUIText();
    }

    // Update GUI text
    void UpdateGUIText()
    {
        TimeGUIText.text = materialCreator.PercentTimePoint.ToString();
    }
}
