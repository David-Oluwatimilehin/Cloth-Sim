using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameParticle : MonoBehaviour
{    
    //TODO: Make the position a transform
    
    public Vector3 pos;
    public Vector3 oldPos;
    public Vector3 vel;
    public int energy;
    public Color colour;
    
    public bool isEnabled;
    public float size;
    public float lifetime;
    
    private SpriteRenderer spriteSheet;
    private SphereCollider col;
    private Transform t;

    public void Start()
    {
        
        col=GetComponent<SphereCollider>();
        col.radius = size;

        t = GetComponent<Transform>();
        t.position = pos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Something is happening");
    }

    public void Update()
    {
        
        
        t.position = pos;
    }
}
