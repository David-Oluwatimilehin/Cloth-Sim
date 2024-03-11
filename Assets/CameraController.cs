using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera cam;
    public GameObject go;
    void Start()
    {
        cam = GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(go.transform.position);
    }
}
