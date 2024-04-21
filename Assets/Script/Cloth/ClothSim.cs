
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] public float springConstant=0.2f;
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
    private List<DiagonalSpring> diagSpringList;
    private List<BendingSpring> bendingSpringList;


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
        diagSpringList = new List<DiagonalSpring>();
        bendingSpringList = new List<BendingSpring>();


        particleSpawnPosition = new Vector2(0, 0);
        
        // Set Initial Forces to Zero
        changeDir= Vector2.zero;
        netForce=Vector2.zero;
        WindForce = Vector2.zero;
        GravityForce = Vector2.zero; 
        AirResistanceForce= Vector2.zero;
        sumForces = Vector2.zero;
        DragForce = Vector2.zero;



        particleColour = new Color(250, 0, 0);
        SetupPoints();
        

    }
   

    void SetupPoints()
    {

        for (int y = 0; y <= rows; y++){
            for (int x = 0; x <= columns; x++){

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

                    /*if (x < columns - 1)
                    {
                        BendingSpring bendingSping = new BendingSpring(particle, particleList[y * (rows + 1) + (x + 1)], particleList[y * (rows + 1) + (x + 2)], Mathf.PI);
                        bendingSpringList.Add(bendingSping);
                    }*/
                    
                }
                if (x > 0 && y > 0)
                {
                    // Top-left diagonal connectors
                    DiagonalSpring topLeftSpring = new DiagonalSpring(particle, particleList[(y - 1) *
                          (rows + 1) + (x - 1)], Mathf.Sqrt(2) * spacing);
                    diagSpringList.Add(topLeftSpring);
                }

                


                // Sets up Y Connectors
                if (y != 0) {
                    
                    Particle connectedParticle = particleList[(y - 1) * (rows + 1) + x];
                    Spring spring = new Spring(particle, connectedParticle,spacing);

                    spring.startParticle.position = particleSpawnPosition;

                    springList.Add(spring);
                    particle.connectedSprings.Add(spring);

                    /*if (y < rows - 1)
                    {
                        BendingSpring bendingSping = new BendingSpring(particle, particleList[(y + 1) * (rows + 1) + x], particleList[(y + 2) * (rows + 1) + x], Mathf.PI);
                        bendingSpringList.Add(bendingSping);
                    }*/

                }

                if (x < columns && y > 0)
                {
                    // Top-right diagonal connectors
                    DiagonalSpring topRightSpring = new DiagonalSpring(particle, particleList[(y - 1) *
                         (rows + 1) + (x + 1)], Mathf.Sqrt(2) * spacing);
                    diagSpringList.Add(topRightSpring);
                }

                

                if (x == 0 && y == 0 || y == rows && x == 0 || y == rows/2 && x == 0)
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
        ComputeDiagConstraints();
        //ComputeBendConstraints();

        DrawSpringConnectors();

    }
    void ComputeBendConstraints()
    {
        foreach (var bendingSpring in bendingSpringList)
        {
            Vector2 dirAB = bendingSpring.particleOne.position - bendingSpring.particleTwo.position;
            Vector2 dirCB = bendingSpring.particleThree.position - bendingSpring.particleTwo.position;

            float angle= Vector2.SignedAngle(dirAB,dirCB);
            float error = Mathf.Abs(angle - bendingSpring.restAngle);

            Vector2 forceAB = springConstant * error * dirAB.normalized;
            Vector2 forceCB = springConstant * error * dirCB.normalized;

            bendingSpring.particleOne.position += forceAB;
            bendingSpring.particleThree.position += forceCB;
        }
    }
    void UpdateVerletBody(Particle particle)
    {
        if (!particle.isPinned)
        {
            netForce = CalcForces(particle);

            netForce += new Vector2(0, particle.gravity * particle.mass);

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
            WindForce = new Vector2(0.5f * airDensity * windSpeed * windSpeed *
                (Mathf.PI * particleSize / 2 * particleSize / 2) * dragCooeficient, 0);
            
            WindForce = WindForce.magnitude * new Vector2(Random.Range(0, MaxInclusive), 0);
        }
        else
        {
            WindForce = Vector2.zero;
        }

        AirResistanceForce = -airResistanceDragCoef * p.velocity.magnitude * p.velocity;

        sumForces = GravityForce + WindForce + AirResistanceForce;
        
        foreach (var spring in p.connectedSprings)
        {
            float currentLength= (p.position - spring.linkedParticle.position).magnitude;
            float error = Mathf.Abs(currentLength-spring.restLength);
            Vector2 springDir = (spring.linkedParticle.position - p.position).normalized;
            Vector2 springForce = springConstant * error * springDir;

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
    private void ComputeDiagConstraints()
    {
        foreach (var diagonalSpring in diagSpringList)
        {
            Vector2 delta = diagonalSpring.particleTwo.position - diagonalSpring.particleOne.position;
            
            float dist = delta.magnitude;
            float error = Mathf.Abs(dist - diagonalSpring.restlength);
            
            Vector2 dir = delta.normalized;
            Vector2 force = springConstant * error * dir;

            diagonalSpring.particleOne.velocity += force * 0.5f;
            diagonalSpring.particleTwo.velocity -= force * 0.5f;
        }
    }

    private void DrawSpringConnectors()
    {
        foreach (var spring in springList)
        {

            if (spring.startParticle != null && spring.linkedParticle != null)
            {

                if (spring.isEnabled)
                {
                    Debug.DrawLine(spring.startParticle.position, spring.linkedParticle.position, Color.white);
                }
            }
        }

        foreach (var bendSpring in bendingSpringList)
        {

            if (bendSpring.particleOne != null && bendSpring.particleThree != null)
            {
                Debug.DrawLine(bendSpring.particleOne.position, bendSpring.particleThree.position, Color.green);
            }
        }

        /*foreach (var diag in diagSpringList)
        {

            if (diag.particleOne != null && diag.particleTwo != null)
            {

                if (diag.isEnabled)
                {
                    Debug.DrawLine(diag.particleOne.position, diag.particleTwo.position, Color.white);
                }
            }
        }*/



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

