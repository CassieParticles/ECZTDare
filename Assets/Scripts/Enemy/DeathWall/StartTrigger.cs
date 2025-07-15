using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTrigger : MonoBehaviour
{
    [SerializeField] private GameObject DeathWall;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Spawn death wall at spawn location
        GameObject newWall = Instantiate(DeathWall,transform.GetChild(0));
        newWall.transform.SetParent(null);

        //Deactivate
        gameObject.SetActive(false);

        //Set the parameters in the new wall
        newWall.GetComponent<DeathWall>().SetData(transform.GetChild(0).GetComponent<WallData>().wallMoveData);
    }
}
