using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowManager : MonoBehaviour
{

    public float intervalSeconds = 3f;
    public float transitionSeconds = 1f;
    public List<Sprite> sprites;

    private bool onImage1 = true;
    private float spriteIndex;

    private Image Image1;
    private Image Image2;

    // Start is called before the first frame update
    void Start()
    {
        Image1 = GameObject.Find("Image1").GetComponent<Image>();
        Image2 = GameObject.Find("Image2").GetComponent<Image>();
        Image1.sprite = sprites[0];
        Image2.sprite = sprites[0];
        Color color = new Color(1, 1, 1, 0);
        Image2.color = color;
        StartCoroutine(Slideshow());
    }

    public IEnumerator Slideshow() {
        while (true) {
            for (int i = 0; i < sprites.Count; i++) {
                Image1.sprite = sprites[i];
                for (float transitionTimer = 0; transitionTimer < transitionSeconds;) {
                    transitionTimer += Time.deltaTime;
                    float percentage = transitionTimer / transitionSeconds;
                    Image2.color = new Color(1, 1, 1, 1 - percentage);
                    yield return null;
                }
                Image2.sprite = Image1.sprite;
                yield return new WaitForSeconds(intervalSeconds);
            }
        }
    }
}
