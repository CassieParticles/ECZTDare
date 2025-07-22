using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathwallRespawn : MonoBehaviour
{
    [SerializeField] private GameObject DeathWallPrefab;

    DeathWall.WallMoveData wallMoveData;
    private bool DeathWallActive;

    private static DeathwallRespawn instance;


    public static DeathwallRespawn GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }

    private void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        //If quitting to main menu
        if(scene.buildIndex == 0)
        {
            Quit();
        }
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
        {
            return; 
        }
        Debug.Log("Respawn happening");
        DeathWall wall = Instantiate(DeathWallPrefab).GetComponent<DeathWall>();
        wall.transform.position = respawnPos - new Vector3(30,0,0);
        wall.SetData(wallMoveData);
    }

    private void Quit()
    {
        Destroy(gameObject);
    }
}
