using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float rotDir;
    private Vector3 moveDir;
    private Vector3 inputDir;

    [SerializeField] public int ScrollSize = 20;
    [SerializeField] public float rotateSpeed = 30.0f;
    [Range(2f, 50f)] public float CameraSpeed = 10.0f;
    private void Update()
    {
        
        inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.y = 1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = 1f;

        //if (Input.mousePosition.x < ScrollSize) inputDir.x = -1f;
        //if (Input.mousePosition.y < ScrollSize) inputDir.y = -1f;
        //if (Input.mousePosition.x > Screen.width - ScrollSize) inputDir.x = 1f;
        //if (Input.mousePosition.y > Screen.height - ScrollSize) inputDir.y = 1f;


        moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        float  camSpeed= CameraSpeed;
        transform.position += moveDir * camSpeed * Time.deltaTime;

        rotDir = 0.0f;
        if (Input.GetKey(KeyCode.Q)) rotDir = 1f;
        if (Input.GetKey(KeyCode.E)) rotDir = -1f;
        transform.eulerAngles += new Vector3( rotDir * rotateSpeed * Time.deltaTime,0, 0);


    }



}
