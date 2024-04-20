
using UnityEngine;

public class BendingSpring
{
    public Particle particleOne;
    public Particle particleTwo;
    public Particle particleThree;

    public float restAngle;

    public BendingSpring(Particle particleOne, Particle particleTwo, Particle particleThree, float restAngle)
    {
        this.particleOne = particleOne;
        this.particleTwo = particleTwo;
        this.particleThree = particleThree;
        this.restAngle = restAngle;
    }
}
