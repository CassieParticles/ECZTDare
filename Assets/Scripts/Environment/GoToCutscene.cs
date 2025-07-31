using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToCutscene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<MovementScript>())
        {
            MenuScript menu = FindAnyObjectByType<MenuScript>();
            if (menu)
            {
                menu.ChangeScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
