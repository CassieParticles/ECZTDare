using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionSwitcher : MonoBehaviour
{
    GameObject player;
    SectionUIChange uiSwitcher;
    public SectionUIChange.UITypes type;

    private void Awake() {
        player = GameObject.Find("Player");
        uiSwitcher = GameObject.Find("SectionUIChanger").GetComponent<SectionUIChange>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision == player.GetComponent<Collider2D>()) {
            uiSwitcher.SwitchUIType(type);
        }
    }
}
