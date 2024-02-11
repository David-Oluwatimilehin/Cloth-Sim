using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private SphereCollider col;
    public Gradient gradient;
    public Vector3 Position;
    public Vector3 Velocity;
    public Color Colour;
    

    // Set up color keys and alpha keys in the inspector or programmatically
    

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

        gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.red, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
        );

        size = Random.Range(MinSize, MaxSize);
        transform.localScale = Vector3.one * size;

        col=GetComponent<SphereCollider>();
        col.radius = size + 0.05f;

        lifeTime = initLifeTime;
    }
    void Update()
    {
        transform.position = Position;
        transform.localScale= Vector3.one * size;
    }
}
