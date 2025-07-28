using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateChaseDisplayText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void UpdateDistance(int distance)
    {
        if (text == null){ return; }
        string distString = distance.ToString();

        text.text = "<- "+distString + "m";
    }
}
