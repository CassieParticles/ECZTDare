using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderControl : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVolume;
    public float musicVolume;
    public float soundVolume;
    public float dialogueVolume;
    public float ambienceVolume;

    public void SetVolume(string whatValue)
    {
        float slidervalue = thisSlider.value;

        if (whatValue == "Master")
        {
            masterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        }

        if (whatValue == "Music")
        {
            musicVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);
        }

        if (whatValue == "Sound")
        {
            soundVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("SoundVolume", soundVolume);
        }

        if (whatValue == "Dialogue")
        {
            dialogueVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("DialogueVolume", dialogueVolume);
        }

        if (whatValue == "Ambience")
        {
            ambienceVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("AmbienceVolume", ambienceVolume);
        }
    }
}
