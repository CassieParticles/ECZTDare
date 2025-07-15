using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Unity.VisualScripting;
using UnityEngine;


[Serializable] struct CameraSettings {
    public Vector2 position;
    public CinemachineBlendDefinition.Style blendType;
    [Range(0.1f, 5f)] public float blendDuration;
    [Range(0.01f, 10f)] public float waitDuration;
    [Range(1f, 20f)] public float zoom;
    public bool moveForWaitDuration;
    public Vector2 finalPosition;
}

public class StealthCamera : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera1;
    CinemachineVirtualCamera virtualCamera2;
    CinemachineBrain mainCamera;
    GameObject player;


    [Header("Defaults:\nBlend Type:\t\tHard Out\nBlend Duration:\t1\nWait Duration:\t\t1\nZoom:\t\t\t8.44")]
    [SerializeField] private List<CameraSettings> cameras = new List<CameraSettings>();

    private float fov;


    void Start()
    {
        virtualCamera1 = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        virtualCamera2 = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();

        player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera1.m_Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        virtualCamera1.m_Lens.FieldOfView = fov;
        virtualCamera2.m_Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        virtualCamera2.m_Lens.FieldOfView = fov;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
 
        if (collision == player.GetComponent<Collider2D>()) {
            //virtualCamera.Priority = 11;
            //mainCamera.m_DefaultBlend.m_Style = blendType;
            //mainCamera.m_DefaultBlend.m_Time = blendDuration;
            StartCoroutine(CameraPreviewCutscene());
        }
    }

    private IEnumerator CameraPreviewCutscene() {
        CinemachineVirtualCamera currentCamera = virtualCamera1;
        CinemachineVirtualCamera targetCamera = virtualCamera2;

        foreach(var cam in cameras) {
            //Set up virtual camera for transition
            targetCamera.transform.localPosition = (Vector3)cam.position + Vector3.back * 10;
            fov = 2 * Mathf.Rad2Deg * Mathf.Atan(cam.zoom / 10);
            targetCamera.m_Lens.FieldOfView = fov;

            //Set settings for blend
            mainCamera.m_DefaultBlend.m_Style = cam.blendType;
            mainCamera.m_DefaultBlend.m_Time = cam.blendDuration;

            //Setting priorities starts the transition
            targetCamera.Priority = 15;
            currentCamera.Priority = 14;

            //Wait the time it takes to transition
            yield return new WaitForSeconds(cam.blendDuration);

            //Switch the cameras around
            CinemachineVirtualCamera temp = currentCamera;
            currentCamera = targetCamera;
            targetCamera = temp;

            //Either wait or move
            if (cam.moveForWaitDuration) { //Move the camera for the duration
                //Setup variables
                Vector3 pos = currentCamera.transform.localPosition;
                Vector3 finalPos = (Vector3)cam.finalPosition;
                Vector3 moveVector = finalPos - (Vector3)cam.position;

                float timer = 0;
                while (timer < cam.waitDuration) {
                    float speed = (moveVector.magnitude / cam.waitDuration) * Time.deltaTime;
                    currentCamera.transform.localPosition += moveVector.normalized * speed;

                    timer += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

            } else {
                //Wait the duration before transitioning into the next camera
                yield return new WaitForSeconds(cam.waitDuration);
            }
            
        }

        //Reset as the camera cutscene is over
        targetCamera.Priority = 9;
        currentCamera.Priority = 9;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    // Update is called once per frame
    private void Update() {

    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        for(int i = 0; i < cameras.Count; i++) {
            Vector2 pos = cameras[i].position + (Vector2)transform.position;
            Gizmos.DrawWireSphere(pos, 0.3f);

            Vector2 startOffset = Vector2.one * 0.5f;
            Vector2 height = Vector2.up * 0.5f;

            for (int j = 0; j < i + 1; j++) {
                Vector2 countOffset = Vector2.right * j * 0.2f;
                Gizmos.DrawLine(pos + startOffset + height + countOffset, pos + startOffset + countOffset);
            }

            if (cameras[i].moveForWaitDuration) {
                Vector2 finalPos = cameras[i].finalPosition + (Vector2)transform.position;
                Gizmos.DrawLine(pos, finalPos);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(finalPos, 0.2f);
                Gizmos.color = Color.yellow;
            }
        }
    }
}
