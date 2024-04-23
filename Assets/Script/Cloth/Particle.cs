
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    public bool isPinned = false;
    public List<Spring> connectedSprings= new List<Spring>();
    
    public Vector2 position;
    public Vector2 pinPos;
    public Vector2 oldPos;
    public Vector2 velocity;

    public float energy;
    public float mass = 1f;
    public Vector2 acc = new Vector2();

    public float friction = 0.60f;
    public float gravity = -2.4f;
    public float dampValue = 0.4f;
}
