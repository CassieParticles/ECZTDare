using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    //How much the parallax effects
    [SerializeField,Range(0,1)] float parallaxPower = 0.5f;
    
    //Camera being followed
    GameObject camera;

    //Initial information
    float startX;
    float camStartX;

    private void Awake()
    {
        //Get camera
        camera = Camera.main.gameObject;

        startX = transform.position.x;
        camStartX = camera.transform.position.x;
    }

    private void Update()
    {
        //Get how far the camera has moved
        float deltaFromStart = ((camera.transform.position.x - camStartX) * parallaxPower);

        Vector3 newPos = transform.position;
        newPos.x = startX+deltaFromStart;
        transform.position = newPos;
    }
}
