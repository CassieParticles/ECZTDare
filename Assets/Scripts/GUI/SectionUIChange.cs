using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionUIChange : MonoBehaviour
{
    GameObject chaseUI;
    GameObject stealthUI;
    GameObject breakroomUI;
    BreakroomDisplay breakroomScoreDisplay;

    public enum UITypes {
        chase,
        stealth,
        breakroom
    }

    UITypes currentType;

    private void Awake() {
        chaseUI = transform.GetChild(0).gameObject;
        stealthUI = transform.GetChild(1).gameObject;
        breakroomUI = transform.GetChild(2).gameObject;
        breakroomScoreDisplay = breakroomUI.GetComponentInChildren<BreakroomDisplay>();

        chaseUI.SetActive(false);
        stealthUI.SetActive(false);
        breakroomUI.SetActive(false);
    }
    public void SwitchUIType(UITypes type) {
        //if (type == currentType) {
        //    return;
        //}
        currentType = type;
        
        switch(currentType) {
            case UITypes.chase:
                chaseUI.SetActive(true);
                stealthUI.SetActive(false);
                breakroomUI.SetActive(false);
                break;
            case UITypes.stealth:
                chaseUI.SetActive(false);
                stealthUI.SetActive(true);
                breakroomUI.SetActive(false);
                break;
            case UITypes.breakroom:
                chaseUI.SetActive(false);
                stealthUI.SetActive(false);
                breakroomUI.SetActive(true);
                break;
        }
    }

    public void BreakRoomDisplayScore(List<ScoreData> scores) {
        breakroomScoreDisplay.AddScore(scores);
    }
}
