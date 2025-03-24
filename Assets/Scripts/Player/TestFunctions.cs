using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFunctions : MonoBehaviour
{
    GUIButtonScript buttonScript;
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
            AkSoundEngine.StopAll(gameObject);
            SceneManager.LoadScene("Main Menu");

            //Tell the score controller the player quit
            MainScoreController.GetInstance().Quit();
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

        //Teleport to building 1
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject.Find("Player").transform.position = new Vector3(-28.5f,-14.6f);
        }
        //Teleport to building 2
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject.Find("Player").transform.position = new Vector3(752.6f, -22f);
        }
    }

}
