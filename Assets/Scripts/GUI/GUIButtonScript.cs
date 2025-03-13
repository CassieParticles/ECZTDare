using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIButtonScript : MonoBehaviour
{
    public AK.Wwise.Event buttonClick;
    public AK.Wwise.Event titleMusic;
    public AK.Wwise.Event titleRain;

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

    private void Start()
    {
        AkSoundEngine.StopAll();
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "Menu");
        //Sets the "Ambience" State Group's active State to "NoAmbience"
        AkSoundEngine.SetState("Ambience", "Outside");

        titleMusic.Stop(gameObject);
        titleRain.Stop(gameObject);
        titleMusic.Post(gameObject);
        titleRain.Post(gameObject);

    }

    
}
