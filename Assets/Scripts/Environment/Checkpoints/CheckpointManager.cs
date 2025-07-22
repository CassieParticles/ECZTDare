using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager m_Instance;

    GameObject[] checkpoints;
    int upcomingCheckpoint;
    private void Awake()
    {
        if(m_Instance)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        checkpoints = new GameObject[transform.childCount];
        upcomingCheckpoint = 0;
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            checkpoints[i] = transform.GetChild(i).gameObject;
            checkpoints[i].GetComponent<Checkpoint>().SetIndex(i);
        }
        SceneManager.sceneLoaded += SceneLoad;
    }

    public void CheckpointReach(int i)
    {
        //Player is backtracking, ignore signal
        if (i < upcomingCheckpoint){ return; }
        upcomingCheckpoint = i+1;
    }

    public Vector3 getRespawnPos()
    {
        return checkpoints[upcomingCheckpoint - 1].transform.position;
    }

    public void GoToCurrentCheckpoint()
    {
        if(upcomingCheckpoint < 1){ return; }
        FindFirstObjectByType<MovementScript>().transform.position = getRespawnPos();
    }
    public void GoToNextCheckpoint()
    {
        upcomingCheckpoint++;
        if (upcomingCheckpoint > checkpoints.Length - 1){ upcomingCheckpoint = checkpoints.Length - 1; }
        GoToCurrentCheckpoint();
    }

    public void GoToPrevCheckpoint()
    {
        upcomingCheckpoint--;
        if(upcomingCheckpoint < 0){  upcomingCheckpoint = 0; }
        GoToCurrentCheckpoint();
    }

    private void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name=="WinScreen")
        {
            Win();
        }
        if(scene.name!="Main Menu" && scene.name!="LoseScene")
        {
            Respawn();
        }
    }

    public void Quit()
    {
        Destroy(gameObject);
        m_Instance = null;
        SceneManager.sceneLoaded -= SceneLoad;
    }

    public void Respawn()
    {
        if(upcomingCheckpoint==0)
        {
            return;
        }
        FindFirstObjectByType<MovementScript>().transform.position = getRespawnPos();

        DeathwallRespawn deathwallRespawner = DeathwallRespawn.GetInstance();
        if(deathwallRespawner)
        {
            deathwallRespawner.Respawn(getRespawnPos());
        }
    }

    public void Win()
    {
        Destroy(gameObject);
    }
}
