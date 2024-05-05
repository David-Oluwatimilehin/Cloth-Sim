
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

    public float emissionRate = 50;
    private float nextEmissionTime = 0.0f;

    public List<FlameParticle> particleList;
    public GameObject particlePrefab;
    
    public int nrAlive;
    public Gradient gradient;
    public int maxNum;
    private Vector3 gravity;
    private float sizeRatio;
    
    void Start()
    {
        particleList = new List<FlameParticle>();
        gravity = new Vector3(0, 0, 0);
        ParticleInitialisation();
        
        
    }
    public Vector3 ComputeVelocity()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 normal = spawnArea.position.normalized;
        
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(minSpeed, maxSpeed), 0);
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

            spawnSphere = spawnArea.position;
            spawnSphere += Random.insideUnitSphere * spawnRadius;
            
            particlePrefab = Instantiate(particlePrefab, new Vector3(spawnSphere.x, spawnSphere.y, spawnSphere.z), Quaternion.identity);
            particlePrefab.name = "Particle: " + (i + 1);
            particlePrefab.transform.SetParent(spawnArea, false);

            particlePrefab.GetComponent<FlameParticle>().isEnabled = false;
            particlePrefab.GetComponent<FlameParticle>().lifetime = particleLifetime;
            particlePrefab.GetComponent<FlameParticle>().size = particleSize;

            particleList.Add(particlePrefab.GetComponent<FlameParticle>());
            
            if(i < numberOfParticles)
            {
                particleList[i].isEnabled = true;
                particleList[i].pos = spawnSphere;
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
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxExtents);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnArea.position, spawnRadius);

        if (!particleList.IsUnityNull())
        {
            
            foreach (var particle in particleList)
            {
                Gizmos.color = particle.colour;
                if (particle.isEnabled)
                {
                    Gizmos.DrawSphere(particle.pos,particle.size);
                }
            }
        }

        

    }
    void Update()
    {
        while (Time.time >= nextEmissionTime)
        {
            EmitParticles();
            nextEmissionTime += 1f / emissionRate; // Increment next emission time
        }

        foreach (var particle in particleList)
        {
            if (particle.isEnabled)
            {
                particle.lifetime -= Time.deltaTime;
                sizeRatio = particle.lifetime / particleLifetime;

                particle.size = particle.size * sizeRatio;
                particle.colour = gradient.Evaluate(1 - particle.lifetime);

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
                
                flameParticle.pos += flameParticle.vel * Time.fixedDeltaTime;

            }
        }
    }

    private void EmitParticles()
    {
        Debug.Log("Function Called");
        for(int i=0; i<particleList.Count; i++)
        {
            
            if (!particleList[i].enabled)
            {

                particleList[i].lifetime = particleLifetime;
                particleList[i].size = particleSize;
                particleList[i].pos = spawnArea.position + Random.insideUnitSphere * spawnRadius;
                particleList[i].vel = ComputeVelocity();
                particleList[i].enabled = true;
                nrAlive++;
            }
        }
    }

    void LateUpdate()
    {
        //Vector3 newRotation = cam.transform.eulerAngles;

        //newRotation.x = 0;
        //newRotation.z = 0;

        //transform.eulerAngles = newRotation;
    }

}
