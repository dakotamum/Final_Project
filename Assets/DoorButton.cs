using UnityEngine;

// script for buttons that control doors
public class ButtonInteract : MonoBehaviour
{
    public DoorSwingController doorController;

    private void OnTriggerEnter(Collider other)
    {
        doorController.ToggleDoor();
    }
}
