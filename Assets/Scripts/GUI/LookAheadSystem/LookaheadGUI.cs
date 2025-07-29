using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LookaheadGUI : MonoBehaviour
{
    //Positions had to be heavily shrunk cause when added they get multiplied by ~28 and go super far out
    static readonly Vector3 leftPosition=new Vector3(-800,0,0);
    static readonly Vector3 rightPosition=new Vector3(800,0,0);
    static readonly Vector3 unitVector = new Vector3(1, 1, 1);
    //Called when lookahead GUI is told to appear

    private LookaheadTracker tracker;

    private TextMeshProUGUI text;

    private RectTransform rectTransform;

    public float distance { get; private set; }
    public bool onLeftSide { get; private set; }

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>(true);
        rectTransform = GetComponent<RectTransform>();
    }
    public void LookaheadStart(LookaheadTracker tracker, bool onLeftSide)
    {
        //Update sprite
        GetComponentInChildren<Image>().sprite = tracker.displaySprite;
        //Move to correct side
        if(onLeftSide)
        {
            GetComponent<RectTransform>().localPosition = leftPosition;
        }
        else
        {
            GetComponent<RectTransform>().localPosition = rightPosition;
        }
        //Set tracker
        this.tracker = tracker;
        this.onLeftSide = onLeftSide;
    }

    public void setDistance(float distance)
    {
        this.distance = distance;
    }

    private float sizeCalculation(float distance)
    {
        //A should be a constant > 1
        float a = 1.2f;
        float b = 2.5f;
        return Mathf.Min(1, 1.0f / (distance + a) + 1.0f / b);
    }

    public void UpdateInformation(int order)
    {
        if (!text){
            return;
        }
        text.text = Mathf.Floor(distance).ToString() + "m";
        Vector3 pos = rectTransform.localPosition;
        pos.y = order * - 120;
        rectTransform.localPosition = pos;

        float size = sizeCalculation(distance);
        rectTransform.localScale = unitVector * size;

    }
}
