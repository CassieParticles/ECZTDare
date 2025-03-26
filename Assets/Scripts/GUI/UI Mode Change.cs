using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModeChange : MonoBehaviour
{

    private MovementScript player;
    private GameObject stealthDisplay;
    private GameObject movementDisplay;
    private GameObject cloakBar;
    private GameObject boostBar;
    // Start is called before the first frame update


    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<MovementScript>();
        stealthDisplay = GameObject.Find("StealthDisplay");
        movementDisplay = GameObject.Find("MovementDisplay");
        cloakBar = GameObject.Find("CloakBar");
        boostBar = GameObject.Find("BoostBar");
        cloakBar.SetActive(false);
        boostBar.SetActive(false);
        
    }
    public void stealthMode()
    {
        stealthDisplay.SetActive(true);
        movementDisplay.SetActive(false);
        if (player.boostCloakUnlocked)
        {
            cloakBar.SetActive(true);
            boostBar.SetActive(false);
        }
    }
    public void movementMode()
    {
        stealthDisplay.SetActive(false);
        movementDisplay.SetActive(true);

        if (player.boostCloakUnlocked)
        {
            cloakBar.SetActive(false);
            boostBar.SetActive(true);
        }
    }

    public void collectUpgrade()
    {
        cloakBar.SetActive(true);
        boostBar.SetActive(false);
        player.batteryCharge = 100;
        player.boostCloakUnlocked = true;
    }
}
