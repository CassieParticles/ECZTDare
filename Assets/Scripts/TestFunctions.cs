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
        //Send player back to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Sets the "Music" State Group's active State to "Hidden"
            AkSoundEngine.SetState("Music", "NoMusic");
            musicHandler.music.Stop(gameObject);
            //Sets the "Ambience" State Group's active State to "NoAmbience"
            AkSoundEngine.SetState("Ambience", "NoAmbience");
            musicHandler.rain.Stop(gameObject);
            SceneManager.LoadScene("Main Menu");
            musicHandler.music.Post(gameObject);
            musicHandler.rain.Post(gameObject);

        }

        //Reset Alarm
        if (Input.GetKeyDown(KeyCode.M))
        {
            AlarmSystem[] alarms = FindObjectsByType<AlarmSystem>(FindObjectsSortMode.None);

            for (int i = 0; i < alarms.Length; i++)
            {
                alarms[i].StopAlarm();
            }
        }
    }

}
