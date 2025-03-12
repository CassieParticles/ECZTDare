using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Stop the clock
        MainScoreController scoreController = MainScoreController.GetInstance();
        if(scoreController)
        {
            scoreController.StopTimer();
            Debug.Log("Stop the clock!");
        }
        SceneManager.LoadScene("WinScreen");
    }
}
