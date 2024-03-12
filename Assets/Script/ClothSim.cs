using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] public float partcleSize = 0.1f;

    
    private List<Particle> particleList;
    private List<Connector> connectorList;
    private List<GameObject> sphereList;

    public Material material;

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
        SetupLines();
    }
    void PhysicsLoop()
    {

    }

    void SetupPoints()
    {
        for (int y=0; y <= rows; y++) {
            for (int x=0; x <= columns; x++) {
                // Creates the Partcles that will be added to the list
                Particle particle = new Particle();
                particle.pinPos= new Vector2(particleSpawnPosition.x, particleSpawnPosition.y);
                
                // Creates the sphere representation
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                

                go.transform.position = new Vector2(particleSpawnPosition.x, particleSpawnPosition.y);
                go.transform.localScale = new Vector2(partcleSize, partcleSize);
                
                var mat = go.GetComponent<Renderer>();
                mat.material = material;


                // Pins the particle if its in the top row of the grid.
                if (x == 0)
                {
                    particle.isPinned = true;
                }

                // Sets up X Connectors
                if (x != 0)
                {
                    LineRenderer line= new GameObject("Line").AddComponent<LineRenderer>();
                    
                    // Creates the connector that will link the particles
                    Connector connector = new Connector();
                    connector.particleOne = go;
                    connector.particleTwo = sphereList[sphereList.Count-1];

                    connector.pointOne = particle;
                    connector.pointTwo = particleList[particleList.Count-1];
                    connector.pointOne.position = go.transform.position;
                    connector.pointTwo.oldPos = go.transform.position;
                    

                    connectorList.Add(connector);

                    connector.lineRender = line;
                    connector.lineRender.material = material;
                

                }

                // Sets up Y Connectors
                if (y != 0)
                {
                    LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();

                    // Creates the connector that will link the particles
                    Connector connector = new Connector();
                    connector.particleOne = go;
                    connector.particleTwo = sphereList[(y - 1) * (rows + 1) + x];

                    connector.pointOne = particle;
                    connector.pointTwo = particleList[(y - 1) * (rows + 1) + x];

                    connector.pointOne.position = go.transform.position;
                    connector.pointTwo.oldPos = go.transform.position;
                    

                    connectorList.Add(connector);
                    
                    connector.lineRender = line;
                    connector.lineRender.material = material;
                }

                particleSpawnPosition.x -= spacing;

                sphereList.Add(go);
                particleList.Add(particle);
                
            }
            particleSpawnPosition.x = 0;
            particleSpawnPosition.y -=spacing;
        }
    }


    private void SetupLines()
    {
        var startDist = 0.5f;

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
            sphereList[p].transform.localScale = new Vector3(partcleSize, partcleSize, partcleSize);
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
        }
    }
}
