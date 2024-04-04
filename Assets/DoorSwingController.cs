using UnityEngine;

// script for controlling door swing
public class DoorSwingController : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float closedPosition = 0f;
    public float openPosition = 90f;
    private bool doorIsOpen = false;
    private float currentRotation;

    void Start()
    {
        currentRotation = transform.localEulerAngles.y;
    }

    // update door angle based on time and if the door is toggled closed or open
    void Update()
    {
        float targetRotation = doorIsOpen ? openPosition : closedPosition;
        if (Mathf.Abs(currentRotation - targetRotation) > 0.01f)
        {
            float step = rotationSpeed * Time.deltaTime;
            currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, step);
            transform.localRotation = Quaternion.Euler(0f, currentRotation, 0f);
        }
    }

    // toggle the door open or closed
    public void ToggleDoor()
    {
        doorIsOpen = !doorIsOpen;
    }
}
