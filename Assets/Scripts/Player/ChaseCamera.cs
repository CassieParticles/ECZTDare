using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera;
    CinemachineTargetGroup targetGroup;
    BoxCollider2D trigger;
    PolygonCollider2D track;
    CinemachineBrain mainCamera;
    GameObject player;
    Rigidbody2D playerRB;

    [SerializeField] private CinemachineBlendDefinition.Style blendType = CinemachineBlendDefinition.Style.HardOut;
    [SerializeField][Range(0.1f, 5f)] private float blendDuration = 1;
    [SerializeField][Range(0.1f, 5f)] private float resetTime = 0.51f;
    [SerializeField][Range(0f, 20f)] private float playerBias = 1; //If the camera points more towards the player or the point of interest. 0 is point of interest 20 is player
    [SerializeField][Range(5f, 20f)] private float zoom = 8.44f; //Perspective 79 calculations to solve ortho to fov in start
    [SerializeField] private List<Vector2> anchorPoints = new List<Vector2> { Vector2.zero, Vector2.right };

    private Vector2 origin;
    private Vector2 offset;
    private float fov;


    void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        trigger = GetComponent<BoxCollider2D>();
        track = GetComponentInChildren<PolygonCollider2D>();
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera.Priority = 9;
        origin = transform.position;
        offset = trigger.offset;

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
            StartCoroutine(WaitThenReset(resetTime));
        }
    }

    IEnumerator WaitThenReset(float seconds) {
        yield return new WaitForSeconds(seconds);
        if (virtualCamera.Priority == 9 ) {
            transform.position = origin;
            trigger.offset = offset;
        }
    }

    // Update is called once per frame
    void Update() {
        //targetGroup.m_Targets[0].weight = playerBias;
        //virtualCamera.m_Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        virtualCamera.m_Lens.FieldOfView = fov;
    }

    private void OnValidate() {
        track = GetComponentInChildren<PolygonCollider2D>();
        List<Vector2> newTrack = new List<Vector2>();
        newTrack.Add(anchorPoints[0]);
        foreach (Vector2 point in anchorPoints) {
            newTrack.Add(point);
        }

        for (int i = 0; i < anchorPoints.Count - 1; i++) {
            newTrack.Add(anchorPoints[anchorPoints.Count - i - 1]);
        }

        track.points = newTrack.ToArray();
    }
}
