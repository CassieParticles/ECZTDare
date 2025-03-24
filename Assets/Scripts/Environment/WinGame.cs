using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    MenuScript menu;

    private void Start() {
        menu = GameObject.Find("Menu Canvas").GetComponent<MenuScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Stop the clock
        MainScoreController scoreController = MainScoreController.GetInstance();
        if(scoreController)
        {
            scoreController.StopTimer();
            Debug.Log("Stop the clock!");
        }
        menu.Win();
    }
}
