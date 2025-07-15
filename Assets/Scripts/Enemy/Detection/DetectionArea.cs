using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : EnemySight
{
    private Color innerColour;
    private Color outerColour;

    public override float calcSuspicionIncreaseRate(GameObject player)
    {
        return Enemy.suspicionScaleRate * Time.fixedDeltaTime;
    }

    //Detection area doesn't move
    public override void LookAt(Vector3 position)
    {
        return;
    }

    private void UpdateSpriteColour()
    {
        transform.Find("OuterSprite").GetComponent<SpriteRenderer>().color = outerColour;
        transform.Find("InnerSprite").GetComponent<SpriteRenderer>().color = innerColour;
    }

    //Fill in texture
    public override void UpdateVisual()
    {
        if(playerVisible)
        {
            outerColour = Color.red;
            innerColour = Color.red;
            innerColour.a = 0.5f;

            UpdateSpriteColour();
        }
        else
        {
            outerColour = Color.white;
            innerColour = Color.white;
            innerColour.a = 0.5f;

            UpdateSpriteColour();
        }

        //Update texture
        Vector2 playerPosition = Vector2.zero;
        if(playerScript)
        {
            playerPosition = (playerScript.transform.position - transform.position);
            playerPosition.x /= transform.lossyScale.x;
            playerPosition.y /= transform.lossyScale.y;
        }

        float scale = Enemy.suspicion / 100.0f;
        scale = 1 - scale;
        scale /= 2;

        float left = playerPosition.x - scale;
        float right = playerPosition.x + scale;
        float top = playerPosition.y + scale;
        float bottom = playerPosition.y - scale;
        

        GetComponentInChildren<DetectionAreaSpritemask>().SetBounds(left,right, top, bottom);
    }

    private new void Awake()
    {
        base.Awake();

        outerColour = Color.white;
        innerColour = Color.white;
        innerColour.a = 0.5f;
        
        UpdateSpriteColour();

        UpdateVisual();
    }
}
