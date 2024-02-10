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
    
    void Start()
    {
        particles = new List<Particle>();
        SpawnParticles();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
