using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class ClothSim : MonoBehaviour
{

    // Methods for determining wind should be globbally varying constant function
    // Do (sin (x*y*t), cos(z*t), sin(cos(5*x*y*z) ) ) For the Wind Vector

    // Uses Verlet Intergration without velocity
    // Computed by taking the previous positions in account with the current and previous timestep

    [SerializeField] public int rows = 48;
    [SerializeField] public int columns = 48;
    [SerializeField] public float spacing = 1.0f;
    [SerializeField] public float particleSize = 0.1f;
    [SerializeField] public float stiffness = 1f; 
    [SerializeField] public float damping = 0.5f; 
    [SerializeField] public float springConstant = 2f;



    private List<Particle> particleList;
    private List<Connector> connectorList;
    private List<GameObject> sphereList;

    public Transform t;

    public Material material;
    public Material connectorMaterial;

    Vector2 particleSpawnPosition;

    void Start()
    {
        // Initializes the Lists
        particleList = new List<Particle>();
        connectorList = new List<Connector>();
        sphereList = new List<GameObject>();

        particleSpawnPosition = new Vector2(0, 0);

        SetupPoints();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //SetupLines();
        
        
        DrawConnectorsAndPoints();
    }

    void PhysicsLoop()
    {
        for(int p=0; p<particleList.Count; p++)
        {
            if (!particleList[p].isPinned)
            {
                //particleList[p].oldPos;
                particleList[p].acc = DetermineAcceleration(particleList[p]);


            }


        }
    }
    Vector2 DetermineAcceleration(Particle particle)
    {
        
        
        //particle.mass;



        return Vector2.down;
    }

    Vector2 HookesLawImplementation(Particle pointOne, Particle pointTwo)
    {
        return Vector2.zero;
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
                newPos += damping * vel;

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
                Vector3 force = stiffness * error * dir;

                connector.pointOne.acc += (Vector2)force;
                connector.pointTwo.acc -= (Vector2)force;
            }
        }
    }


    void SetupPoints()
    {

        for (int y = 0; y <= rows; y++)
        {
            for (int x = 0; x <= columns; x++)
            {
                    // Creates the Partcles that will be added to the list
                    Particle particle = new Particle();
                    particle.pinPos = new Vector2(particleSpawnPosition.x, particleSpawnPosition.y);

                    // Creates the sphere representation
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    go.transform.SetParent(t, false);

                    go.transform.position = new Vector2(particleSpawnPosition.x, particleSpawnPosition.y);
                    go.transform.localScale = new Vector2(particleSize, particleSize);

                    var mat = go.GetComponent<Renderer>();
                    mat.material = material;

                    // Sets up X Connectors
                    if (x != 0)
                    {

                        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();

                        // Creates the connector that will link the particles
                        Connector connector = new Connector();
                        connector.particleOne = go;
                        connector.particleTwo = sphereList[sphereList.Count - 1];

                        connector.pointOne = particle;
                        connector.pointTwo = particleList[particleList.Count - 1];
                        connector.pointOne.position = go.transform.position;
                        connector.pointTwo.oldPos = go.transform.position;

                        line.transform.SetParent(t, false);

                        connector.restLength = spacing;

                        connectorList.Add(connector);

                        connector.lineRender = line;
                        connector.lineRender.material = connectorMaterial;

                        if (y != 0)
                        { 
                            CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x - 1)], particle, particleList[(y - 1) * (rows + 1) + (x - 1)], Mathf.Sqrt(2) * spacing);
                        }

                    }

                    // Sets up Y Connectors
                    if (y != 0)
                    {
                        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
                        line.transform.SetParent(t, false);
                        // Creates the connector that will link the particles
                        Connector connector = new Connector();
                        connector.particleOne = go;
                        connector.particleTwo = sphereList[(y - 1) * (rows + 1) + x];

                        connector.pointOne = particle;
                        connector.pointTwo = particleList[(y - 1) * (rows + 1) + x];

                        connector.pointOne.position = go.transform.position;
                        connector.pointTwo.oldPos = go.transform.position;

                        connector.restLength = spacing;

                        connectorList.Add(connector);

                        connector.lineRender = line;
                        connector.lineRender.material = connectorMaterial;

                        if (x != 0)
                        {
                            // Top-right diagonal connectors
                            CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x + 1)], particle, particleList[(y - 1) * (rows + 1) + (x + 1)], Mathf.Sqrt(2) * spacing);
                        }
                        if (x != columns)
                        {
                            // Top-left diagonal connectors
                            CreateDiagConnector(go, sphereList[(y - 1) * (rows + 1) + (x + 1)], particle, particleList[(y - 1) * (rows + 1) + (x + 1)],Mathf.Sqrt(2) * spacing);
                        }
                    }

                    particleSpawnPosition.x -= spacing;

                    sphereList.Add(go);
                    particleList.Add(particle);

            }
            particleSpawnPosition.x = 0;
            particleSpawnPosition.y -= spacing;

        }
    }


    private void DrawConnectorsAndPoints()
    {
        // Update positions and scale of particles
        for (int p = 0; p < particleList.Count; p++)
        {
            Particle point = particleList[p];
            sphereList[p].transform.position = new Vector2(point.position.x, point.position.y);
            sphereList[p].transform.localScale = new Vector3(particleSize, particleSize, particleSize);
        }

        foreach (Connector c in connectorList)
        {
            if (!c.isEnabled)
            {
                Destroy(c.lineRender);

            }
            else
            {
                var points = new Vector3[2];
                points[0] = c.particleOne.transform.position + new Vector3(0, 0, 0);
                points[1] = c.particleTwo.transform.position + new Vector3(0, 0, 0);

                c.lineRender.startWidth = 0.04f;
                c.lineRender.startColor = Color.yellow;

                c.lineRender.endWidth = 0.04f;
                c.lineRender.endColor = Color.blue;
                c.lineRender.SetPositions(points);
            }
        }
        
    }

    void CreateDiagConnector(GameObject gameObjOne, GameObject gameObjTwo, Particle particleOne, Particle particleTwo, float length)
        {
            LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
            line.transform.SetParent(t, false);

            Connector connector = new Connector();
            connector.particleOne = gameObjOne;
            connector.particleTwo = gameObjTwo;

            connector.pointOne = particleOne;
            connector.pointTwo = particleTwo;

            connector.pointOne.position = gameObjOne.transform.position;
            connector.pointTwo.oldPos = gameObjTwo.transform.position;

            connector.restLength = length;

            connectorList.Add(connector);

            connector.lineRender = line;
            connector.lineRender.material = connectorMaterial;
        }


    void SetupLines()
    {

            var startDist = 0.5f;

            // Loop through all connectors
            for (int i = 0; i < connectorList.Count; i++)
            {
                Connector connector = connectorList[i];

                // Check if the connector is enabled
                if (connector.isEnabled)
                {
                    // Calculate distance between particles
                    float dist = (connector.pointTwo.position-connector.pointOne.position).magnitude;
                    float errorMargin = Mathf.Abs(dist - startDist);

                    // Determine direction of change
                    Vector2 changeDir;
                    if (dist > startDist)
                    {
                        changeDir = (connector.pointTwo.position - connector.pointOne.position).normalized;
                    }
                    else
                    {
                        changeDir = (connector.pointTwo.position - connector.pointOne.position).normalized;
                    }

                    // Calculate change amount
                    Vector2 changeAmount = changeDir * errorMargin;

                    // Update positions of connected particles
                    connector.pointOne.position+= changeAmount * 0.5f;
                    connector.pointTwo.position = changeAmount * 0.5f;

                    // Update line renderer positions
                    var points = new Vector3[2];
                    points[0] = connector.particleOne.transform.position;
                    points[1] = connector.particleTwo.transform.position;

                    connector.lineRender.startWidth = 0.05f;
                    connector.lineRender.startColor = Color.yellow;

                    connector.lineRender.endWidth = 0.05f;
                    connector.lineRender.endColor = Color.blue;
                    connector.lineRender.SetPositions(points);
                }
                else
                {
                    // If the connector is not enabled, destroy its line renderer
                    Destroy(connector.lineRender);
                }
            }

            // Update positions and scale of particles
            for (int p = 0; p < particleList.Count; p++)
            {
                Particle point = particleList[p];
                sphereList[p].transform.position = new Vector2(point.position.x, point.position.y);
                sphereList[p].transform.localScale = new Vector3(particleSize, particleSize, particleSize);
            }
    }

        /*var startDist = 0.5f;

        for (int i = 0; i < connectorList.Count; i++)
        {
            if (!connectorList[i].isEnabled)
            {
                // Do Absolutely Nothing
            }
            else
            {
                float dist = (connectorList[i].pointOne.position - connectorList[i].pointTwo.position).magnitude;
                float errorMargin = Mathf.Abs(dist - startDist);

                if (dist > startDist)
                {
                    connectorList[i].changeDir = (connectorList[i].pointOne.position - connectorList[i].pointTwo.position).normalized;
                }
                else if (startDist < dist)
                {
                    connectorList[i].changeDir = (connectorList[i].pointTwo.position - connectorList[i].pointOne.position).normalized;
                }
                Vector2 changeAmount = connectorList[i].changeDir * errorMargin;
                connectorList[i].pointOne.position -= changeAmount * 0.5f;
                connectorList[i].pointTwo.position += changeAmount * 0.5f;
            }
        }

        for (int p=0; p<particleList.Count;p++)
        {
            Particle point = particleList[p];
            sphereList[p].transform.position = new Vector2(point.position.x, point.position.y);
            sphereList[p].transform.localScale = new Vector3(particleSize, particleSize, particleSize);
        }

        foreach(Connector c in connectorList){
            if (!c.isEnabled)
            {
                Destroy(c.lineRender);

            }
            else
            {
                var points = new Vector3[2];
                points[0] = c.particleOne.transform.position + new Vector3(0, 0, 0);
                points[1] = c.particleTwo.transform.position + new Vector3(0, 0, 0);

                c.lineRender.startWidth = 0.04f;
                c.lineRender.startColor = Color.yellow;

                c.lineRender.endWidth = 0.04f;
                c.lineRender.endColor = Color.blue;
                c.lineRender.SetPositions(points);
            }
        }*/
}

