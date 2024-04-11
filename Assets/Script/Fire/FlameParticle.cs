using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameParticle : MonoBehaviour
{
    [Header("Positional Information")]
    Vector3 pos;
    Vector3 oldPos;
    Vector3 vel;
    
    [Header("Lifetime Info")]
    int energy;
    float size;
    
    public Color color;

}
