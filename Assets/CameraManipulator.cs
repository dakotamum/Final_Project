using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManipulator : MonoBehaviour
{
    private float minSpeed = 10.0f;
    private float maxSpeed = 20.0f;
    private float currentSpeed;
    private float accel = 20.0f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float rotSens = 3.0f;

    // Update is called once per frame
    void Update()
    {
        // camera rotation
        yaw += Input.GetAxis("Mouse X") * rotSens;
        pitch -= Input.GetAxis("Mouse Y") * rotSens;
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Move camera to center of room 1
        if (Input.GetKey(KeyCode.Alpha1))
        {
            transform.position = new Vector3(28, 4, 4);
        }
        // Move camera to center of room 2
        if (Input.GetKey(KeyCode.Alpha2))
        {
            transform.position = new Vector3(-1, 4, 2);
        }
        // Move forward
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 translateVector = transform.forward;
            translateVector.y = 0;
            transform.Translate(translateVector * currentSpeed * Time.deltaTime, Space.World);
        }
        // Move backward
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 translateVector = -1 * transform.forward;
            translateVector.y = 0;
            transform.Translate(translateVector * currentSpeed * Time.deltaTime, Space.World);
        }
        // Move left
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 translateVector = transform.TransformDirection(Vector3.left);
            translateVector.y = 0;
            transform.Translate(translateVector * currentSpeed * Time.deltaTime, Space.World);
        }
        // Move right
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 translateVector = transform.TransformDirection(Vector3.right);
            translateVector.y = 0;
            transform.Translate(translateVector * currentSpeed * Time.deltaTime, Space.World);
        }
        // Accelerate
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed += accel * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed = minSpeed;
        }
    }
    private void Start()
    {
        currentSpeed = minSpeed;
    }
}
