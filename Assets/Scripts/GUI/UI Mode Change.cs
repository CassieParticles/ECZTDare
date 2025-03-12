using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModeChange : MonoBehaviour
{

    
    [SerializeField] private GameObject stealthDisplay;
    [SerializeField] private GameObject movementDisplay;
    // Start is called before the first frame update


    // Update is called once per frame
    public void stealthMode()
    {
        stealthDisplay.SetActive(true);
        movementDisplay.SetActive(false);
    }
    public void movementMode()
    {
        stealthDisplay.SetActive(false);
        movementDisplay.SetActive(true);
    }
}
