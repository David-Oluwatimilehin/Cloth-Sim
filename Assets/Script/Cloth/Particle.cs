
using System.Collections.Generic;
using UnityEngine;

public class Particle 
{
    public bool isPinned = false;
    public List<Spring> connectedSprings= new List<Spring>();
    
    public Vector3 position;
    public Vector3 pinPos;
    public Vector3 oldPos;
    public Vector3 velocity;

    public float energy;
    public float mass = 1f;
    //public Vector3 acc = new Vector2();

    public float friction = 0.60f;
    public float gravity = -0.981f;
    public float dampValue = 0.4f;
}
