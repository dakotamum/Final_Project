using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Navigation : MonoBehaviour
{
    private CharacterController character;
    private XROrigin rig;
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    private Vector2 rotationInput;
    private Vector2 movementInput;
    public float speed = 5.0f;
    public float boostedSpeedMultiplier = 2.0f;
    private float fallingSpeed;
    public float rotationSpeed = 50f;
    public float jumpHeight = 8f;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    private void FixedUpdate()
    {
        if (character.isGrounded && fallingSpeed < 0)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += Physics.gravity.y * PortalTeleporter.scaleRatio * Time.fixedDeltaTime; // apply gravity
        }

        Vector3 fallMovement = fallingSpeed * Time.fixedDeltaTime * Vector3.up;
        if (fallMovement != Vector3.zero)
        {
            character.Move(fallMovement);
        }
        
        if (movementInput != Vector2.zero)
        {
            Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
            Vector3 direction = headYaw * new Vector3(movementInput.x, 0, movementInput.y);
            character.Move(speed * Time.fixedDeltaTime * direction);
        }

        if (rotationInput != Vector2.zero)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime * rotationInput.x);
        }
    }

    private void Update()
    {
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out movementInput);

        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rotationInput);

        leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isLeftStickPressed);
        rightDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool isRightStickPressed);

        if (isLeftStickPressed && isRightStickPressed) {
            Reset();
        }
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
