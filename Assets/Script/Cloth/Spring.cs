
using UnityEngine;

public class Spring
{
    public bool isEnabled = true;

    public Particle startParticle;
    public Particle linkedParticle;

    
    public float restLength;

    public Spring (Particle startParticle,Particle linkedParticle, float restLength)
    {
        
        this.startParticle = startParticle;
        this.linkedParticle = linkedParticle;

        this.restLength = restLength;
        
    }
}
