using System;
using System.Collections;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    enum TextMode {
        Instant,
        Write,
        WriteJarbled
    }

    //[SerializeField] private List<string> text;
    [SerializeField] private string text;
    [SerializeField] private float duration;
    [SerializeField] private TextMode SubtitleMode = TextMode.Instant;
    [SerializeField] private float writingSpeed;
    [SerializeField] private int jarblerSpeed;

    private SubtitleManager subtitleManager;

    private bool writing = false;
    private bool writingCoroutine;
    [NonSerialized] public string writtenText = "";
    //private int textNumber = 0;
    private bool finished = false;
    private bool waiting = false;
    private float timer;

    public bool turnOn = false;

    private void Awake() {
        //speaker = GameObject.Find("Player").GetComponentInChildren<TextMeshProUGUI>();
        subtitleManager = GameObject.Find("SubtitleText").GetComponent<SubtitleManager>();
    }

    private void StartSubtitle() {
        //float writingDuration = text.Length / writingSpeed;
        //timer = writingDuration;
        if (!subtitleManager.subtitles.Contains(this)) {
            writing = true;
            subtitleManager.AddSubtitle(this);
        }
    }

    public void EndSubtitle() {
        if (subtitleManager.subtitles.Contains(this)) {
            subtitleManager.RemoveSubtitle(this);
            subtitleManager.UpdateText();
            writing = false;
            finished = false;
            waiting = false;
            timer = 0;
            writtenText = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (turnOn) {
            turnOn = false;
            if (!writing || !finished || !waiting) {
                StartSubtitle();
            }
        }

        if (writing && !writingCoroutine && timer >= 0 && !finished) {
            if (SubtitleMode != TextMode.Instant) {
                StartCoroutine(WriteText());
            } else {
                writtenText = text/*[textNumber]*/;
                subtitleManager.UpdateText();
                writing = false;
                finished = true;
                timer = duration;
                waiting = true;
            }
        } else if (finished && !waiting && !writingCoroutine) {
            timer = duration;
            waiting = true;
        }
        if (timer > 0 && (writing | finished)) {
            timer -= Time.deltaTime;
        }
        if (timer <= 0 && finished && waiting) {
            EndSubtitle();
        }
    }

    IEnumerator WriteText() {
        writingCoroutine = true;
        string textSoFar = "";
        int iterationsPerFrame = 1;
        if (SubtitleMode == TextMode.WriteJarbled) {
            for (int i = 0; i < text.Length; i++) {
                for (int j = 0; j < jarblerSpeed; j++) {
                    writtenText = textSoFar + Convert.ToChar(UnityEngine.Random.Range(21, 122));
                    subtitleManager.UpdateText();
                    if (1f / writingSpeed * jarblerSpeed * iterationsPerFrame < 1f/60f) {
                        iterationsPerFrame++;
                    } else {
                        iterationsPerFrame = 1;
                        yield return new WaitForSeconds(1 / (writingSpeed * jarblerSpeed));
                    }
                }
                textSoFar += text[i];
            }
            writtenText = textSoFar;
            subtitleManager.UpdateText();
        } else {
            for (int i = 0; i < text.Length; i++) {
                textSoFar += text[i];
                writtenText = textSoFar;
                subtitleManager.UpdateText();
                if (1f / writingSpeed * iterationsPerFrame < 1f / 60f) {
                    iterationsPerFrame++;
                } else {
                    iterationsPerFrame = 1;
                    yield return new WaitForSeconds(1 / writingSpeed);
                }
            }
        }
        //textNumber++;
        writing = false;
        writingCoroutine = false;
        finished = true;
    }
}
