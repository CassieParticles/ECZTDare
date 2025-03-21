using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public List<Subtitle> subtitles;
    public TextMeshProUGUI textBox;
    public string currentSubtitles;
    public void AddSubtitle(Subtitle subtitle) {
        subtitles.Add(subtitle);
    }

    public void RemoveSubtitle(Subtitle subtitle) {
        subtitles.Remove(subtitle);
    }

    public void UpdateText() {
        currentSubtitles = "";
        if (subtitles.Count != 0) {
            foreach (Subtitle subtitle in subtitles) {
                currentSubtitles += subtitle.writtenText + "\n";
            }
        }
        textBox.text = currentSubtitles;
    }

    // Start is called before the first frame update
    void Start() {
        textBox = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (subtitles.Count != 0) {
        //    foreach (Subtitle subtitle in subtitles) {
        //        currentSubtitles += subtitle.writtenText + "\n";
        //    }
       // }
        //textBox.text = currentSubtitles;
    }
}
