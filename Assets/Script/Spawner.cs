using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

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
    public IObjectPool<Particle> objectPool;

    public enum PoolType
    {
        Stack,
        LinkedList
    }

    public PoolType poolType;

    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 25;

    IObjectPool<Particle> m_Pool;

    public IObjectPool<Particle> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                if (poolType == PoolType.Stack)
                    m_Pool = new ObjectPool<Particle>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 25, maxPoolSize);
                else
                    m_Pool = new LinkedPool<Particle>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }

            return m_Pool;
        }
    }
    void OnReturnedToPool(Particle system)
    {
        system.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(Particle system)
    {
        system.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(Particle system)
    {
        Destroy(system.gameObject);
    }
    Particle CreatePooledItem()
    {
        var go = new GameObject("Pooled Object");
        go.transform.position = transform.position * Random.insideUnitCircle * fireSpawnRadius;
        
        Particle ps = go.AddComponent<Particle>();
        
        

        // This is used to return ParticleSystems to the pool when they have stopped.
        var returnToPool = go.AddComponent<Spawner>();
        returnToPool.objectPool = Pool;

        return ps;
    }
    void OnGUI()
    {
        GUILayout.Label("Pool size: " + Pool.CountInactive);
        if (GUILayout.Button("Create Particles"))
        {
            
            for (int i = 0; i < NumberOfParticles; ++i)
            {
                //var ps = Pool.Get();
                //ps.InitialiseParticles(transform.position, MaximumSize, MinnimumSize, initialLifeTime);
                //ps.transform.SetParent(transform);

            }
        }
    }

    private void SpawnParticles()
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = transform.position *Random.insideUnitCircle * fireSpawnRadius;
        
        
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
        var itemsToAdd = new List<string>();

        foreach (Particle p in particles )
        {
            p.lifeTime -= Time.deltaTime;

            if (p.lifeTime <= 0f)
            {
                
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
        //UpdateParticleVisuals();
        //UpdateParticleMovement();
        //UpdateParticleLifetime();
        //RemoveExpiredParticles();
    }
}
