
using UnityEngine;

public class DiagonalSpring
{
    public Particle particleOne;
    public Particle particleTwo;

    public bool isEnabled = true;
    public float restLength;

    public DiagonalSpring(Particle p1, Particle p2, float length)
    {
        particleOne = p1;
        particleTwo = p2;

        restLength = length;
    }



}
