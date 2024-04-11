using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameParticle
{
    [Header("Positional Information")]
    
    //TODO: Make the position a transform
    
    public Vector3 pos;
    Vector3 oldPos;
    public Vector3 vel;
    
    [Header("Lifetime Info")]
    int energy;
    public float size;
    
    public Color color;

}
