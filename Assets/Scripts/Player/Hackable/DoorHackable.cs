using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHackable : Hackable
{
    LockableDoor door;

    private void Awake()
    {
        door = GetComponent<LockableDoor>();
    }
    public override void OnHack()
    {
        AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, hackingNoiseRadius, 15, AudioSource.Hacked);
        door.ToggleState();
    }
}
