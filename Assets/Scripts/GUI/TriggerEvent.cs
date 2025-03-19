/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private List<Event> events;
    [SerializeField] private bool stopInputs = false; //If the trigger prevents the player from moving until it is finished
    [SerializeField] private bool continueIfExitTrigger = false; //If the trigger's events continue to happen if the player leaves the trigger, mostly used for dialogue

    BoxCollider2D trigger;
    GameObject player;
    Rigidbody2D playerRB;

    private bool runningEvents = false;
    private bool coroutineStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        runningEvents = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!continueIfExitTrigger) {
            runningEvents = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (runningEvents && !coroutineStarted) {
            StartCoroutine(RunAllEvents());
        } else if (!runningEvents && coroutineStarted) {
            StopCoroutine(RunAllEvents());    
            coroutineStarted = false;
        }
    }

    IEnumerator RunAllEvents() {
        coroutineStarted = true;
        for (int i = 0; i < events.Count; i++) {
            yield return new WaitForSeconds(events[i].pauseBefore);
            events[i].RunEvent();
            yield return new WaitForSeconds(events[i].duration);
            events[i].EndEvent();
            yield return new WaitForSeconds(events[i].pauseAfter);
        }
        runningEvents = false;
        coroutineStarted = false;
    }
}
*/