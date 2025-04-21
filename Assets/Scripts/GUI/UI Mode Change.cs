using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModeChange : MonoBehaviour
{

    public MovementScript player;
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
        if (player.inStealthMode) {
            StealthMode();
        } else {
            MovementMode();
        }
        
    }
    public void StealthMode()
    {
        stealthDisplay.SetActive(true);
        movementDisplay.SetActive(false);
        if (player.boostCloakUnlocked)
        {
            cloakBar.SetActive(true);
            boostBar.SetActive(false);
        }
    }
    public void MovementMode()
    {
        stealthDisplay.SetActive(false);
        movementDisplay.SetActive(true);

        if (player.boostCloakUnlocked)
        {
            cloakBar.SetActive(false);
            boostBar.SetActive(true);
        }
    }

    public void CollectUpgrade()
    {
        if (player == null) {
            Start();
        }
        player.batteryCharge = 100;
        player.boostCloakUnlocked = true;
        if (player.inStealthMode) {
            boostBar.SetActive(false);
            cloakBar.SetActive(true);
        } else {
            cloakBar.SetActive(false);
            boostBar.SetActive(true);
        }
    }
}
