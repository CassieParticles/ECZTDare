using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PullCameraScript : MonoBehaviour
{
    private GameObject mainCamera; //The main camera in the scene
    private CameraMovement mainCameraScript;
    private PixelPerfectCamera mainCameraZoom;
    private Rigidbody2D playerRB;
    
    //[SerializeField] private Collider2D trigger; //The collider that triggers the pulling of the camera
    
    [SerializeField] bool holdCamera = false; //Whether the object with this script holds the camera on its position or lightly tugs it towards it
    [SerializeField][Range(0, 1)] float tugStrength; //How strongly the object tugs on the camera (only works if holdCamera is false)
    [SerializeField] float ExponentX; //How quickly the camera moves with distance in the X axis
    [SerializeField] float ExponentY; //How quickly the camera moves with distance in the Y axis
    [SerializeField][Range(0, 128)] float zoomPPU; //How much the camera zooms when being pulled

    private bool isPulling = false;

    // Start is called before the first frame update
    void Start() {
        mainCamera = GameObject.Find("Main Camera");
        mainCameraScript = mainCamera.GetComponent<CameraMovement>();
        mainCameraZoom = mainCamera.GetComponent<PixelPerfectCamera>();
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        isPulling = true;
        mainCameraScript.isBeingPulled = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isPulling = false;
        mainCameraScript.isBeingPulled = false;
        mainCameraScript.pulledTargetPos = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update() {
        if (isPulling) {
            mainCameraScript.pulledExponentX = ExponentX;
            mainCameraScript.pulledExponentY = ExponentY;
            if (holdCamera) {
                mainCameraScript.pulledTargetPos = transform.position;
            } else {
                mainCameraScript.pulledTargetPos = Vector2.Lerp(playerRB.position, transform.position, tugStrength);
            }
        }
    }
}
