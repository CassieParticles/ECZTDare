using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialTextUpdate : MonoBehaviour
{
    [SerializeField] string text;
    [SerializeField] ControlsScript.Controls[] controls;
    string[] splitString;

    StringBuilder stringBuilder;
    TextMeshPro textGUI;

    public void RefreshText()
    {
        //Put the first part of the string in
        stringBuilder.Clear().Append(splitString[0]).Append(' ');

        //Iterate through the rest of the string, adding all the controls in
        ControlsScript controlsScript = FindAnyObjectByType<ControlsScript>();
        if (!controlsScript){ return; }
        for(int i=0;i<controls.Length;++i)
        {
            stringBuilder.Append(controlsScript.GetBoundControl(controls[i])).Append(' ');
            stringBuilder.Append(splitString[i + 1]).Append(' ');
        }

        textGUI.text = stringBuilder.ToString();
    }

    private void Awake()
    {
        textGUI = GetComponent<TextMeshPro>();
        stringBuilder = new StringBuilder(text);
        splitString = text.Split('_');
        if(splitString.Length != controls.Length + 1 )
        {
            Debug.LogError("ERROR: CONTROLS AND STRING GAP MISMATCH");
            return;
        }
    }

    private void Start()
    {
        RefreshText();
    }
}
