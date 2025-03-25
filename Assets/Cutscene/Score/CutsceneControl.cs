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

    private IEnumerator FreezePlayer(Rigidbody2D playerRB)
    {
        yield return new WaitForSeconds(0.5f);
        playerRB.bodyType = RigidbodyType2D.Static;
    }

    private void Awake()
    {
        director=GetComponent<PlayableDirector>();
    }

    public void DisplayScore(float timeTaken, int stealthScore)
    {
        //Create GUI and get text
        ScoreGUI = Instantiate(ScoreGUIPrefab);
        GameObject TimeScoreText = ScoreGUI.transform.GetChild(3).gameObject;
        GameObject StealthScoreText = ScoreGUI.transform.GetChild(4).gameObject;

        //Update text with score
        TimeScoreText.GetComponent<TextMeshProUGUI>().text = timeTaken.ToString();
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
    }
}
