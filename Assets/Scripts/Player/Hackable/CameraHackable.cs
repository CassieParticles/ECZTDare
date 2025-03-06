using System.Collections;
using UnityEngine;

public class CameraHackable : Hackable
{
    [SerializeField] float hackTimer = 5;

    CameraBehaviour cameraAttached;

    private IEnumerator HackCamera()
    {
        cameraAttached.beingHacked = true;
        yield return new WaitForSeconds(hackTimer);
        cameraAttached.beingHacked = false;
    }
    private void Awake()
    {
        cameraAttached = GetComponent<CameraBehaviour>();
    }


    public override void OnHack()
    {
        StartCoroutine(HackCamera());
    }
}
