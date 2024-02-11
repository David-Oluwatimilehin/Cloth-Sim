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
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.red, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 1.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        size = Random.Range(MinSize, MaxSize);
        

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
