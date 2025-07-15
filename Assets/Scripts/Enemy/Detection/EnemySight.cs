using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySight : MonoBehaviour
{
    //Enemy using the sight object
    protected BaseEnemyBehaviour Enemy;

    //Player, and if player is visible
    protected MovementScript playerScript;
    protected bool playerVisible;

    //booleans for cloak state, used to track when player turns cloak on/off
    protected bool cloakLastFrame;
    protected bool cloakThisFrame;

    public abstract void LookAt(Vector3 position);
    public abstract void UpdateVisual();

    public abstract float calcSuspicionIncreaseRate(GameObject player);

    protected void Awake()
    {
        //Get the enemy the sight is attached to
        Enemy = transform.parent.GetComponent<BaseEnemyBehaviour>();
    }

    protected void FixedUpdate()
    {
        //Check if the player cloaked this frame
        if (playerScript)
        {
            cloakLastFrame = cloakThisFrame;
            cloakThisFrame = playerScript.cloaking;
        }
        else
        {
            cloakLastFrame = false;
            cloakThisFrame = false;
        }

        //Cloaked/uncloaked this frame
        if (cloakThisFrame != cloakLastFrame)
        {
            if (cloakThisFrame)
            {
                playerVisible = false;
                Enemy.LosePlayer();
            }
            else
            {
                playerVisible = true;
                Enemy.SeePlayer(playerScript.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerScript = collision.GetComponent<MovementScript>();
            if (!playerScript.cloaking)
            {
                playerVisible = true;
                Enemy.SeePlayer(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!playerScript.cloaking)
            {
                playerVisible = false;
                Enemy.LosePlayer();
            }
            playerScript = null;
        }
    }
}
