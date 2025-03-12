using System.Collections;
using UnityEngine;

public class CameraHackable : Hackable
{
    public AK.Wwise.Event Hack_Start;
    public AK.Wwise.Event Hack_Stop;

    [SerializeField] float hackTimer = 5;
    [SerializeField] float hackDetectionRadius = 8;

    CameraBehaviour cameraAttached;

    private IEnumerator HackCamera()
    {
        float dist = cameraAttached.visionCone.distance;    //Store original range
        //Disable camera
        cameraAttached.beingHacked = true;
        cameraAttached.visionCone.distance = 0;
        beingHacked = true;
        Hack_Start.Post(gameObject);

        yield return new WaitForSeconds(hackTimer);
        //Enable camera
        cameraAttached.beingHacked = false;
        cameraAttached.visionCone.distance = dist;
        beingHacked = false;
        Hack_Stop.Post(gameObject);

    }
    private void Awake()
    {
        cameraAttached = GetComponent<CameraBehaviour>();
    }


    public override void OnHack()
    {
        AudioDetectionSystem.getAudioSystem().PlaySound(transform.position, hackDetectionRadius, 15, AudioSource.Hacked);
        StartCoroutine(HackCamera());
    }
}
