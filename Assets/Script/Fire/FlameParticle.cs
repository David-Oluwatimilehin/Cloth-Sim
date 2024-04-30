using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameParticle : MonoBehaviour
{    
    //TODO: Make the position a transform
    
    public Vector3 pos;
    Vector3 oldPos;
    public Vector3 vel;

    int energy;
    public float size;
    public float lifetime;
    
    private SpriteRenderer spriteSheet;
    private SphereCollider col;
    public Color color;

    public void Start()
    {
        spriteSheet= GetComponent<SpriteRenderer>();
        spriteSheet.size = new Vector2(size,size);
        
        col=GetComponent<SphereCollider>();   
        col.radius = size;
    }

    public void Update()
    {
        transform.position = pos;
    }
}
