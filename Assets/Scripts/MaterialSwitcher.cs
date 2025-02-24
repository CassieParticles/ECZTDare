using AK.Wwise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    enum MaterialTypes
    {
        Concrete,
        Dirt,
        Rubber,
        Metal
    }

    [SerializeField] private MaterialTypes materialType;
    //BoxCollider2D boxCollider;
    BoxCollider2D player;

    void Start()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").GetComponent<BoxCollider2D>();

        //Sets the "Player_Footstep_Material" Switch Group's active State to "Outside"
        AkSoundEngine.SetSwitch("Player_Footstep_Material", "Concrete", player.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == player)
        {
            //Change reverb to reverbType

            if (materialType == MaterialTypes.Concrete)
            {
                //Sets the "Reverb" State Group's active State to "Concrete"
                AkSoundEngine.SetSwitch("Player_Footstep_Material", "Concrete", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Jump_Material", "Concrete", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Land_Material", "Concrete", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Slide_Material", "Concrete", player.gameObject);
                //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Concrete", guard.gameObject);
            }

            if (materialType == MaterialTypes.Dirt)
            {
                //Sets the "Reverb" State Group's active State to "Dirt"
                AkSoundEngine.SetSwitch("Player_Footstep_Material", "Dirt", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Jump_Material", "Dirt", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Land_Material", "Dirt", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Slide_Material", "Dirt", player.gameObject);
                //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Dirt", guard.gameObject);
                //AkSoundEngine.GetState("Reverb", out currentState);
            }

            if (materialType == MaterialTypes.Rubber)
            {
                //Sets the "Reverb" State Group's active State to "Rubber"
                AkSoundEngine.SetSwitch("Player_Footstep_Material", "Rubber", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Jump_Material", "Rubber", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Land_Material", "Rubber", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Slide_Material", "Rubber", player.gameObject);
                //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Rubber", guard.gameObject);
            }

            if (materialType == MaterialTypes.Metal)
            {
                //Sets the "Reverb" State Group's active State to "Metal"
                AkSoundEngine.SetSwitch("Player_Footstep_Material", "Metal", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Jump_Material", "Metal", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Land_Material", "Metal", player.gameObject);
                AkSoundEngine.SetSwitch("Player_Slide_Material", "Metal", player.gameObject);
                //AkSoundEngine.SetSwitch("Guard_Footstep_Material", "Metal", guard.gameObject);
            }
            
        }
    }
}
