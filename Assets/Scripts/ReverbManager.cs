using AK.Wwise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbManager : MonoBehaviour
{
    enum ReverbTypes
    {
        Outside,
        Small,
        Medium,
        Large
    }

    [SerializeField] private ReverbTypes reverbType;
    BoxCollider collider;
    GameObject player;



    // Start is called before the first frame update

    private void Awake()
    {
        name =  "Reverb " + reverbType.ToString();
    }
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        player = GameObject.Find("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collider == player)
        {
            //Change reverb to reverbType
            if (reverbType == ReverbTypes.Small)
            {
                //Change it to small
            }

            if (reverbType == ReverbTypes.Medium)
            {
                //Change it to medium
            }

            if (reverbType == ReverbTypes.Large)
            {
                //Change it to large
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collider == player)
        {
            //Change reverb to outside
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
