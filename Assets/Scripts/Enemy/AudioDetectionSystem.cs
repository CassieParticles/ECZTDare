using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetectionSystem : MonoBehaviour
{
    public delegate void HearNoise(Vector3 soundLocation, float suspicionIncrease);

    private Dictionary<GameObject, HearNoise> listeners;

    public static AudioDetectionSystem getAudioSystem()
    {
        return GameObject.Find("AudioDetectionHandler").GetComponent<AudioDetectionSystem>();
    }

    public void AddListener(GameObject gameObject, HearNoise listenerFunc)
    {
        listeners.Add(gameObject,listenerFunc);
    }

    public void PlaySound(Vector3 noiseLocation, float noiseRadius, float suspicionIncrease)
    {
        foreach (KeyValuePair<GameObject, HearNoise> listener in listeners)
        {
            //If gameObject is within range to hear noise
            if (noiseRadius * noiseRadius > (noiseLocation - listener.Key.transform.position).sqrMagnitude)
            {
                listener.Value(noiseLocation,suspicionIncrease);
            }
        }
    }

    public void Awake()
    {
        listeners = new Dictionary<GameObject, HearNoise>();
    }
}
