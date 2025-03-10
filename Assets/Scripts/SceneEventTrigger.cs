using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEventTrigger : MonoBehaviour
{
    [SerializeField] private GameObject disable1;
    [SerializeField] private GameObject disable2;
    [SerializeField] private GameObject disable3;
    [SerializeField] private GameObject hudEnable1;
    private void Start()
    {
       
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //disable1.gameObject.SetActive(false);
            //disable2.gameObject.SetActive(false);
            //disable3.gameObject.SetActive(false);
            Destroy(disable1);
            Destroy(disable2);
            Destroy(disable3);
            hudEnable1.gameObject.SetActive(true);

        }
    }

}


