
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FlameSystem : MonoBehaviour
{
    private Camera cam;
        
    [SerializeField]public Vector3 boxExtents;
    private Vector3 spawnSphere;

    public Transform t;
    public Transform spawnArea;

    
    [Header("Particle Information")]
    
    public float minSpeed;
    public float maxSpeed;
    public float particleLifetime;
    
    public float spawnRadius;
    public float particleSize;
    public int numberOfParticles;

    public float emissionRate = 10;
    public float nextEmissionTime = 0.0f;

    public List<FlameParticle> particleList;
    public GameObject particlePrefab;
    
    public int nrAlive;
    public Gradient gradient;
    public int maxNum;

    private Vector3 gravity;
    private Vector3 velocity;
    private Vector3 normal;
    private float sizeRatio;
    
    void Start()
    {
        particleList = new List<FlameParticle>();
        gravity = new Vector3(0, 0.981f, 0);
        
        ParticleInitialisation();       
    }
    public Vector3 ComputeVelocity()
    {
        velocity = Vector3.zero;
        normal = spawnArea.position.normalized;
        
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(minSpeed, maxSpeed), 0/*Random.Range(-0.5f, 0.5f)*/);
        velocity *= normal.magnitude;

        velocity += gravity;

        return velocity;
    } 
    private void ParticleInitialisation()
    {
        for (int i=0; i<maxNum; i++)
        {
            //particlesList.pos = new Vector3(spawnSphere.x, spawnSphere.y, Random.Range(-1, 1));
            //spawnSphere = new Vector3(Random.Range(spawnSphere.x, spawnSphere.y), Random.Range(-1, 1));

            spawnSphere = spawnArea.position+ Random.insideUnitSphere * spawnRadius;
            
            
            particlePrefab = Instantiate(particlePrefab, spawnSphere, Quaternion.identity);
            particlePrefab.name = "Particle: " + (i + 1);
            particlePrefab.transform.SetParent(spawnArea, false);


            particlePrefab.GetComponent<FlameParticle>().isEnabled = false;
            particlePrefab.GetComponent<FlameParticle>().lifetime = particleLifetime;
            particlePrefab.GetComponent<FlameParticle>().size = particleSize;
            particlePrefab.GetComponent<FlameParticle>().oldPos = spawnSphere;
            particlePrefab.GetComponent<FlameParticle>().pos = particlePrefab.GetComponent<FlameParticle>().oldPos;
            
            particlePrefab.GetComponent<FlameParticle>().vel = Vector3.zero;

            particleList.Add(particlePrefab.GetComponent<FlameParticle>());
            
            if(i < numberOfParticles)
            {
                particleList[i].isEnabled = true;
                //particleList[i].pos = spawnSphere;
                particleList[i].vel = ComputeVelocity();
                nrAlive++;
            }
                     
        }
    }
    private void ResetParticlePositions(FlameParticle particle)
    {
        particle.lifetime = particleLifetime;
        particle.size = particleSize;
        particle.pos = spawnArea.position + spawnRadius * Random.insideUnitSphere;
        particle.vel = ComputeVelocity();
        particle.isEnabled = false;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxExtents);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnArea.position, spawnRadius);


        foreach (var particle in particleList)
        {
            Gizmos.color = particle.colour;
            if (particle.isEnabled)
            {
                Gizmos.DrawSphere(particle.pos, particle.size);
            }
        }

    }
    void Update()
    {
        //while (Time.time >= nextEmissionTime)
        //{

        //     EmitParticles();

        //    nextEmissionTime += 1f / emissionRate; // Increment next emission time
        //}
        
        if (Time.time >= nextEmissionTime)
        {
            EmitParticles();
            nextEmissionTime = Time.time + 1f / emissionRate; // Calculate next emission time
        }

        foreach (var particle in particleList)
        {
            if (particle.isEnabled)
            {
                particle.lifetime -= Time.deltaTime;
                sizeRatio = particle.lifetime / particleLifetime;

                particle.size = Mathf.Clamp(particle.size, particle.size * sizeRatio, 0.05f);
                particle.colour = gradient.Evaluate(particleLifetime - particle.lifetime);

            }
        }
        

    }
    private void FixedUpdate()
    {
        foreach (var flameParticle in particleList)
        {
            if (flameParticle.lifetime <= 0)
            {
                ResetParticlePositions(flameParticle);
            }
            else 
            {
                if (flameParticle.isEnabled)
                {
                    flameParticle.pos += flameParticle.vel * Time.fixedDeltaTime;
                }
                
            }
        }
    }

    private void EmitParticles()
    {
        for(int i = 0; i<particleList.Count/2; i++) {
            
            if (!particleList[i].isEnabled) {

                particleList[i].isEnabled = true;
                particleList[i].lifetime = particleLifetime;
                particleList[i].size = particleSize;

                //Debug.LogError("Function Called");

                particleList[i].pos = spawnArea.position + spawnRadius * Random.insideUnitSphere;
                particleList[i].vel = ComputeVelocity();

            }
            

        }
        //for(int i=0; i<particleList.Count; i++)
        //{
        //    //Debug.Log("Function Called " + i + " time");
        //    for(int j=0; j<numberOfParticles; j++)
        //    {
        //        if (!particleList[i].enabled)
        //        {
        //            particleList[i].enabled = true;
        //            particleList[i].lifetime = particleLifetime;
        //            particleList[i].size = particleSize;
        //            particleList[i].pos = spawnArea.position + Random.insideUnitSphere * spawnRadius;
        //            particleList[i].vel = ComputeVelocity();
                    
        //            nrAlive++;
        //        }
        //    }
        //}
    }

    void LateUpdate()
    {
        //Vector3 newRotation = cam.transform.eulerAngles;

        //newRotation.x = 0;
        //newRotation.z = 0;

        //transform.eulerAngles = newRotation;
    }

}
