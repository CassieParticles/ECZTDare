using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathwallRespawn : MonoBehaviour
{
    [SerializeField] private GameObject DeathWallPrefab;

    DeathWall.WallMoveData wallMoveData;
    private bool DeathWallActive;

    private static DeathwallRespawn instance;

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void DeathWallStart(DeathWall.WallMoveData deathWallData)
    {
        DeathWallActive = true;
        wallMoveData = deathWallData;
    }

    public void DeathWallStop()
    {
        DeathWallActive = false;
    }

    public void Respawn(Vector3 respawnPos) 
    {
        Debug.Log("REspawn called");
        if (!DeathWallActive)
        { return; }
        DeathWall wall = Instantiate(DeathWallPrefab).GetComponent<DeathWall>();
        wall.transform.position = respawnPos - new Vector3(30,0,0);
        wall.SetData(wallMoveData);
    }

    public void Quit()
    {
        Destroy(gameObject);
    }
}
