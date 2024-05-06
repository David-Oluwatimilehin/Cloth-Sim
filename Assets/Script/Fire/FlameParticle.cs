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
    public 
    int energy;
    public Color colour;
    
    public bool isEnabled;
    public float size;
    public float lifetime;
    
    private SpriteRenderer spriteSheet;
    private SphereCollider col;
    

    public void Start()
    {
        spriteSheet= GetComponent<SpriteRenderer>();
        spriteSheet.size = new Vector2(size,size);
        
        col=GetComponent<SphereCollider>();   
        
    }

    public void Update()
    {
        col.radius = size;
        
        transform.position = pos;
    }
}
