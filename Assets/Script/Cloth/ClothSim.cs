
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ClothSim : MonoBehaviour
{
    private const float MaxInclusive = 30f;

    // Methods for determining wind should be globbally varying constant function
    // ISSUE: The current wind function doesnt reset; FIXED
    // ISSUE: The particle needs to be pin correctly FIXED


    // TODO: REMOVE the pinned particles so that only the first and last and Middle particles of the row are pinned.


    // Uses Verlet Intergration with velocity
    public Camera cam;

    [Header("Cloth Attributes")]
    [SerializeField] public int rows;
    [SerializeField] public int columns;
    [SerializeField] public float spacing = 1.0f;
    [SerializeField] public float springConstant=0.2f;
    [SerializeField] public bool hasStraightLine;


    private Mesh clothMesh; 
    private List<int> triangles = new List<int>(); 

    [Header("Wind Attributes")]
    [SerializeField] public bool simulateWind;
    [SerializeField] public float airDensity;
    [SerializeField] public float airResistanceDragCoef;
    [SerializeField] public float windSpeed;
    [SerializeField] public float dragCooeficient;
    

    
    public List<Particle> particleList;
    private List<Spring> springList;
    private List<DiagonalSpring> diagSpringList;
    private List<BendingSpring> bendingSpringList;


    // 
    private Vector3 netForce;
    private Vector3 changeDir;
    private Vector3 changeAmount;
    private Vector3 changeDiagDir;
    private Vector3 changeDiagAmount;
    private Vector3 WindForce;
    private Vector3 DragForce;
    private Vector3 sumForces;
    private Vector3 GravityForce;
    private Vector3 AirResistanceForce;
    public GameObject particlePrefab;

    [Header("Particle Attributes")]
    public Transform t;
    public bool showDiagConstraints;
    public bool showStructuralConstraints;
    public bool showParticles;

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
        //bendingSpringList = new List<BendingSpring>();

        particleSpawnPosition = new Vector3(t.position.x, t.position.y, t.position.z);
        cam = Camera.main;
        // Set Initial Forces to Zero
        changeDir= Vector3.zero;
        netForce= Vector3.zero;
        WindForce = Vector3.zero;
        GravityForce = Vector3.zero; 
        AirResistanceForce= Vector3.zero;
        sumForces = Vector3.zero;
        DragForce = Vector3.zero;

        
        SetupPoints();
        
        
    }

    

    void SetupPoints()
    {
        for (int y = 0; y <= rows; y++) {
            for (int x = 0; x <= columns; x++) {

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

                if (x > 0 && y > 0) {

                    // Top-left diagonal connectors
                    DiagonalSpring topLeftSpring = new DiagonalSpring(particle, particleList[(y - 1) *
                          (rows + 1) + (x - 1)], Mathf.Sqrt(2) * spacing);
                    diagSpringList.Add(topLeftSpring);
                }

                // Sets up Y Connectors
                if (y != 0) {
                    
                    Particle connectedParticle = particleList[(y - 1) * (rows + 1) + x];
                    Spring spring = new Spring(particle, connectedParticle,spacing*2);

                    spring.startParticle.position = particleSpawnPosition;

                    springList.Add(spring);
                    particle.connectedSprings.Add(spring);

                    /*if (y < rows - 1)
                    {
                        BendingSpring bendingSping = new BendingSpring(particle, particleList[(y + 1) * (rows + 1) + x], particleList[(y + 2) * (rows + 1) + x], Mathf.PI);
                        bendingSpringList.Add(bendingSping);
                    }*/
                    if (x < columns && y > 0)
                    {
                        // Top-right diagonal connectors
                        DiagonalSpring topRightSpring = new DiagonalSpring(particle, particleList[(y - 1) *
                         (rows + 1) + (x + 1)], Mathf.Sqrt(2) * spacing);
                        diagSpringList.Add(topRightSpring);
                    }

                }
                
                if (hasStraightLine)
                {
                    if (x == 0)
                    {
                        particle.isPinned = true;
                    }
                }
                else
                {
                    if (x == 0 && y == 0 || y == rows && x == 0 || y == rows / 2 && x == 0)
                    {
                        particle.isPinned = true;
                    }
                }
                

                particleSpawnPosition.x -= spacing;
                
                particleList.Add(particle);

            }

            particleSpawnPosition.x = 0;
            particleSpawnPosition.y -= spacing;

        }

        // Creates the gameobjects
        for (int i=0; i<particleList.Count; ++i)
        {
            particlePrefab.name = "Particle: " + (i);
            particlePrefab = Instantiate(particlePrefab, particleList[i].position, Quaternion.identity);
            particlePrefab.GetComponent<ParticleObject>().p = particleList[i];
            particlePrefab.GetComponent<ParticleObject>().size = particleSize;
            particlePrefab.transform.SetParent(t, true);
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
        //ComputeDiagConstraints();
            
    }

    void ComputeBendConstraints()
    {
        foreach (var bendingSpring in bendingSpringList)
        {
            Vector2 dirAB = bendingSpring.particleOne.position - bendingSpring.particleTwo.position;
            Vector2 dirCB = bendingSpring.particleThree.position - bendingSpring.particleTwo.position;

            float angle= Vector2.SignedAngle(dirAB,dirCB);
            float error = Mathf.Abs(angle - bendingSpring.restAngle);

            Vector3 forceAB = springConstant * error * dirAB.normalized;
            Vector3 forceCB = springConstant * error * dirCB.normalized;

            //bendingSpring.particleOne.position += forceAB;
            //bendingSpring.particleThree.position += forceCB;
        }
    }

    void UpdateVerletBody(Particle particle)
    {
        if (!particle.isPinned)
        {
            netForce = CalcForces(particle);

            particle.velocity = (particle.position - particle.oldPos) +
                netForce / particle.mass * Time.fixedDeltaTime;

            DragForce = Vector2.zero;

            DragForce = new Vector2((particle.mass * particle.velocity.x * particle.velocity.x / particleSize * particleSize) / 2,
            (particle.mass * particle.velocity.y * particle.velocity.y / particleSize * particleSize) / 2).normalized;

            DragForce = DragForce * particle.velocity.magnitude * dragCooeficient;
            particle.velocity -= DragForce;

            /*float dragCoef = netForce.magnitude;

            dragCoef = dragCooeficient * dragCoef + (dragCoef * dragCoef) * dragCooeficient * dragCooeficient;

            netForce= netForce.normalized;
            netForce *= -dragCoef;*/

            particle.oldPos = particle.position;
            particle.position += particle.velocity* particle.dampValue;
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

    void ResetForces()
    {
        GravityForce = Vector3.zero;
        WindForce = Vector3.zero;
        AirResistanceForce = Vector3.zero;
        DragForce = Vector3.zero;
    }
    Vector2 CalcForces(Particle p)
    {
        ResetForces();

        GravityForce = new Vector2(0, p.gravity * p.mass) * 10f;

        if (simulateWind)
        {
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
            Vector3 springDir = (spring.linkedParticle.position - p.position).normalized;
            Vector3 springForce = springConstant * error * springDir;

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
                    changeDir = (connector.linkedParticle.position + connector.startParticle.position).normalized;
                }

                changeAmount = changeDir * error;
                

                connector.startParticle.position -= changeAmount * 0.5f;
                connector.linkedParticle.position += changeAmount * 0.5f;
            }

        }
    }

    private void ComputeDiagConstraints()
    {
        foreach (var diagonalSpring in diagSpringList)
        {
            if (diagonalSpring.isEnabled) {

                float distance= (diagonalSpring.particleOne.position - diagonalSpring.particleTwo.position).magnitude;
                float errorChange = Mathf.Abs(diagonalSpring.restLength-distance);
                
                if (distance > diagonalSpring.restLength)
                {
                    changeDiagDir = (diagonalSpring.particleOne.position - diagonalSpring.particleTwo.position).normalized;
                }
                
                else if (distance < diagonalSpring.restLength)
                {
                    changeDiagDir = (diagonalSpring.particleOne.position + diagonalSpring.particleTwo.position).normalized;
                }
                changeDiagAmount = changeDiagDir * errorChange;
            
                
                diagonalSpring.particleTwo.position += changeDiagAmount * 0.5f;
                diagonalSpring.particleOne.position -= changeDiagAmount * 0.5f;

            }           
        }
    }

    private void DrawSpringConnectors()
    {
        //foreach (var spring in springList)
        //{
        //    if (spring.startParticle != null && spring.linkedParticle != null)
        //    {

        //        if (spring.isEnabled)
        //        {
        //            Debug.DrawLine(spring.startParticle.position, spring.linkedParticle.position, Color.white,Time.deltaTime);
        //        }
        //    }
        //}

        //foreach (var bendSpring in bendingSpringList)
        //{
        //    if (bendSpring.particleOne != null && bendSpring.particleThree != null)
        //    {
                
        //        Debug.DrawLine(bendSpring.particleOne.position, bendSpring.particleThree.position, Color.white);
        //    }
        //}

    }

    private void OnDrawGizmos()
    {
        if (showParticles)
        {
            Gizmos.color = particleColour;
            if (!particleList.IsUnityNull())
            {
                for (int i = 0; i < particleList.Count; ++i)
                {
                    Gizmos.DrawSphere(particleList[i].position, particleSize);
                }
            }
            
        }
        
        if (showDiagConstraints)
        {
            Gizmos.color = Color.white;

            if (!diagSpringList.IsUnityNull())
            {
                foreach (var spring in diagSpringList)
                {
                    if (spring.isEnabled)
                    {
                        Gizmos.DrawLine(spring.particleOne.position, spring.particleTwo.position);
                    }
                }
            }
        }

        if (showStructuralConstraints)
        {

            Gizmos.color = Color.white;
            if (!diagSpringList.IsUnityNull())
            {
                foreach (var spring in springList)
                {
                    if (spring.isEnabled)
                    {
                        Gizmos.DrawLine(spring.startParticle.position, spring.linkedParticle.position);
                    }
                }
            }
        }

    }

}

