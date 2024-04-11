
using UnityEngine;

public class Particle
{
    public bool isPinned = false;
    
    public Vector2 position;
    public Vector2 pinPos;
    public Vector2 oldPos;
    public Vector2 velocity;

    public float mass = 0.5f;
    public Vector2 acc = new Vector2();

    public float friction = 0.60f;
    public float gravity = -0.35f;
    public float dampValue = 0.2f;
}
