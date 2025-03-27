using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    TextMeshPro jumpText;
    TextMeshPro slideText;
    TextMeshPro hackText;

    private void Awake() {

    }

    public void Refresh(string jump, string slide, string hack) {
        jumpText = transform.Find("Jump").GetComponent<TextMeshPro>();
        slideText = transform.Find("Slide").GetComponent<TextMeshPro>();
        hackText = transform.Find("Hack").GetComponent<TextMeshPro>();

        jumpText.text = "Press " + jump + " to Jump";
        slideText.text = "Press " + slide + " when at speed to Slide";
        hackText.text = "Aim with Mouse \nand hit " + hack + " to Hack";
    }
}
