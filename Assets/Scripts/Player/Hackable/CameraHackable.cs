using System.Collections;
using UnityEngine;

public class CameraHackable : Hackable
{
    [SerializeField] float hackTimer = 5;

    CameraBehaviour cameraAttached;

    private IEnumerator HackCamera()
    {
        float dist = cameraAttached.visionCone.distance;    //Store original range
        //Disable camera
        cameraAttached.beingHacked = true;
        cameraAttached.visionCone.distance = 0;
        beingHacked = true;

        yield return new WaitForSeconds(hackTimer);
        //Enable camera
        cameraAttached.beingHacked = false;
        cameraAttached.visionCone.distance = dist;
        beingHacked = false;
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
