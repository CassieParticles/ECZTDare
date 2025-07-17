using UnityEngine;

public class MusicStateChanger : MonoBehaviour
{
    enum MusicTypes
    {
        Hidden,
        Alarm_Low,
        Alarm_Middle,
        Alarm_High,
        Menu,
        Cutscene,
        NoMusic
    }

    [SerializeField] private MusicTypes musicTypes;

    //BoxCollider2D boxCollider;
    BoxCollider2D player;

    void Start()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == player)
        {
            //Change music to musicType

            if (musicTypes == MusicTypes.Hidden)
            {
                //Sets the "Music" State Group's active State to "Hidden"
                AkSoundEngine.SetState("Music", "Hidden");
            }

            if (musicTypes == MusicTypes.Alarm_Low)
            {
                //Sets the "Music" State Group's active State to "Alarm_Low"
                AkSoundEngine.SetState("Music", "Alarm_Low");
            }

            if (musicTypes == MusicTypes.Alarm_Middle)
            {
                //Sets the "Music" State Group's active State to "Alarm_Middle"
                AkSoundEngine.SetState("Music", "Alarm_Middle");
            }

            if (musicTypes == MusicTypes.Alarm_High)
            {
                //Sets the "Music" State Group's active State to "Alarm_High"
                AkSoundEngine.SetState("Music", "Alarm_High");
            }

            if (musicTypes == MusicTypes.Menu)
            {
                //Sets the "Music" State Group's active State to "Menu"
                AkSoundEngine.SetState("Music", "Menu");
            }

            if (musicTypes == MusicTypes.Cutscene)
            {
                //Sets the "Music" State Group's active State to "Cutscene"
                AkSoundEngine.SetState("Music", "Cutscene");
            }

            if (musicTypes == MusicTypes.NoMusic)
            {
                //Sets the "Music" State Group's active State to "NoMusic"
                AkSoundEngine.SetState("Music", "NoMusic");
            }
        }
    }
}
