using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFunctions : MonoBehaviour
{
    AlarmMusicHandler musicHandler;
    [SerializeField] GameObject testCamera;

    // Start is called before the first frame update
    void Start()
    {
        musicHandler = GameObject.Find("MusicSystem").GetComponent<AlarmMusicHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            //Sets the "Music" State Group's active State to "Hidden"
            AkSoundEngine.SetState("Music", "NoMusic");
            musicHandler.music.Stop(gameObject);
            //Sets the "Ambience" State Group's active State to "NoAmbience"
            AkSoundEngine.SetState("Ambience", "NoAmbience");
            musicHandler.rain.Stop(gameObject);
            SceneManager.LoadScene("Level1");
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Hacking");
            testCamera.GetComponent<CameraHackable>().OnHack();
        }
    }
}
