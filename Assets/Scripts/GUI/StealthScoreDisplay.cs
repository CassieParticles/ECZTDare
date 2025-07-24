using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StealthScoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stealthScoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void UpdateGUI(int stealthScore) {

        int thousand = Mathf.FloorToInt(stealthScore / 1000);
        int remainder = Mathf.FloorToInt(stealthScore % 1000);
        int hundred = Mathf.FloorToInt(remainder / 100);
        remainder = Mathf.FloorToInt(hundred % 100);
        int ten = Mathf.FloorToInt(remainder / 10);
        remainder = Mathf.FloorToInt(ten % 10);
        string niceScore = string.Format("{0} {1}{2}{3}", thousand, hundred, ten, remainder);
        stealthScoreText.text = niceScore;
    }
}
