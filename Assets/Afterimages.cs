using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimages : MonoBehaviour
{
    private ParticleSystem afterimage;
    ParticleSystem.MainModule particle;
    ParticleSystemRenderer particleRenderer;
    ParticleSystem.ShapeModule shape;

    private const float xPos = -0.218f;

    [Header("Afterimage Settings")]
    [SerializeField] Color afterimageColor = Color.green;
    [SerializeField] float lifeTime = 0.3f;

    [Header("Afterimage Spawning")]
    [SerializeField][Range(1, 15)] int copiesPerDash = 4;
    [SerializeField] bool firstCopyImmediately = true;

    public void Start() {
        afterimage = GetComponent<ParticleSystem>();
        particle = afterimage.main;
        shape = afterimage.shape;
        particleRenderer = afterimage.GetComponent<ParticleSystemRenderer>();
    }

    public void StartDash(float dashDuration, int dashDir) {
        int dashDirection = 1 - ((dashDir + 1) / 2); //convert from the range (-1 to 1) to (1 to 0) so that if the direction is reversed it flips it in the correct direction
        particleRenderer.flip = new Vector3(dashDirection, 0, 0);
        shape.position = new Vector3(xPos * dashDir, 0.675f, 0);
        StartCoroutine(DashAfterimage(dashDuration));
    }

    public IEnumerator DashAfterimage(float dashDuration) {
        float timer = 0;
        float copyTimer = 0;
        int totalCopies = copiesPerDash;
        if (firstCopyImmediately) {
            //Make copy
            afterimage.Play();
            totalCopies--;
            if (totalCopies == 0) {
                yield break;
            }
        }

        float copyInterval = dashDuration / (totalCopies + 1); //How often to make a copy
        int placedCopies = 0; //How many copies have been placed already
        while (timer < dashDuration) {
            if (copyTimer > copyInterval) {
                //Make copy
                afterimage.Play();
                copyTimer -= copyInterval;
                placedCopies++;
            }

            copyTimer += Time.deltaTime;
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnValidate() {
        afterimage = GetComponent<ParticleSystem>();
        particle = afterimage.main;
        particle.startLifetime = lifeTime;
        try {
            particle.duration = lifeTime;
        } catch (System.Exception) {
            throw;
        }

        particle.startColor = afterimageColor;
    }
}
