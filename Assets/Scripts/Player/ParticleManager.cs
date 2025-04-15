using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager {
    ParticleSystem dustParticles;
    ParticleSystem boostParticles;
    ParticleSystem.MainModule boostMain;

    // Start is called before the first frame update
    public ParticleManager() {
        dustParticles = GameObject.Find("DustParticles").GetComponent<ParticleSystem>();
        boostParticles = GameObject.Find("BoostParticles").GetComponent<ParticleSystem>();
        boostMain = boostParticles.main;
        boostParticles.Stop();
    }

    public void Dust() {
        dustParticles.Play();
    }

    public void BoostOn(float velocityX) {
        boostMain.startSpeedMultiplier = velocityX / 4;
        if (!boostParticles.isPlaying) {
            boostParticles.Play();
        }
    }

    public void BoostOff() {
        boostParticles.Stop();
    }
}
