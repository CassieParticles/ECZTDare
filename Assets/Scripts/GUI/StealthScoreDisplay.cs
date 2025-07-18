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

        string niceScore = string.Format("{0} {1}", thousand, remainder);
        stealthScoreText.text = niceScore;
    }
}
