
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class ClothSim : MonoBehaviour
{
    private const float MaxInclusive = 30f;

    // Methods for determining wind should be globbally varying constant function
    // ISSUE: The current wind function doesnt reset;
    // Do (sin (x*y*t), cos(z*t), sin(cos(5*x*y*z) ) ) For the Wind Vector

    // TODO: Change Mesh to be skinnedMesh attack bpnes for the connectors.
    // TODO: Figure out how to deform the mesh
    // TODO: REMOVE the pinned particles so that only the first and last particles of the row are pinned.
    // TODO: Keep in mind 

    // Uses Verlet Intergration with velocity
    // Computed by taking the previous positions in account with the current and previous timestep
    [Header("Cloth Attributes")]
    [SerializeField] public int rows = 48;
    [SerializeField] public int columns = 48;
    [SerializeField] public float spacing = 1.0f;
    [SerializeField] public float stiffness=0.2f;
    private Mesh clothMesh;


    [Header("Wind Attributes")]
    [SerializeField] public bool simulateWind;
    [SerializeField] public float airDensity;
    [SerializeField] public float airResistanceDragCoef;
    [SerializeField] public float windSpeed;
    [SerializeField] public float dragCooeficient;
    //float springConstant = 2f;

    // 
    private Vector3[] clothVertices;
    public List<Particle> particleList;
    private List<Spring> springList;

    // 
    private Vector2 netForce;
    private Vector2 changeDir;
    private Vector2 WindForce;
    private Vector2 DragForce;
    private Vector2 sumForces;
    private Vector2 GravityForce;
    private Vector2 AirResistanceForce;


    [Header("Particle Attributes")]
    public Transform t;
    public Color particleColour;
    public Material connectorMaterial;
    [SerializeField] public float particleSize;

    Vector2 particleSpawnPosition;
    
    void Start()
    {
        // Initializes the Lists
        particleList = new List<Particle>();
        springList = new List<Spring>();
        

        particleSpawnPosition = new Vector2(0, 0);
        
        // Set Initial Forces to Zero
        changeDir= Vector2.zero;
        netForce=Vector2.zero;
        WindForce = Vector2.zero;
        GravityForce = Vector2.zero; 
        AirResistanceForce= Vector2.zero;
        sumForces = Vector2.zero;
        DragForce = Vector2.zero;
        particleColour = new Color(255, 0, 0);
        
        
        SetupPoints();
        

    }
   

    void SetupPoints()
    {

        for (int y = 0; y <= rows; y++)
        {
            for (int x = 0; x <= columns; x++)
            {
                    /* Creates the Partcles that will be added to the list*/
                    Particle particle = new Particle();
                    particle.pinPos = new Vector2(particleSpawnPosition.y, particleSpawnPosition.x);
                    

                    // Sets up X Connectors
                    if (x != 0)
                    {

                        Particle connectedParticle = particleList[particleList.Count - 1];
                        Spring spring = new Spring(particle, connectedParticle, spacing);

                        spring.startParticle.position = particleSpawnPosition;
                       

                        springList.Add(spring);
                        particle.connectedSprings.Add(spring);

                        /*horizontalConnector.lineRender = line;
                        //horizontalConnector.lineRender.material = connectorMaterial;

                        if (y != 0)
                        { 
                            //CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x - 1)], particle, particleList[(y - 1) * (rows + 1) + (x - 1)], Mathf.Sqrt(2) * spacing);
                        }*/

                }

                    // Sets up Y Connectors
                    if (y != 0) { 
                                           

                         Particle connectedParticle = particleList[(y - 1) * (rows + 1) + x];
                         Spring spring = new Spring(particle, connectedParticle,spacing);
                         
                         spring.startParticle.position= particleSpawnPosition;
                         
                         
                         springList.Add(spring);    
                         particle.connectedSprings.Add(spring);

                        /*if (x != 0)
                        {
                            // Top-right diagonal connectors
                            //CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x + 1)], particle, particleList[(y - 1) * (rows + 1) + (x + 1)], Mathf.Sqrt(2) * spacing);
                        }
                        if (x != columns)
                        {
                            // Top-left diagonal connectors
                            //CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x + 1)], particle, particleList[(y - 1) * (rows + 1) + (x + 1)],Mathf.Sqrt(2) * spacing);
                        }*/
                    }

                    if(x == 0)
                    {
                        particle.isPinned = true;
                    }
                    particleSpawnPosition.x -= spacing;

                    particleList.Add(particle);

            }

            particleSpawnPosition.x = 0;
            particleSpawnPosition.y -= spacing;

        }
    }
    private void GenerateVertices()
    {
       
        GetComponent<MeshFilter>().mesh = clothMesh = new Mesh();
        clothMesh.name = "Procedural Cloth";

        clothVertices = new Vector3[(rows + 1) * (columns + 1)];
        for (int i = 0, y = 0; y <= columns; y++)
        {
            for (int x = 0; x <= rows; x++, i++)
            {
                clothVertices[i] = new Vector3(-x, -y);
            }
        }
        clothMesh.vertices = clothVertices;

        int[] triangles = new int[rows * columns * 6];
        for (int ti = 0, vi = 0, y = 0; y < columns; y++, vi++)
        {
            for (int x = 0; x < rows; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + rows + 1;
                triangles[ti + 5] = vi + rows + 2;
                
            }
        }
        clothMesh.triangles = triangles;
        

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        PhysicsLoop();
    }
    void Update()
    {
        
        
    }

    
    void PhysicsLoop()
    {
        foreach (Particle p in particleList)
        {
            UpdateVerletBody(p);

            ComputeConstraints(p);

        }

        DrawSpringConnectors();

    }
    void UpdateVerletBody(Particle particle)
    {
        if (!particle.isPinned)
        {
            netForce = CalcForces(particle);

            particle.velocity = (particle.position - particle.oldPos) * particle.friction +
                netForce / particle.mass * Time.fixedDeltaTime;

            DragForce = new Vector2((particle.mass * particle.velocity.x * particle.velocity.x / particleSize * particleSize) / 2,
            (particle.mass * particle.velocity.y * particle.velocity.y / particleSize * particleSize) / 2).normalized;

            DragForce = DragForce * particle.velocity.magnitude * dragCooeficient;
            particle.velocity -= DragForce;

            /*float dragCoef = netForce.magnitude;

            dragCoef = dragCooeficient * dragCoef + (dragCoef * dragCoef) * dragCooeficient * dragCooeficient;

            netForce= netForce.normalized;
            netForce *= -dragCoef;*/

            particle.oldPos = particle.position;
            particle.position += particle.velocity * particle.dampValue;
            //particleList[p].position.x += SinWindFunc(particleList[p]) * Time.fixedDeltaTime;
            //particleList[p].position.x += particleList[p].mass*ApplyWind() * Time.fixedDeltaTime;
            //particleList[p].position.y += particleList[p].gravity * Time.fixedDeltaTime;
        }
        else
        {
            particle.position = particle.pinPos;
            particle.oldPos = particle.pinPos;
        }
    }


    Vector2 CalcForces(Particle p)
    {
        GravityForce = new Vector2(0, p.gravity * p.mass);
        
        if (simulateWind)
        {
            WindForce = Vector2.zero;

            WindForce = new Vector2(0.5f * airDensity * windSpeed * windSpeed *
                (Mathf.PI * particleSize / 2 * particleSize / 2) * dragCooeficient, 0);
            
            WindForce = WindForce.magnitude * new Vector2(Random.Range(0, MaxInclusive), 0);
        }

        AirResistanceForce = -airResistanceDragCoef * p.velocity.magnitude * p.velocity;

        sumForces = GravityForce + WindForce + AirResistanceForce;
        
        foreach (var spring in p.connectedSprings)
        {
            float currentLength= (p.position - spring.linkedParticle.position).magnitude;
            float error = Mathf.Abs(currentLength-spring.restLength);
            Vector2 springDir = (spring.linkedParticle.position - p.position).normalized;
            Vector2 springForce = stiffness * error * springDir;

            p.velocity += springForce;
            spring.linkedParticle.velocity -= springForce;
        }            
        
        return sumForces;
    }
    private void ComputeConstraints(Particle p)
    {
        foreach (var connector in p.connectedSprings)
        {
            if (connector.isEnabled)
            {
                float dist = (connector.startParticle.position - connector.linkedParticle.position).magnitude;
                float error = Mathf.Abs(dist - connector.restLength);

                if (dist > connector.restLength)
                {
                    changeDir = (connector.startParticle.position - connector.linkedParticle.position).normalized;

                }
                else if (dist < connector.restLength)
                {
                    changeDir = (connector.linkedParticle.position - connector.startParticle.position).normalized;
                }

                Vector2 changeAmount = changeDir * error;

                connector.startParticle.position -= changeAmount * 0.5f;
                connector.linkedParticle.position += changeAmount * 0.5f;
            }

        }
    }

    private void DrawSpringConnectors()
    {
        foreach(Spring spring in springList)
        {
            if(spring.startParticle!=null && spring.linkedParticle != null)
            {
                if (spring.isEnabled)
                {
                    Debug.DrawLine(spring.startParticle.position, spring.linkedParticle.position,Color.white);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = particleColour;
        if (particleList != null){
            

            for(int i=0; i<particleList.Count; ++i)
            {
                Gizmos.DrawSphere(particleList[i].position, particleSize);
            }
        }
    }

}

