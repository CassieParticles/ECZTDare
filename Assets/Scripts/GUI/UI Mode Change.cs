using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModeChange : MonoBehaviour
{

    
     private GameObject stealthDisplay;
     private GameObject movementDisplay;
     private GameObject cloakBar;
     private GameObject boostBar;
     public bool hasAbilities = false;
    // Start is called before the first frame update

    private void Start()
    {
        stealthDisplay = GameObject.Find("StealthDisplay");
        movementDisplay = GameObject.Find("MovementDisplay");
        cloakBar = GameObject.Find("CloakBar");
        boostBar = GameObject.Find("BoostBar");
        cloakBar.SetActive(false);
        boostBar.SetActive(false);
        hasAbilities = false;
    }
    public void stealthMode()
    {
        stealthDisplay.SetActive(true);
        movementDisplay.SetActive(false);
        if (hasAbilities)
        {
            cloakBar.SetActive(true);
            boostBar.SetActive(false);
        }
    }
    public void movementMode()
    {
        stealthDisplay.SetActive(false);
        movementDisplay.SetActive(true);

        if (hasAbilities)
        {
            cloakBar.SetActive(false);
            boostBar.SetActive(true);
        }
    }

    public void collectUpgrade()
    {
            cloakBar.SetActive(true);
            boostBar.SetActive(false);
            hasAbilities = true;
    }
}
