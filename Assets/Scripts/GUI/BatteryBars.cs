using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour
{
    public Slider HackingSlider;
    public Slider BoostSlider;
    public Slider CloakSlider;
    public MovementScript movementScript;
    public HackingScript hackingScript;
    // [SerializeField] private Image sliderFill;
    //[SerializeField] Color32 fullCharge;
    //[SerializeField] Color32 twoThirdCharge;
    //[SerializeField] Color32 oneThirdCharge;

    // Update is called once per frame
    private void Start()
    {
        //fetch movement script
        movementScript = GameObject.Find("Player").GetComponent<MovementScript>();
        hackingScript = GameObject.Find("Player").GetComponent<HackingScript>();
        
    }

    void FixedUpdate()
    {
        //changes the size of the bar depending on how full it is.
        if (movementScript.boostCloakUnlocked && movementScript.inStealthMode) {
            CloakSlider.value = movementScript.batteryCharge;
        } else if (movementScript.boostCloakUnlocked) {
            BoostSlider.value = movementScript.batteryCharge;
        }
        HackingSlider.value = hackingScript.hackCharge;
        //changes the colour of the bar depending on how full it is
      //  if (movementScript.boostCharge < 66 && movementScript.boostCharge > 33)
      //  {
      //      sliderFill.color = twoThirdCharge;
      //  }
      //  if (movementScript.boostCharge < 66 && movementScript.boostCharge > 33)
      //  {
      //      sliderFill.color = oneThirdCharge;
      //  }
      //  else
      //  {
      //      sliderFill.color = fullCharge;
      //  }
    }
}
