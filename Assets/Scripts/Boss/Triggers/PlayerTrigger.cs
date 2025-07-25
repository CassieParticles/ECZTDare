using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : BaseTrigger
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementScript>())
        {
            BackwallDetectability backwall = FindAnyObjectByType<BackwallDetectability>();
            if(!backwall || backwall.playerVisible)
            {
                if (GetComponent<SpotlightAI>().IsDistractible())
                {
                    reciever.RecieveSignal(anchor);
                }
            }

        }
    }
}
