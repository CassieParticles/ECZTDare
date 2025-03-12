using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeChanger : MonoBehaviour
{
    enum Modes {
        MovementMode,
        StealthMode
    }

    [SerializeField] private Modes switchToMode;

    GameObject player;
    BoxCollider2D playerCollider;
    MovementScript playerScript;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerScript = player.GetComponent<MovementScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision == playerCollider) {
            if (switchToMode == Modes.MovementMode) {
                playerScript.inStealthMode = false;
            } else {
                playerScript.inStealthMode = true;
            }
        }   
    }
}
