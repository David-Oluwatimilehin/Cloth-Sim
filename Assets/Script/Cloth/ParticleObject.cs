
using Unity.VisualScripting;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    public SphereCollider col;
    Transform t;
    public Particle p;
    public float size;

    void Start()
    {
        col = GetComponent<SphereCollider>();
        col.radius = size;
        
        t = GetComponent<Transform>();
        p.position = transform.localPosition;
        
    }
    private void OnCollisionEnter(Collision collision)
    {
               
    }

    void Update()
    {
        t.position = p.position;
    }
}
