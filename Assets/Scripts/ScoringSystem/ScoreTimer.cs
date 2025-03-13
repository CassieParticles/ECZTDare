using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTimer : MonoBehaviour
{
    public float time {  get; private set; }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }
}
