using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButtonScript : MonoBehaviour
{
    public AK.Wwise.Event buttonClick;
    public void ChangeScene(string sceneName)
    {
        buttonClick.Post(gameObject);
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToLevel()
    {
        buttonClick.Post(gameObject);
        SceneChangeTracker.GetTracker().GoBack();
    }

    public void Quit()
    {
        buttonClick.Post(gameObject);
        Application.Quit();
    }
}
