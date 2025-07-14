using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : EnemySight
{
    private Texture2D areaTexture;
    private Color areaColour;
    public override float calcSuspicionIncreaseRate(GameObject player)
    {
        return Enemy.suspicionScaleRate * Time.fixedDeltaTime;
    }

    //Detection area doesn't move
    public override void LookAt(Vector3 position)
    {
        return;
    }

    //Fill in texture
    public override void UpdateVisual()
    {
        //Update colour
        switch (Enemy.suspicionState)
        {
            case BaseEnemyBehaviour.SuspicionState.Idle:
                areaColour = Color.white;
                break;
            default:
                areaColour = Color.red;
                break;
        }

        //Update texture
        Vector2 playerPosition = transform.position;
        if(playerScript)
        {
            playerPosition = playerScript.transform.position;
        }
        Vector2 tlCorner = transform.position + (transform.localScale * -0.5f);

        Vector2 playerPosUV = (playerPosition - tlCorner) / transform.localScale;

        

        //pre-calcualte consistent values
        float leftDist = playerPosUV.x;
        float rightDist = 1.0f - leftDist;
        float topDist = playerPosUV.y;
        float bottomDist = 1.0f - topDist;
        float scale = Enemy.suspicion / 100.0f;

        //Pre-multiply
        leftDist *= scale;
        rightDist *= scale;
        topDist *= scale;
        bottomDist *= scale;

        Color32[] colourArray = new Color32[128 * 128];

        //Iterate through pixels setting colours
        for(int x=0;x < 128;x++)
        {
            for(int y=0;y < 128;y++) 
            {
                int index = y * 128 + x;
                Vector2 uv = new Vector2(x / 128.0f, y / 128.0f);

                colourArray[index] = areaColour;

                if(!(uv.x < leftDist|| uv.x > 1.0f - rightDist|| uv.y < topDist || uv.y > 1.0f - bottomDist))
                {
                    colourArray[index].a = 0x66;
                }
            }
        }

        areaTexture.SetPixels32(colourArray);
        areaTexture.Apply();
    }

    private new void Awake()
    {
        base.Awake();

        areaTexture = new Texture2D(128, 128);
        Sprite sprite = Sprite.Create(areaTexture, new Rect(0, 0, areaTexture.width, areaTexture.height), new Vector2(0.5f,0.5f),128);

        GetComponent<SpriteRenderer>().sprite = sprite;
        areaColour = Color.white;
        UpdateVisual();
    }
}
