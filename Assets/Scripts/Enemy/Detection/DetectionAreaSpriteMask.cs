using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionAreaSpritemask : MonoBehaviour
{
    public void SetBounds(float left, float right, float top, float bottom)
    {
        float width = (right - left);
        float height = (top - bottom);

        float x = left + width / 2;
        float y = bottom + height / 2;

        transform.localPosition=new Vector2(x, y);
        transform.localScale = new Vector3(width, height, 1);
    }
}
