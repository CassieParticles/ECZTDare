using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    public Slider HackingSlider;
    public Slider BoostSlider;
    public MovementScript movementScript;
    public HackingScript hackingScript;

    public Sprite cloakBar;

    private GameObject cloaktext;

    // Update is called once per frame
    private void Start()
    {
        //fetch movement script
        movementScript = GameObject.Find("Player").GetComponent<MovementScript>();
        hackingScript = GameObject.Find("Player").GetComponent<HackingScript>();

        cloaktext = BoostSlider.transform.GetChild(3).gameObject;
    }

    void FixedUpdate()
    {
        BoostSlider.value = movementScript.batteryCharge;
        HackingSlider.value = hackingScript.hackCharge;
    }

    public void SetCloakBar() {
        BoostSlider.transform.GetChild(1).GetComponent<Image>().sprite = cloakBar;
        cloaktext.SetActive(true);
    }
}
