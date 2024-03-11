using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector
{
    public bool isEnabled = true;

    public LineRenderer lineRender;
    public GameObject particleOne;
    public GameObject particleTwo;
    
    public Particle pointOne;
    public Particle pointTwo;
    public Vector2 changeDir;
}
