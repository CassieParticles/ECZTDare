using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAndDestroy : MonoBehaviour
{
    [Header("Shrink Settings")]
    [SerializeField] private float minSize  = 0.2f;
    [SerializeField] private float shrinkRate = 0.4f;
    [SerializeField] private float scale = 1f;
    [SerializeField] private float delay = 2f;

    [Header("Text Content")]
    [SerializeField][TextArea] private string[] textDisplay;
    [SerializeField] private float textSpeed = 0.01f;

    [Header("UI Elements")]
    [SerializeField] private TMPro.TextMeshPro displayedText;
    private int currentDisplayingText = 0;


void Update()
    {
        shrink();
       
    }

    private void Start()
    {
        StartCoroutine(AnimateText());
    }

    void shrink()
    {
        transform.localScale = Vector3.one * scale;
        scale -= shrinkRate * Time.deltaTime;
        if (scale < minSize) Destroy(gameObject);
        Debug.Log(scale);
    }

    IEnumerator AnimateText()
    {
        for (int i = 0; i < textDisplay[currentDisplayingText].Length +1 ; i++)
        {
            displayedText.text = textDisplay[currentDisplayingText].Substring(0,i);
        }
        yield return new WaitForSeconds(textSpeed);
    }

}
