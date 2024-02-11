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
    private void CreateParticle()
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = Random.insideUnitSphere * fireSpawnRadius;

        Particle p = gameObject.GetComponent<Particle>();
        p.InitialiseParticles(transform.position, MaximumSize, MinnimumSize, initialLifeTime);

        p.transform.SetParent(transform);
        particles.Add(p);
    }
    void UpdateParticleLifetime()
    {
        foreach (Particle p in particles )
        {
            p.lifeTime -= Time.deltaTime;

            if (p.lifeTime <= 0f)
            {
                // Recycle or respawn the particle
                SpawnParticles();
            }
        }
    }
    void UpdateParticleVisuals()
    {
        foreach(Particle p in particles){
            float normalizedLifetime = 1f - p.lifeTime / initialLifeTime;

            // Update particle color and size based on normalized lifetime
            Color value = p.gradient.Evaluate(normalizedLifetime);
            p.Colour = value;
            p.size = Mathf.Lerp(MinnimumSize, MaximumSize, normalizedLifetime);
        }
    }
    void Start()
    {
        particles = new List<Particle>();
        
        SpawnParticles();
    }
    void RemoveExpiredParticles()
    {
        // Use a separate list to avoid modifying the original list while iterating
        List<Particle> particlesToRemove = new List<Particle>();

        foreach (Particle particle in particles)
        {
            // Check if the particle's lifetime has expired
            if (particles.Count >= 100)
            {
                particlesToRemove.Add(particle);
            }
        }

        // Remove or deactivate the expired particles
        foreach (Particle particleToRemove in particlesToRemove)
        {
            particles.Remove(particleToRemove);
            Destroy(particleToRemove.gameObject); // Or deactivate the game object
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticleVisuals();
        UpdateParticleMovement();
        UpdateParticleLifetime();
        //RemoveExpiredParticles();
    }
}
