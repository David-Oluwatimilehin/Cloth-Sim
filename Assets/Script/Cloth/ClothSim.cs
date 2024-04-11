using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class ClothSim : MonoBehaviour
{

    // Methods for determining wind should be globbally varying constant function
    // Do (sin (x*y*t), cos(z*t), sin(cos(5*x*y*z) ) ) For the Wind Vector

    // TODO: Change Mesh to be skinnedMesh attack bpnes for the connectors.
    // TODO: Figure out how to deform the mesh
    // TODO: REMOVE the pinned particles so that only the first and last particles of the row are pinned.

    // Uses Verlet Intergration with velocity
    // Computed by taking the previous positions in account with the current and previous timestep
    [Header("Cloth Attributes")]
    [SerializeField] public int rows = 48;
    [SerializeField] public int columns = 48;
    [SerializeField] public float spacing = 1.0f;

    private Mesh clothMesh;
    

    Vector2 spawnVec;


    [Header("Wind Attributes")]
    [SerializeField] public bool simulateWind;
    [SerializeField] public float airDensity;
    [SerializeField] public float windSpeed;
    [SerializeField] public float dragCooeficient;
    //float springConstant = 2f;

    
    private Vector3[] clothVertices;
    private List<Particle> particleList;
    private List<Connector> connectorList;
    
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
        connectorList = new List<Connector>();
        //sphereList = new List<GameObject>();

        particleSpawnPosition = new Vector2(0, 0);
        particleColour = new Color(255, 0, 0);

        //GenerateVertices();
        GenerateSkinnedMesh();
        SetupPoints();
        

    }
    private void GenerateSkinnedMesh()
    {
        GetComponent<MeshFilter>().mesh= clothMesh = new Mesh();
        clothMesh.name = "Skinned Mesh";
        
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

    float ApplyWind()
    {
        Vector2 wind = Vector2.zero;
        if (simulateWind)
        {
            wind.x = 0.5f * airDensity * windSpeed * windSpeed *
                (3.14159f * particleSize / 2 * particleSize / 2) * dragCooeficient;
        }

        return wind.x;
    }
    void SetupPoints()
    {

        for (int y = 0; y <= rows; y++)
        {
            for (int x = 0; x <= columns; x++)
            {
                    // Creates the Partcles that will be added to the list
                    Particle particle = new Particle();
                    particle.pinPos = new Vector2( particleSpawnPosition.y,particleSpawnPosition.x);


                    spawnVec = new Vector2(particleSpawnPosition.y, particleSpawnPosition.x);

                    // Sets up X Connectors
                    if (x != 0)
                    { 
                        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();

                        // Creates the connector that will link the particles
                        //line.transform.SetParent(t, false);

                        Connector connector = new Connector();
                        //connector.particleOne = go;
                        //connector.particleTwo = sphereList[sphereList.Count - 1];

                        connector.pointOne = particle;
                        connector.pointTwo = particleList[particleList.Count - 1];
                        connector.pointOne.position = spawnVec;
                        connector.pointTwo.oldPos = spawnVec;

                        connector.restLength = spacing;

                        connectorList.Add(connector);

                        connector.lineRender = line;
                        connector.lineRender.material = connectorMaterial;

                        if (y != 0)
                        { 
                            //CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x - 1)], particle, particleList[(y - 1) * (rows + 1) + (x - 1)], Mathf.Sqrt(2) * spacing);
                        }

                    }

                    // Sets up Y Connectors
                    if (y != 0)
                    {
                        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
                        
                        //line.transform.SetParent(t, false);
                        // Creates the connector that will link the particles
                        
                        Connector connector = new Connector();
                        //connector.particleOne = go;
                        //connector.particleTwo = sphereList[(y - 1) * (rows + 1) + x];

                        connector.pointOne = particle;
                        connector.pointTwo = particleList[(y - 1) * (rows + 1) + x];

                        connector.pointOne.position = spawnVec;
                        connector.pointTwo.oldPos = spawnVec;

                        connector.restLength = spacing;

                        connectorList.Add(connector);

                        connector.lineRender = line;
                        connector.lineRender.material = connectorMaterial;

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

    float SinWindFunc(Particle p)
    {

        return Mathf.Sin(p.position.x * p.position.y);
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

    void Update()
    {
        //Vector3 mousePos= Input.mousePosition;
        //Vector3 mousePos_new = Camera.main.ScreenToWorldPoint(mousePos);


        //if(Input.GetMouseButton(0))
        //{
        //    for(int i=0; i<connectorList.Count; i++)
        //    {
        //        float dist = Vector3.Distance(mousePos_new, connectorList[i].particleOne.transform.position);
        //        if(dist <= 1.05f)
        //        {
        //            connectorList[i].isEnabled = false;
        //        }
        //    }
        //}

        //for (int i=0; i<connectorList.Count; i++)
        //{
        //    float dist = Vector3.Distance(connectorList[i].pointOne.position, connectorList[i].pointTwo.position);
            
        //    if(dist > 1.4f)
        //    {
        //        connectorList[i].isEnabled = false;
        //    }

        //}
        
    }

    void PhysicsLoop()
    {
        for(int p=0; p<particleList.Count; p++)
        {
            
            if (particleList[p].isPinned==true)
            {
                particleList[p].position = particleList[p].pinPos;
                particleList[p].oldPos = particleList[p].pinPos;
            }
            else
            {        
                particleList[p].velocity = (particleList[p].position - particleList[p].oldPos) * particleList[p].friction;
                particleList[p].oldPos = particleList[p].position;

                
                particleList[p].position += particleList[p].velocity * particleList[p].dampValue;
                particleList[p].position.x += ApplyWind() * Time.fixedDeltaTime;
                //particleList[p].position.x += SinWindFunc(particleList[p]) * Time.fixedDeltaTime;
                particleList[p].position.y += particleList[p].gravity * Time.fixedDeltaTime;
            }
        }

        foreach (var connector in connectorList)
        {
            if (connector.isEnabled)
            {
                float dist = (connector.pointOne.position - connector.pointTwo.position).magnitude;
                float error = Mathf.Abs(dist - spacing);

                if (dist > spacing)
                {
                    connector.changeDir = (connector.pointOne.position - connector.pointTwo.position).normalized;

                }else if (dist < spacing)
                {
                    connector.changeDir = (connector.pointTwo.position - connector.pointOne.position).normalized;
                }

                Vector2 changeAmount = connector.changeDir * error;
                
                connector.pointOne.position -= changeAmount * 0.5f;
                connector.pointTwo.position += changeAmount * 0.5f;
            }
            else
            {
                // Do Nothing
            }
        
        }

        

        for (int i=0; i < connectorList.Count; i++)
        {
            if (connectorList[i].isEnabled == false)
            {
                Destroy(connectorList[i].lineRender);
            }
            else
            {
                var points = new Vector3[2];
                points[0] = connectorList[i].pointOne.position;
                points[1] = connectorList[i].pointTwo.position;

                connectorList[i].lineRender.startWidth = 0.04f;
                connectorList[i].lineRender.endWidth = 0.04f;
                connectorList[i].lineRender.SetPositions(points);

            }
        }

    }


    void UpdateVerletBody()
    {
        //var startDist = 0.5f;


        foreach (var particle in particleList)
        {
            if (particle.isPinned)
            {
                particle.position = particle.pinPos;
                particle.oldPos = particle.pinPos;
            }
            else
            {
                // Calculate new position using Verlet integration
                Vector3 newPos = 2 * particle.position - particle.oldPos + particle.acc * Time.fixedDeltaTime * Time.fixedDeltaTime;
                particle.oldPos = particle.position;

                // Apply damping
                Vector3 vel = (newPos - (Vector3)particle.oldPos) / Time.fixedDeltaTime;
                newPos += particle.dampValue * vel;

                // Update position
                particle.position = newPos;
            }
        }



        // Update connectors
        foreach (var connector in connectorList)
        {
            if (connector.isEnabled)
            {
                Vector3 delta = connector.pointTwo.position - connector.pointOne.position;
                float dist = delta.magnitude;
                float error = dist - connector.restLength;

                Vector3 dir = delta.normalized;
                //Vector3 force = stiffness * error * dir;

                //connector.pointOne.acc += (Vector2)force;
                //connector.pointTwo.acc -= (Vector2)force;
            }
        }
    }

    



    

    void CreateDiagConnector(GameObject gameObjOne, GameObject gameObjTwo, Particle particleOne, Particle particleTwo, float length)
        {
            LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
            
            //line.transform.SetParent(t, false);
            //connector.restLength = length;

            Connector connector = new Connector();
            //connector.particleOne = gameObjOne;
            //connector.particleTwo = gameObjTwo;

            connector.pointOne = particleOne;
            connector.pointTwo = particleTwo;

            connector.pointOne.position = gameObjOne.transform.position;
            connector.pointTwo.oldPos = gameObjTwo.transform.position;

            connectorList.Add(connector);

            connector.lineRender = line;
            connector.lineRender.material = connectorMaterial;
        }

}

