using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float respawnTime;
    public ParticleSystem baseParticle;
    public ParticleSystem boostParticle;

    private float timeBeforeRepawn;
    private bool isActive;

    private void Start()
    {
        isActive = true;
    }

    private void Update()
    {
        if(timeBeforeRepawn > 0)
        {
            timeBeforeRepawn -= Time.deltaTime;
        }
        else if(!isActive)
        {
            Appear();
        }
    }

    private void Appear()
    {
        baseParticle.Play();
        isActive = true;
    }

    private void Boost()
    {
        baseParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        isActive = false;
        boostParticle.Play();
        timeBeforeRepawn = respawnTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(isActive)
        {
            FishBehaviour potentialFish = collider.GetComponent<FishBehaviour>();
            if (potentialFish != null && potentialFish.isControlled)
            {
                potentialFish.schoolHandler.Boost();
                Boost();
            }
        }
    }
}
