using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallData : MonoBehaviour
{
    public DeathWall.WallMoveData wallMoveData;

    [SerializeField] private float closeDist=20;
    [SerializeField] private float mediumDist=30;

    [SerializeField] private float speedClose=20;
    [SerializeField] private float speedMedium=30;
    [SerializeField] private float speedFar=40;

    [SerializeField] private float AccCloseMedium=8;
    [SerializeField] private float AccMediumFar=12;

    [SerializeField] private bool facingRight=true;

    private void Awake()
    {
        wallMoveData.closeDist = closeDist;
        wallMoveData.mediumDist = mediumDist;
        wallMoveData.mediumDist = mediumDist;

        wallMoveData.speedClose = speedClose;
        wallMoveData.speedMedium = speedMedium;
        wallMoveData.speedFar = speedFar;

        wallMoveData.AccCloseMedium = AccCloseMedium;
        wallMoveData.AccMediumFar = AccMediumFar;

        wallMoveData.facingRight = facingRight;

        wallMoveData.yPosition = transform.position.y;
    }
}
