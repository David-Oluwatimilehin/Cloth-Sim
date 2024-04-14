
using UnityEngine;

public class Spring
{
    public Particle linkedParticle;

    public float restLength;

    public Spring (Particle linkedParticle, float restLength)
    {
        this.linkedParticle = linkedParticle;
        this.restLength = restLength;
    }
}
