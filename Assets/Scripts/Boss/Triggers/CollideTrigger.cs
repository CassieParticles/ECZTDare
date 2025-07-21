using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideTrigger : BaseTrigger
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            reciever.RecieveSignal(anchor);
        }
    }
}
