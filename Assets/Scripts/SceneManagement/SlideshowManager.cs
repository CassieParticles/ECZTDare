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
    private List<Sprite> shuffledSprites;

    private Image Image1;
    private Image Image2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RepeatStart());
    }

    public IEnumerator RepeatStart() {
        while (Image1 == null) {
            if (GameObject.Find("Image1") != null) {
                Image1 = GameObject.Find("Image1").GetComponent<Image>();
                Image2 = GameObject.Find("Image2").GetComponent<Image>();
                Image1.sprite = sprites[0];
                Image2.sprite = sprites[0];
                Color color = new Color(1, 1, 1, 0);
                Image2.color = color;
                StartCoroutine(Slideshow());
            }
            yield return null;
        }
    }

    private void Update() {

    }

    public IEnumerator Slideshow() {
        int randomStart = Mathf.FloorToInt(UnityEngine.Random.Range(0, sprites.Count));
        Image1.sprite = sprites[randomStart];
        Image2.sprite = Image1.sprite;
        yield return new WaitForSeconds(intervalSeconds);
        for (int i = randomStart + 1; i < sprites.Count; i++) {
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
            /*
            shuffledSprites = new List<Sprite>(sprites); //Reset the list
            while (shuffledSprites.Count >= 1) {
                int randomSprite = Mathf.FloorToInt(UnityEngine.Random.Range(0, shuffledSprites.Count));
                Image1.sprite = shuffledSprites[randomSprite];
                for (float transitionTimer = 0; transitionTimer < transitionSeconds;) {
                    transitionTimer += Time.deltaTime;
                    float percentage = transitionTimer / transitionSeconds;
                    Image2.color = new Color(1, 1, 1, 1 - percentage);
                    yield return null;
                }
                Image2.sprite = Image1.sprite;
                shuffledSprites.RemoveAt(randomSprite);
                yield return new WaitForSeconds(intervalSeconds);
            }
            */
        }
    }
}
