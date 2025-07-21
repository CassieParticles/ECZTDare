using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class KillBox : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 3f;
    [SerializeField] private float pulseSpeed = 1f;

    private Light2D glowLight;
    private float time = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {

       
        if (collision.CompareTag("Player"))
        {
        
            FindFirstObjectByType<MenuScript>().Lose();
            
        }
    }
    private void Start()
    {
        glowLight = GetComponent<Light2D>();   
    }

    void Update()
    {
        AdjustLightIntensity();
    }

    void AdjustLightIntensity()
    {
        time += Time.deltaTime;
        // Calculate the sine wave for intensity
        float sineValue = Mathf.Sin(time * pulseSpeed);
        // Remap the sine wave to the desired intensity range
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (sineValue + 1f) / 2f);
        glowLight.intensity = intensity;
    }
}

