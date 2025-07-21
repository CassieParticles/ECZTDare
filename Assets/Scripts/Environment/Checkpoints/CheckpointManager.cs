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

        FindAnyObjectByType<LevelManager>().AddCallback(ChangeLevel);
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

    public void Quit()
    {
        Destroy(gameObject);
        m_Instance = null;
        FindAnyObjectByType<LevelManager>().RemoveCallback(ChangeLevel);
    }

    private void ChangeLevel(LevelManager.Levels level, bool reload)
    {
        if(level == LevelManager.Levels.MainMenu)
        {
            Quit();
        }
        else if(reload)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if(upcomingCheckpoint==0)
        {
            return;
        }
        FindFirstObjectByType<MovementScript>().transform.position = getRespawnPos();

        DeathwallRespawn deathwallRespawner = FindAnyObjectByType<DeathwallRespawn>();
        if(deathwallRespawner)
        {
            deathwallRespawner.Respawn(getRespawnPos());
        }
    }
}
