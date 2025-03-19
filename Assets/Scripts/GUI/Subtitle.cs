using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    enum TextMode {
        Instant,
        Write,
        WriteJarbled
    }

    [SerializeField] private List<string> text;
    [SerializeField] private float duration;
    [SerializeField] private TextMode SubtitleMode = TextMode.Instant;
    [SerializeField] private float writingSpeed;
    [SerializeField] private int jarblerSpeed;

    private TextMeshProUGUI textBox;

    private bool writing = false;
    private bool writingCoroutineStarted;
    public string writtenText = "";
    private int textNumber = 0;
    private bool finished = false;
    private bool waiting = false;
    private float timer;

    private void Awake() {
        //speaker = GameObject.Find("Player").GetComponentInChildren<TextMeshProUGUI>();
        textBox = GameObject.Find("SubtitleText").GetComponent<TextMeshProUGUI>();
    }

    public void RunEvent() {
        float writingDuration = text[textNumber].Length / writingSpeed;
        timer = writingDuration;
        writing = true;
    }

    public void EndEvent() {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (writing && !writingCoroutineStarted && timer >= 0 && !finished) {
            if (SubtitleMode != TextMode.Instant) {
                StartCoroutine(WriteText());
            } else {
                writtenText = text[textNumber];
                finished = true;
            }
        } else if (finished && !waiting && timer <= 0) {
            timer = duration;
            waiting = true;
        }
        duration -= Time.deltaTime;
        if (duration <= 0 && finished && !waiting) {
            EndEvent();
        }
    }

    IEnumerator WriteText() {
        writingCoroutineStarted = true;
        if (SubtitleMode == TextMode.WriteJarbled) {
            string textSoFar = "";
            for (int i = 0; i < text[textNumber].Length; i++) {
                for (int j = 0; j < jarblerSpeed; j++) {
                    writtenText = textSoFar + Convert.ToChar(UnityEngine.Random.Range(21, 126));
                    yield return new WaitForSeconds(1 / (writingSpeed * jarblerSpeed));
                }
                textSoFar += text[textNumber][i];
            }
        } else {
            for (int i = 0; i < text[textNumber].Length; i++) {
                    writtenText += text[textNumber][i];
                    yield return new WaitForSeconds(1 / writingSpeed);
            }
        }
        textNumber++;
        writing = false;
        writingCoroutineStarted = false;
        finished = true;
    }
}
