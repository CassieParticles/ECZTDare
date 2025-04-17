using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    [SerializeField] bool fadeToBlack = true;
    MenuScript menu;

    private void Start() {
        menu = GameObject.Find("Menu Canvas").GetComponent<MenuScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If score system exists
        if(MainScoreController.GetInstance() && MainScoreController.GetInstance().currentlyScoring)
        {
            MainScoreController.GetInstance().EndLevel(fadeToBlack);
        }
        else //If score system doesn't exist
        {
            menu.Win();
        }
    }
}
