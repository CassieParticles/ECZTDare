using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneControl : MonoBehaviour
{
    [SerializeField] GameObject ScoreGUIPrefab;

    GameObject ScoreGUI;
    PlayableDirector director;

    Rigidbody2D playerRB;
    Vector2 freezeVelocity;

    bool fadeToBlack;

    bool endOfLevel;

    private IEnumerator FreezePlayer(Rigidbody2D playerRB)
    {
        yield return new WaitForSeconds(0.5f);
        playerRB.bodyType = RigidbodyType2D.Static;
    }

    private void Awake()
    {
        director=GetComponent<PlayableDirector>();
    }

    public void DisplayScore(float timeTaken, int stealthScore, bool endOfLevel, bool fadeToBlack)
    {
        //Cutscene GUI should remain if fade to black is true, so the black remains
        this.fadeToBlack =fadeToBlack;
        //Create GUI and get text
        ScoreGUI = Instantiate(ScoreGUIPrefab);
        GameObject TimeScoreText = ScoreGUI.transform.GetChild(4).gameObject;
        GameObject StealthScoreText = ScoreGUI.transform.GetChild(5).gameObject;

        //Format the time taken into mm:ss
        int minutes = (int)timeTaken / 60;
        int seconds = (int)timeTaken % 60;

        //Minutes and seconds as string, formatted so seconds is 
        string minutesStr = minutes.ToString();
        string secondsStr = seconds.ToString();

        if (secondsStr.Length == 1)
        {
            secondsStr = "0" + secondsStr;
        }

        string timeStr = minutesStr;
        timeStr += ":";
        timeStr += secondsStr;

        //Update text with score
        TimeScoreText.GetComponent<TextMeshProUGUI>().text = timeStr;
        StealthScoreText.GetComponent<TextMeshProUGUI>().text = stealthScore.ToString();

        ScoreGUI.transform.GetChild(0).gameObject.SetActive(fadeToBlack);
        


        //Set up Bindings
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        foreach (TrackAsset item in timeline.GetOutputTracks())
        {
            switch (item.name)
            {
                case "FadeToBlack":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(0).gameObject);
                    break;
                case "Title":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(1).gameObject);
                    break;
                case "TimeTaken":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(2).gameObject);
                    break;
                case "StealthScore":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(3).gameObject);
                    break;
                case "Speed":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(4).gameObject);
                    break;
                case "Stealth":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(5).gameObject);
                    break;
            }
        }

        //Start cutscene
        director.Play();

        //Pause player
        MovementScript player = FindAnyObjectByType<MovementScript>();
        playerRB = player.GetComponent<Rigidbody2D>();

        player.InputLocked = true;
        freezeVelocity = playerRB.velocity;
        StartCoroutine(FreezePlayer(playerRB));

        this.endOfLevel = endOfLevel;
    }

    public void EndCutscene()
    {
        //Unpause player
        MovementScript player = FindAnyObjectByType<MovementScript>();
        player.InputLocked = false;
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        playerRB.velocity = freezeVelocity;

        if(!fadeToBlack)
        {
            Destroy(ScoreGUI);
            ScoreGUI = null;
        }
        //Non-final cutscene ends here
        if (!endOfLevel)
        {
            Destroy(gameObject);
            return; 
        }

        //End level
        Debug.Log("End of level");
        if (FindAnyObjectByType<MenuScript>())
        {
            FindAnyObjectByType<MenuScript>().Win();
        }

        Destroy(gameObject);
    }
}
