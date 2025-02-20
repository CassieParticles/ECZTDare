using AK.Wwise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbStateChanger : MonoBehaviour
{
    enum ReverbTypes
    {
        Outside,
        Small,
        Medium,
        Large
    }

    [SerializeField] private ReverbTypes reverbType;
    //BoxCollider2D boxCollider;
    BoxCollider2D player;

    void Start()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();

        //Sets the "Reverb" State Group's active State to "Outside"
        AkSoundEngine.SetState("Reverb", "Outside");
        Debug.Log("Outside");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == player)
        {
            //Change reverb to reverbType

            if (reverbType == ReverbTypes.Outside)
            {
                //Sets the "Reverb" State Group's active State to "Outside"
                AkSoundEngine.SetState("Reverb", "Outside");
                Debug.Log("Outside");
            }

            if (reverbType == ReverbTypes.Small)
            {
                //Sets the "Reverb" State Group's active State to "Small"
                AkSoundEngine.SetState("Reverb", "Small");
                //AkSoundEngine.GetState("Reverb", out currentState);
                Debug.Log("Small");
            }

            if (reverbType == ReverbTypes.Medium)
            {
                //Sets the "Reverb" State Group's active State to "Medium"
                AkSoundEngine.SetState("Reverb", "Medium");
                Debug.Log("Medium");
            }

            if (reverbType == ReverbTypes.Large)
            {
                //Sets the "Reverb" State Group's active State to "Large"
                AkSoundEngine.SetState("Reverb", "Large");
                Debug.Log("Large");
            }
            
        }
    }
}
