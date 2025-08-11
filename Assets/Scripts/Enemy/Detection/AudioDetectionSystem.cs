using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioSource
{
    Player,
    Hacked
}

public class AudioDetectionSystem : MonoBehaviour
{
    public delegate void HearNoise(Vector3 soundLocation, float suspicionIncrease, AudioSource source);

    private Dictionary<GameObject, HearNoise> listeners;

    [SerializeField] private GameObject SoundVisualizeCirclePrefab;

    private List<GameObject> soundCirclePool;
    private readonly int poolSize = 5;

    private GameObject GetInactiveCircle()
    {
        for(int i=0;i<poolSize;++i)
        {
            if (!soundCirclePool[i].activeSelf)
            {
                return soundCirclePool[i];
            }
        }
        Debug.LogError("ERROR: INSUFFICIENT POOL SIZE, INCREASE SIZE OF POOL");
        return null;
    }

    public static AudioDetectionSystem getAudioSystem()
    {
        if(FindAnyObjectByType<AudioDetectionSystem>())
        {
            return FindAnyObjectByType<AudioDetectionSystem>();
        }
        return null;
    }

    public void AddListener(GameObject gameObject, HearNoise listenerFunc)
    {
        listeners.Add(gameObject,listenerFunc);
    }

    public void PlaySound(Vector3 noiseLocation, float noiseRadius, float suspicionIncrease,AudioSource source)
    {
        GameObject soundCircle = GetInactiveCircle();
        if (!soundCircle){ return; }


        soundCircle.transform.position = noiseLocation;
        soundCircle.GetComponent<CreateCircle>().StartCircle(noiseRadius);


        foreach (KeyValuePair<GameObject, HearNoise> listener in listeners)
        {
            //If gameObject is within range to hear noise
            if (noiseRadius * noiseRadius > (noiseLocation - listener.Key.transform.position).sqrMagnitude)
            {
                listener.Value(noiseLocation,suspicionIncrease,source);
            }
        }
    }

    public void Awake()
    {
        listeners = new Dictionary<GameObject, HearNoise>();
        soundCirclePool = new List<GameObject>(poolSize);

        for(int i=0;i<poolSize;++i)
        {
            soundCirclePool.Add(Instantiate(SoundVisualizeCirclePrefab));
            soundCirclePool[i].SetActive(false);
        }
    }
}
