using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public List<Subtitle> subtitles;
    [SerializeField]
    private TextMeshProUGUI textBox;
    private StringBuilder stringBuilder;
    public void AddSubtitle(Subtitle subtitle) {
        subtitles.Add(subtitle);
    }

    public void RemoveSubtitle(Subtitle subtitle) {
        subtitles.Remove(subtitle);
    }

    public void UpdateText() {
        stringBuilder.Clear();
        if (subtitles.Count != 0) {
            foreach (Subtitle subtitle in subtitles) {
                stringBuilder.Append(subtitle.writtenText).Append("\n");
            }
        }
        textBox.text = stringBuilder.ToString();
    }

    // Start is called before the first frame update
    void Start() {
        textBox = GetComponent<TextMeshProUGUI>();
        stringBuilder = new StringBuilder();
    }
}
