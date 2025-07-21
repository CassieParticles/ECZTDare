using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathwallRespawn : MonoBehaviour
{
    [SerializeField] private GameObject DeathWallPrefab;

    DeathWall.WallMoveData wallMoveData;
    private bool DeathWallActive;

    private static DeathwallRespawn instance;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        instance = null;
    }

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
        }
        instance = this;
        FindAnyObjectByType<LevelManager>().AddCallback(ChangeLevel);
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
        if (!DeathWallActive)
        { return; }
        DeathWall wall = Instantiate(DeathWallPrefab).GetComponent<DeathWall>();
        wall.transform.position = respawnPos - new Vector3(30,0,0);
        wall.SetData(wallMoveData);
    }

    public void Quit()
    {
        Destroy(gameObject);
        FindAnyObjectByType<LevelManager>().RemoveCallback(ChangeLevel);
    }

    private void ChangeLevel(LevelManager.Levels level, bool reload)
    {
        if (!reload)
        {
            Quit();
        }
    }
}
