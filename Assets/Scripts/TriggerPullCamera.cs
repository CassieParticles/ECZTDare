using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPullCamera : MonoBehaviour
{

    CinemachineVirtualCamera virtualCamera;
    CinemachineTargetGroup targetGroup;
    CinemachineBrain mainCamera;
    GameObject player;

    [SerializeField] CinemachineBlendDefinition.Style blendType = CinemachineBlendDefinition.Style.HardOut;
    [SerializeField] float blendDuration = 1;
    [SerializeField][Range(0, 20f)] float playerBias = 1; //If the camera points more towards the player or the point of interest. 0 is point of interest 20 is player
    

    

    void Start()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();
        player = GameObject.Find("Player");
        mainCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();

        virtualCamera.Priority = 9;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        virtualCamera.Priority = 11;
        mainCamera.m_DefaultBlend.m_Style = blendType;
        mainCamera.m_DefaultBlend.m_Time = blendDuration;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        virtualCamera.Priority = 9;
    }


    // Update is called once per frame
    void Update() {

        targetGroup.m_Targets[0].weight = playerBias;
    }
}
