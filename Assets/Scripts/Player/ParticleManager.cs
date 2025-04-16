using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager {
    ParticleSystem dustParticles;
    ParticleSystem boostParticles;
    ParticleSystem.MainModule boostMain;
    ParticleSystem cloakParticles;
    ParticleSystem.MainModule cloakMain;
    ParticleSystem.VelocityOverLifetimeModule cloakVelocity;

    // Start is called before the first frame update
    public ParticleManager() {
        dustParticles = GameObject.Find("DustParticles").GetComponent<ParticleSystem>();
        boostParticles = GameObject.Find("BoostParticles").GetComponent<ParticleSystem>();
        cloakParticles = GameObject.Find("CloakParticles").GetComponent<ParticleSystem>();
        boostMain = boostParticles.main;
        cloakMain = cloakParticles.main;
        cloakVelocity = cloakParticles.velocityOverLifetime;
        dustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        boostParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloakParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Dust() {
        dustParticles.Play();
    }

    public void WhileBoosting(float velocityX) {
        boostMain.startSpeedMultiplier = velocityX / 4;
        if (!boostParticles.isPlaying) {
            boostParticles.Play();
        }
    }

    public void BoostOff() {
        boostParticles.Stop();
    }

    public void CloakOn() {
        cloakParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloakVelocity.yMultiplier = 18;
        cloakMain.startSpeed = -0.5f;
        cloakParticles.transform.localPosition = new Vector3(0, 1.2f, 0);
        cloakParticles.Play();
    }

    public void CloakOff() {
        cloakParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloakVelocity.yMultiplier = -18;
        cloakMain.startSpeed = 0.5f;
        cloakParticles.transform.localPosition = new Vector3(0, -1.2f, 0);
        cloakParticles.Play();

    }
}
