using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class Minimap : MonoBehaviour
{
    private CharacterController character;
    private XROrigin rig;
    public XRNode leftInputSource;
    public XRNode rightInputSource;
    private InputDevice leftDevice;
    private InputDevice rightDevice;

    private readonly float minimapHeight = 75f;
    private Vector3 minimapRotation = new(90f, 0f, 0f);

    public static bool isMinimapActive = false;
    public static Vector3 originalPosition;
    public static Quaternion originalRotation;
    public static Vector3 originalForward;
    private float originalOrthographicSize;
    private bool originalCameraState;

    private float lastToggleTime = 0f;
    private readonly float toggleCooldown = 0.5f;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    void Update()
    {
        if (Time.time - lastToggleTime > toggleCooldown)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
            rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
            leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isLeftGripPressed);
            rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isRightGripPressed);

            if (isLeftGripPressed && isRightGripPressed)
            {
                ToggleMinimap();
                lastToggleTime = Time.time;
            }
        }
    }

    private void ToggleMinimap()
    {
        if (!isMinimapActive)
        {
            EnterMinimapMode();
        }
        else
        {
            ExitMinimapMode();
        }
        isMinimapActive = !isMinimapActive;
    }

    private void EnterMinimapMode()
    {
        originalPosition = rig.transform.position;
        originalRotation = rig.Camera.transform.rotation;
        originalForward = rig.Camera.transform.forward;
        originalOrthographicSize = rig.Camera.orthographicSize;
        originalCameraState = rig.Camera.orthographic;
        rig.Camera.orthographic = true;
        rig.Camera.orthographicSize = 10;
        rig.transform.position = new Vector3(rig.transform.position.x, minimapHeight, rig.transform.position.z);
        rig.Camera.transform.rotation = Quaternion.Euler(minimapRotation);
        character.enabled = false;
    }

    private void ExitMinimapMode()
    {
        rig.Camera.orthographic = originalCameraState;
        rig.Camera.orthographicSize = originalOrthographicSize;
        rig.Camera.transform.rotation = originalRotation;
        rig.transform.position = originalPosition;
        character.enabled = true;
    }
}
