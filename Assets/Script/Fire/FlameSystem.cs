
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameSystem : MonoBehaviour
{
    public Camera cam;

    [SerializeField]
    public Texture2D texture;
    [SerializeField]
    public int numberOfParticles;

    [SerializeField]
    public Vector3 boxExtents;
    [SerializeField]
    public Vector2 spawnSphere;


    [SerializeField]
    public Vector3 minSpeed;
    [SerializeField]
    public Vector3 maxSpeed;

    public BlendShapeBufferRange[] blendShapes;

    private FlameParticle[] particles;

    
    public int nrAlive;

    void Start()
    {
        cam = Camera.main;
        
        particles = new FlameParticle[numberOfParticles];

        ParticleInitialisation();
    }

    private void ParticleInitialisation()
    {
        for(int i = 0; i < numberOfParticles; i++)
        {
            particles[i] = new FlameParticle();
            
            particles[i].pos = new Vector3(spawnSphere.x,spawnSphere.y, Random.Range(-1,1));
            particles[i].size = Random.Range(0.01f, 0.05f);
            particles[i].color = Color.red;
            
            particles[i].vel = new Vector3(Random.Range(minSpeed.x,maxSpeed.x), Random.Range(minSpeed.y, maxSpeed.y), Random.Range(minSpeed.z, maxSpeed.z));
            

            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxExtents);
        
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(spawnSphere, 0.5f);

        if (particles != null)
        {
            Gizmos.color=Color.red;
            for (int i=0; i<particles.Length; i++)
            {   
                
                Gizmos.DrawSphere(particles[i].pos, particles[i].size);
            }
        }
        
    }
    void Update()
    {
        
        for (int i=0; i<particles.Length; i++)
        {
            particles[i].pos += particles[i].vel * Time.deltaTime;

        }



    }
    private void FixedUpdate()
    {

        

    }


    void LateUpdate()
    {
        transform.LookAt(cam.transform);

        transform.Rotate(0, 180, 0);
    }

}
