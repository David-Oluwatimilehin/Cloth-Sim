using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] int NumberOfParticles;
    [SerializeField] float MaximumSize;
    [SerializeField] float MinnimumSize;
    [SerializeField] float initialLifeTime;
    [SerializeField] float fireSpawnRadius;

    //public GameObject prefab;
    List<Particle> particles;
    public GameObject prefab;
    
    
    private void SpawnParticles()
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = Random.insideUnitSphere * fireSpawnRadius;
        
        Particle p = gameObject.GetComponent<Particle>();
        p.InitialiseParticles(transform.position, MaximumSize, MinnimumSize, initialLifeTime);
        
        p.transform.SetParent(transform);
        particles.Add(p);

        if (particles.Count < NumberOfParticles)
        {
            Invoke("SpawnParticles", 0.05f);
        }
    }

    void UpdateParticleMovement()
    {
        foreach (var p in particles)
        {
            p.Velocity += Vector3.up * 2.5f * Time.deltaTime;
            p.Position += p.Velocity * Time.deltaTime;
        }

        /*for (int i = 0; i <= NumberOfParticles; i++)
        {
            particles[i].Velocity += Vector3.up * 2.5f * Time.deltaTime;
            particles[i].Position += particles[i].Velocity * Time.deltaTime;
        }*/
    }

    void UpdateParticleLifetime()
    {
        for (int i = 0; i < NumberOfParticles; ++i)
        {
            particles[i].lifeTime -= Time.deltaTime;

            if (particles[i].lifeTime <= 0f)
            {
                // Recycle or respawn the particle
                SpawnParticles();
            }
        }
    }
    void UpdateParticleVisuals()
    {
        for (int i = 0; i < NumberOfParticles; ++i)
        {
            float normalizedLifetime = 1f - particles[i].lifeTime / initialLifeTime;

            // Update particle color and size based on normalized lifetime
            particles[i].Colour = particles[i].gradient.Evaluate(normalizedLifetime);
            particles[i].size = Mathf.Lerp(MinnimumSize, MaximumSize, normalizedLifetime);
        }
    }
    void Start()
    {
        particles = new List<Particle>();
        
        SpawnParticles();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticleVisuals();
        UpdateParticleMovement();
        UpdateParticleLifetime();
    }
}
