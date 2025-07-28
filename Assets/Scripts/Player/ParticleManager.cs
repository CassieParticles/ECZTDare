using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager {
    ParticleSystem dustParticles;
    ParticleSystem cloakParticles;
    ParticleSystem.MainModule cloakMain;
    ParticleSystem.VelocityOverLifetimeModule cloakVelocity;
    Afterimages afterimages;

    // Start is called before the first frame update
    public ParticleManager() {
        dustParticles = GameObject.Find("DustParticles").GetComponent<ParticleSystem>();
        cloakParticles = GameObject.Find("CloakParticles").GetComponent<ParticleSystem>();
        afterimages = GameObject.Find("DashAfterimages").GetComponent<Afterimages>();
        cloakMain = cloakParticles.main;
        cloakVelocity = cloakParticles.velocityOverLifetime;
        dustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cloakParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Dust() {
        dustParticles.Play();
    }

    public void Afterimages(float dashDuration, int dashDir) {
        afterimages.StartDash(dashDuration, dashDir);
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
