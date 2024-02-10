using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private SphereCollider col;

    public Vector3 Position;
    public Vector3 Velocity;
    public Color Colour;

    public float lifeTime;
    public float size;
    float buoyancy = 2.5f;
    
    public void InitialiseParticles(Vector3 spawnPosition,float MaxSize,float MinSize, float initLifeTime)
    {
        Position = spawnPosition;
        Velocity = new Vector3(Random.Range(-1f, 1f), buoyancy + Random.Range(1f, 2f), Random.Range(-1f, 1f));
        Colour = Color.Lerp(Color.yellow, Color.red, Random.value);

        Renderer[] rends = GetComponents<Renderer>();
        foreach (Renderer r in rends)
        {
            r.material.color = Colour;

        }

        size = Random.Range(MinSize, MaxSize);
        col=GetComponent<SphereCollider>();
        col.radius = size + 0.05f;

        lifeTime = initLifeTime;
    }
    void Awake()
    {
        
        
        
    }

}
