using UnityEngine;

public class ReverbStateChanger : MonoBehaviour
{
    enum ReverbTypes
    {
        Outside,
        Small,
        Medium,
        Large
    }
    enum AmbienceTypes
    {
        Inside,
        Outside,
        NoAmbience
    }

    [SerializeField] private ReverbTypes reverbType;
    [SerializeField] private AmbienceTypes ambienceType;

    //BoxCollider2D boxCollider;
    BoxCollider2D player;

    void Start()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();

        //Sets the "Reverb" State Group's active State to "Outside"
        AkSoundEngine.SetState("Reverb", "Outside");
        //Sets the "Ambience" State Group's active State to "Outside"
        AkSoundEngine.SetState("Ambience", "Outside");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == player)
        {
            //Change reverb to reverbType, and ambience to ambienceType

            if (reverbType == ReverbTypes.Outside)
            {
                //Sets the "Reverb" State Group's active State to "Outside"
                AkSoundEngine.SetState("Reverb", "Outside");
            }

            if (reverbType == ReverbTypes.Small)
            {
                //Sets the "Reverb" State Group's active State to "Small"
                AkSoundEngine.SetState("Reverb", "Small");
            }

            if (reverbType == ReverbTypes.Medium)
            {
                //Sets the "Reverb" State Group's active State to "Medium"
                AkSoundEngine.SetState("Reverb", "Medium");
            }

            if (reverbType == ReverbTypes.Large)
            {
                //Sets the "Reverb" State Group's active State to "Large"
                AkSoundEngine.SetState("Reverb", "Large");
            }

            if (ambienceType == AmbienceTypes.Inside)
            {
                //Sets the "Ambience" State Group's active State to "Inside"
                AkSoundEngine.SetState("Ambience", "Inside");
            }

            if (ambienceType == AmbienceTypes.Outside)
            {
                //Sets the "Ambience" State Group's active State to "Outside"
                AkSoundEngine.SetState("Ambience", "Outside");
            }

            if (ambienceType == AmbienceTypes.NoAmbience)
            {
                //Sets the "Ambience" State Group's active State to "Outside"
                AkSoundEngine.SetState("Ambience", "NoAmbience");
            }
        }
    }
}
