
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public Vector3 emitterVelocity;

    

    public List<FlameParticle> particleList;
    public GameObject particlePrefab;
    
    public int nrAlive;

    void Start()
    {
        cam = Camera.main;

        particleList = new List<FlameParticle>();

        ParticleInitialisation();
    }
    public Vector3 ComputeVelocity()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 normal = spawnArea.position.normalized;
        
        velocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(minSpeed, maxSpeed), 0);
        velocity *= normal.magnitude;

        return velocity;
    } 
    private void ParticleInitialisation()
    {
        for (int i=0; i<numberOfParticles; i++)
        {
            //particlesList.pos = new Vector3(spawnSphere.x, spawnSphere.y, Random.Range(-1, 1));
            //spawnSphere = new Vector3(Random.Range(spawnSphere.x, spawnSphere.y), Random.Range(-1, 1));

            spawnSphere = spawnArea.position;
            spawnSphere += Random.insideUnitSphere * spawnRadius;
            

            particlePrefab = Instantiate(particlePrefab, new Vector3(spawnSphere.x, spawnSphere.y, spawnSphere.z), Quaternion.identity);
            particlePrefab.name = "Particle: " + (i + 1);
            particlePrefab.transform.SetParent(spawnArea, false);

            
            particlePrefab.GetComponent<FlameParticle>().lifetime = particleLifetime;
            particlePrefab.GetComponent<FlameParticle>().size = particleSize;
            

            particleList.Add(particlePrefab.GetComponent<FlameParticle>());

            particleList[i].pos = spawnSphere;
            particleList[i].vel = ComputeVelocity();       
            
            nrAlive++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxExtents);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnArea.position, spawnRadius);

        if (particleList != null)
        {
            Gizmos.color=Color.red;
            foreach (var particle in particleList)
            {
                Gizmos.DrawSphere(particle.pos,particle.size);
            }
        }
        
    }
    void Update()
    {

        foreach (var particle in particleList)
        {
            if (particlePrefab != null)
            {
                particle.lifetime -= Time.time;
            }
        }



    }
    private void FixedUpdate()
    {
        for (int i = 0; i < particleList.Count; i++)
        {
            particleList[i].pos += particleList[i].vel * Time.fixedDeltaTime;

        }


    }


    void LateUpdate()
    {
        Vector3 newRotation = cam.transform.eulerAngles;

        newRotation.x = 0;
        newRotation.z = 0;

        transform.eulerAngles = newRotation;
    }

}
