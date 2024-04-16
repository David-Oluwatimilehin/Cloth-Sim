using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraZoom : MonoBehaviour { 

    public float zoomSpeed;
    public float minSpeed;
    public float maxSpeed;

    private float scrollInput;
    private float newZoom;
    
    void Update()
    {
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        newZoom = transform.position.y + scrollInput * zoomSpeed;
        newZoom = Mathf.Clamp(newZoom, minSpeed, maxSpeed);

        transform.position = new Vector3(transform.position.x,transform.position.z , newZoom);
    }


}
