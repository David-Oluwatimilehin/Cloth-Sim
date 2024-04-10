using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSystem : MonoBehaviour
{
    public Camera cam;

    [SerializeField]
    public Texture2D texture;
    [SerializeField]
    public int numberOfParticles;


    public BlendShapeBufferRange[] blendShapes;
    public FlameParticle[] particles;

    
    public int nrAlive;

    void Start()
    {
        cam = Camera.main;
        particles = new FlameParticle[numberOfParticles];
    }

    void Update()
    {
        




    }
    void LateUpdate()
    {
        transform.LookAt(cam.transform);

        transform.Rotate(0, 180, 0);
    }

}
