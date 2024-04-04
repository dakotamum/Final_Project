using UnityEngine;

// script for controlling the elevator
public class ElevatorControl : MonoBehaviour
{
    public Transform elevator;
    public Vector3[] floorPositions;
    private int currentFloor = 0;
    private bool isMoving = false;
    public float speed = 3.0f;

    void Start()
    {
        // initialize floor positions
        floorPositions = new Vector3[] {
                new Vector3(elevator.position.x, 0, elevator.position.z), // first floor
                new Vector3(elevator.position.x, 11, elevator.position.z), // second floor
                new Vector3(elevator.position.x, 22, elevator.position.z) // third floor
            };
    }
    void Update()
    {
        if (isMoving)
        {
            // move towards the next floor position
            elevator.position = Vector3.MoveTowards(elevator.position, floorPositions[currentFloor], speed * Time.deltaTime);

            // if the elevator has reached its destination, stop
            if (elevator.position == floorPositions[currentFloor])
            {
                isMoving = false;
            }
        }
    }
    public void MoveElevatorUp()
    {
        if (!isMoving && currentFloor < floorPositions.Length - 1)
        {
            currentFloor++;
            isMoving = true;
        }
    }
    public void MoveElevatorDown()
    {
        if (!isMoving && currentFloor > 0)
        {
            currentFloor--;
            isMoving = true;
        }
    }
}
