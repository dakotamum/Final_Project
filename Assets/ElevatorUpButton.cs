using UnityEngine;

// script for elevator up button
public class ElevatorUpButton : MonoBehaviour
{
    public ElevatorControl elevatorControl;
    private void OnTriggerEnter(Collider other)
    {
        elevatorControl.MoveElevatorUp();
    }
}
