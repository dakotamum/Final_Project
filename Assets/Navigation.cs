using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class Navigation : MonoBehaviour
{
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    private Vector2 rotationInput;
    private Vector2 movementInput;
    private CharacterController character;
    public float speed = 5.0f;
    public float boostedSpeedMultiplier = 2.0f;
    private XROrigin rig;
    private float fallingSpeed;
    public float rotationSpeed = 50f;
    public float jumpHeight = 8f;
    private float jumpSpeed;
    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
        // initialize jump speed and currentSpeed
        jumpSpeed = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);
    }
    private void FixedUpdate()
    {
        if (character.isGrounded && fallingSpeed < 0)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += Physics.gravity.y * Time.fixedDeltaTime; // apply gravity
        }

        Vector3 fallMovement = Vector3.up * fallingSpeed * Time.fixedDeltaTime;
        character.Move(fallMovement);

        if (movementInput != Vector2.zero)
        {
            Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
            Vector3 direction = headYaw * new Vector3(movementInput.x, 0, movementInput.y);
            character.Move(direction * Time.fixedDeltaTime * speed);
        }

        if (rotationInput != Vector2.zero)
        {
            transform.Rotate(Vector3.up, rotationInput.x * rotationSpeed * Time.fixedDeltaTime);
        }
    }
    private void Update()
    {
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rotationInput);
        // bool xButtonPressed;
        // leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out xButtonPressed);
        // // jump if we're on the ground and x was pressed
        // if (xButtonPressed && character.isGrounded)
        // {
        //     fallingSpeed = jumpSpeed;
        // }

        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out movementInput);

        // check for acceleration
        // rightDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        // bool isTriggerPressed = triggerValue > 0.1f;
        // currentSpeed = isTriggerPressed ? speed * boostedSpeedMultiplier : speed;
    }
}
