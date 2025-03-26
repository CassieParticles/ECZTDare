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

    public void DisplayScore(float timeTaken, int stealthScore, bool endOfLevel)
    {
        //Create GUI and get text
        ScoreGUI = Instantiate(ScoreGUIPrefab);
        GameObject TimeScoreText = ScoreGUI.transform.GetChild(3).gameObject;
        GameObject StealthScoreText = ScoreGUI.transform.GetChild(4).gameObject;

        //Format the time taken into mm:ss
        int minutes = (int)timeTaken / 60;
        int seconds = (int)timeTaken % 60;

        string timeStr=minutes.ToString();    //Minutes
        timeStr += ":";
        timeStr += seconds.ToString();

        //Update text with score
        TimeScoreText.GetComponent<TextMeshProUGUI>().text = timeStr;
        StealthScoreText.GetComponent<TextMeshProUGUI>().text = stealthScore.ToString();

        //Set up Bindings
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        foreach (TrackAsset item in timeline.GetOutputTracks())
        {
            Debug.Log(item.name);
            switch (item.name)
            {
                case "Title":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(0).gameObject);
                    break;
                case "TimeTaken":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(1).gameObject);
                    break;
                case "StealthScore":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(2).gameObject);
                    break;
                case "Speed":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(3).gameObject);
                    break;
                case "Stealth":
                    director.SetGenericBinding(item, ScoreGUI.transform.GetChild(4).gameObject);
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

        Destroy(ScoreGUI);
        ScoreGUI = null;

        //Non-final cutscene ends here
        if (!endOfLevel)
        { return; }

        //Play final cutscene showing players score
        MainScoreController.GetInstance().EndLevelCont();

        //End level
        Debug.Log("End of level");
        if (FindAnyObjectByType<MenuScript>())
        {
            FindAnyObjectByType<MenuScript>().Win();
        }
    }
}
