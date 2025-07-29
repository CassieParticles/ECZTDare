using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookaheadCanvas : MonoBehaviour
{
    void Start()
    {
        //Set Screen Space camera to main camera
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
