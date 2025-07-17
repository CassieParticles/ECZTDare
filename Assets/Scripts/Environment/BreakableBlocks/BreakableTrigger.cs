using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableTrigger : MonoBehaviour
{

    BreakableBlock block;
    // Start is called before the first frame update
    void Start()
    {
        block = GetComponentInParent<BreakableBlock>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        block.BeginBreaking();
    }
}
