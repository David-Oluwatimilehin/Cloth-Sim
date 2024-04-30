using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameParticle : MonoBehaviour
{    
    //TODO: Make the position a transform
    
    public Vector3 pos;
    Vector3 oldPos;
    public Vector3 vel;

    int energy;
    public float size;
    private SpriteRenderer spriteSheet;
    public Color color;

    public void Start()
    {
        spriteSheet= GetComponent<SpriteRenderer>();

        spriteSheet.size = new Vector2(size,size);
    }
}
