using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalSpring
{
    public Particle particleOne;
    public Particle particleTwo;

    public bool isEnabled = true;
    public float restlength;

    public DiagonalSpring(Particle p1, Particle p2, float length)
    {
        particleOne = p1;
        particleTwo = p2;

        restlength = length;
    }



}
