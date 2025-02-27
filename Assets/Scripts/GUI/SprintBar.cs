using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprintBar : MonoBehaviour
{
    public Slider slider;
    public MovementScript movementScript;


    // Update is called once per frame
    private void Start()
    {
        //fetch movement script
        movementScript = GameObject.Find("Player").GetComponent<MovementScript>();

    }

    void FixedUpdate()
    {
        //
 
        slider.value = movementScript.boostCharge;
    }
}
