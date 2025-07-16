using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{

    [Tooltip("Does the block use a trigger collider, it can be triggered by chaining regardless")][SerializeField] bool noTriggerBox = false;
    [Tooltip("How long the block waits before beginning to break")][SerializeField] float waitDuration = 1f;
    [Tooltip("How long the block is visibly breaking for")][SerializeField] float breakingDuration = 1f;
    [Header("All chained blocks get triggered at once")]
    [Tooltip("List of all blocks that will start to break after this one does")][SerializeField] List<BreakableBlock> chainBlocks = new List<BreakableBlock>();

    // Start is called before the first frame update
    void Start()
    {
        if (!noTriggerBox) {
            transform.GetChild(0).gameObject.SetActive(true);
        } else {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginBreaking() {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(waitDuration);
        StartCoroutine(BreakBlock());
    }

    private IEnumerator BreakBlock() {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(breakingDuration);
        foreach (var block in chainBlocks) {
            if (block != null) {
                block.BeginBreaking();
            }
        }
        Destroy(gameObject);
    }

    public void OnValidate() {
        if (!noTriggerBox) {
            transform.GetChild(0).gameObject.SetActive(true);
        } else {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
}
