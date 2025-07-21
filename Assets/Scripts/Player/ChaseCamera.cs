using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera;
    PolygonCollider2D track;
    CinemachineBrain mainCamera;
    GameObject player;

    [Header("SETTINGS")]
    [SerializeField] private CinemachineBlendDefinition.Style blendType = CinemachineBlendDefinition.Style.HardOut;
    [SerializeField][Range(0.1f, 5f)] private float blendDuration = 1;
    [SerializeField][Range(5f, 20f)] private float zoom = 8.44f; //Perspective 79 calculations to solve ortho to fov in start
    [SerializeField][Range(10, 20)] private int priority = 11; //What priority the camera uses
    [Header("ANCHOR POINTS\nTwo anchors is enough for a straight line\nPosition is relative to the gameobject")]
    [SerializeField] private List<Vector2> anchorPoints = new List<Vector2> { Vector2.zero, Vector2.right };

    private float fov;


    void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        track = GetComponentInChildren<PolygonCollider2D>();
        player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera.Priority = 9;
        virtualCamera.Follow = player.transform;

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
        track = GetComponentInChildren<PolygonCollider2D>();
        List<Vector2> newTrack = new List<Vector2> { anchorPoints[0] };
        foreach (Vector2 point in anchorPoints) {
            newTrack.Add(point);
        }

        for (int i = 0; i < anchorPoints.Count - 1; i++) {
            newTrack.Add(anchorPoints[anchorPoints.Count - i - 1]);
        }

        track.points = newTrack.ToArray();
    }
}
