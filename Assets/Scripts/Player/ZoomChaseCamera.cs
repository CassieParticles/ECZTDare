using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class ChaseCameraZoom : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera;
    PolygonCollider2D track;
    CinemachineConfiner confiner;
    CinemachineBrain mainCamera;
    GameObject player;

    [Header("SETTINGS")]
    [SerializeField] private CinemachineBlendDefinition.Style blendType = CinemachineBlendDefinition.Style.HardOut;
    [SerializeField][Range(0.1f, 5f)] private float blendDuration = 1;
    [SerializeField][Range(5f, 20f)] private float zoom = 8.44f; //Perspective 79 calculations to solve ortho to fov in start
    [SerializeField][Range(10, 20)] private int priority = 12; //What priority the camera uses

    private float fov;


    void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        track = transform.parent.GetComponentInChildren<PolygonCollider2D>();
        player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera.Priority = 9;
        virtualCamera.Follow = player.transform;

        if (track != null ) {
            confiner = GetComponentInChildren<CinemachineConfiner>(); 
            confiner.m_BoundingShape2D = track;
        }

        fov = 2 * Mathf.Rad2Deg * Mathf.Atan(zoom / 10);
        virtualCamera.m_Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        virtualCamera.m_Lens.FieldOfView = fov;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
 
        if (collision == player.GetComponent<Collider2D>()) {
            virtualCamera.Priority = 11;
            mainCamera.m_DefaultBlend.m_Style = blendType;
            mainCamera.m_DefaultBlend.m_Time = blendDuration;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision == player.GetComponent<Collider2D>())
        {
            virtualCamera.Priority = 9;
            //StartCoroutine(WaitThenReset(resetTime));
        }
    }

    // Update is called once per frame
    void Update() {
        //targetGroup.m_Targets[0].weight = playerBias;
        //virtualCamera.m_Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        virtualCamera.m_Lens.FieldOfView = fov;
    }

    private void OnValidate() {
        fov = 2 * Mathf.Rad2Deg * Mathf.Atan(zoom / 10);
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.FieldOfView = fov;
        if (transform.parent != null) { //For some reason unity throws an error sometimes
            track = transform.parent.GetComponentInChildren<PolygonCollider2D>();
        }
        if (track != null) {
            confiner = GetComponentInChildren<CinemachineConfiner>();
            confiner.m_BoundingShape2D = track;
        }
    }
}
