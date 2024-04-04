using UnityEngine;

// script for elevator down button
public class ElevatorDownButton : MonoBehaviour
{
    public ElevatorControl elevatorControl;
    private void OnTriggerEnter(Collider other)
    {
        elevatorControl.MoveElevatorDown();
    }
}
