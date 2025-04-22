using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPullCamera : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera;
    CinemachineTargetGroup targetGroup;
    BoxCollider2D trigger;
    CinemachineBrain mainCamera;
    GameObject player;
    Rigidbody2D playerRB;

    [SerializeField] private CinemachineBlendDefinition.Style blendType = CinemachineBlendDefinition.Style.HardOut;
    [SerializeField][Range(0.1f, 5f)] private float blendDuration = 1;
    [SerializeField][Range(0.1f, 5f)] private float resetTime = 0.51f;
    [SerializeField][Range(0f, 20f)] private float playerBias = 1; //If the camera points more towards the player or the point of interest. 0 is point of interest 20 is player
    [SerializeField][Range(5f, 20f)] private float zoom = 8.44f;
    [SerializeField] private bool followX = false;
    [SerializeField] private bool followY = false;

    private Vector2 origin;
    private Vector2 offset;



    void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();
        trigger = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera.Priority = 9;
        origin = transform.position;
        offset = trigger.offset;
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
        targetGroup.m_Targets[0].weight = playerBias;
        virtualCamera.m_Lens.OrthographicSize = zoom;

        if (followX) {
            transform.position = new Vector3(playerRB.position.x, transform.position.y, transform.position.z);
            trigger.offset = new Vector2(offset.x + (origin.x - playerRB.position.x), trigger.offset.y);
        }
        if (followY) {
            transform.position = new Vector3(transform.position.x, playerRB.position.y, transform.position.z);
            trigger.offset = new Vector2(trigger.offset.x, offset.y + (origin.y - playerRB.position.y));
        }
    }
}
