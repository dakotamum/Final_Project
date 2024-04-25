using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class MovePortalWithRaycast : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public GameObject objectToMove;
    public GameObject fixedObject;
    public XRNode rightInputSource;
    public XRNode leftInputSource;
    private InputDevice leftDevice;
    private InputDevice rightDevice;
    private readonly float scaleSpeed = 0.5f;
    private readonly float rotationSpeed = 55f;
    private static bool isOnTable = false;
    private bool portalToggle = false;

    private float lastToggleTime = 0f;
    private readonly float toggleCooldown = 0.5f;

    private Vector3 initialScale;
    private float initialDistance;
    private float initialLeftAngle;
    private float initialRightAngle;
    private float initialObjectAngle;

    private void Start()
    {
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        fixedObject.transform.localScale = xrOrigin.transform.localScale;
        initialScale = objectToMove.transform.localScale;
        rayInteractor.hitClosestOnly = true;
    }

    void Update()
    {
        Transform indicator = objectToMove.transform.Find("Indicator");
        if (Time.time - lastToggleTime > toggleCooldown)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
            leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerPressed);

            //Toggle Portal Mode
            if (isLeftTriggerPressed)
            {
                portalToggle = !portalToggle;
                if (!portalToggle && fixedObject.activeSelf)
                {
                    fixedObject.transform.position = new(-3, -1, -8);
                    fixedObject.transform.localScale = new(1, 1, 1);
                    fixedObject.SetActive(false);

                    objectToMove.transform.position = new(0, -1, -8);
                    objectToMove.transform.localScale = new(1, 1, 1);
                    objectToMove.SetActive(false);
                }
                lastToggleTime = Time.time;
            }
        }

        if (!portalToggle)
        {
            return;
        }

        rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerPressed);

        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (isRightTriggerPressed)
        {
            fixedObject.SetActive(false);
            if (!objectToMove.activeSelf)
            {
                objectToMove.SetActive(true);
                objectToMove.transform.localScale = Vector3.one;
                objectToMove.transform.rotation = Quaternion.Euler(0, xrOrigin.Camera.transform.eulerAngles.y, 0);
            }

            indicator.gameObject.SetActive(true);
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    Vector3 newObjectToMovePosition = hit.point + new Vector3(0f, 0.001f, 0f);
                    objectToMove.transform.position = newObjectToMovePosition;
                }
            }
            else
            {
                return;
            }
        }
        else if (!isRightTriggerPressed && indicator.gameObject.activeSelf)
        {
            indicator.gameObject.SetActive(false);

            fixedObject.SetActive(true);
            fixedObject.transform.localScale = xrOrigin.transform.localScale;

            Vector3 newFixedObjectPosition;
            Vector3 offset = new(0, 0.2f, 0);
            if (Minimap.isMinimapActive)
            {
                if (Physics.Raycast(Minimap.originalPosition + offset, -Vector3.up, out RaycastHit hit))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                    {
                        newFixedObjectPosition = hit.point + new Vector3(0f, 0.001f, 0f) + Minimap.originalForward * xrOrigin.transform.localScale.magnitude;
                        fixedObject.transform.SetPositionAndRotation(newFixedObjectPosition, Minimap.originalRotation);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(xrOrigin.Camera.transform.position + offset, -Vector3.up, out RaycastHit hit))
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                    {
                        newFixedObjectPosition = hit.point + new Vector3(0f, 0.001f, 0f) + xrOrigin.transform.forward * xrOrigin.transform.localScale.magnitude;
                        fixedObject.transform.SetPositionAndRotation(newFixedObjectPosition, xrOrigin.transform.rotation);
                    }
                }
            }
        }

        rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);
        
        leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed);
        leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed);
        

        if (isAPressed || isXPressed)
        {
            rightDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandPosition);
            leftDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandPosition);

            if (initialDistance == 0)
            {
                initialDistance = Vector3.Distance(leftHandPosition, rightHandPosition);
            }
            else
            {
                float currentDistance = Vector3.Distance(leftHandPosition, rightHandPosition);
                float scaleMultiplier = currentDistance / initialDistance;

                Vector3 newScale = 1.1f * scaleMultiplier * initialScale;
                if (scaleMultiplier < 1)
                {
                    newScale.x = Mathf.Max(0.08f, newScale.x);
                    newScale.y = Mathf.Max(0.08f, newScale.y);
                    newScale.z = Mathf.Max(0.08f, newScale.z);
                }
                else if (scaleMultiplier > 1)
                {
                    newScale.x = Mathf.Min(1f, newScale.x);
                    newScale.y = Mathf.Min(1f, newScale.y);
                    newScale.z = Mathf.Min(1f, newScale.z);
                }

                objectToMove.transform.localScale = newScale;
            }
        }
        else
        {
            initialDistance = 0;
            initialScale = objectToMove.transform.localScale;
        }

        leftDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftHandRotation);
        if (isYPressed)
        {

            if (initialLeftAngle == 0)
            {
                initialLeftAngle = leftHandRotation.eulerAngles.y;
                initialObjectAngle = objectToMove.transform.eulerAngles.y;
            }
            else
            {
                float currentAngle = leftHandRotation.eulerAngles.y;
                float angleDifference = currentAngle - initialLeftAngle;
                objectToMove.transform.rotation = Quaternion.Euler(0, initialObjectAngle + angleDifference, 0);
            } 
        }
        else
        {
            initialLeftAngle = 0;
        }

        rightDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightHandRotation);
        if (isBPressed)
        {

            if (initialRightAngle == 0)
            {
                initialRightAngle = rightHandRotation.eulerAngles.y;
                initialObjectAngle = objectToMove.transform.eulerAngles.y;
            }
            else
            {
                float currentAngle = rightHandRotation.eulerAngles.y;
                float angleDifference = currentAngle - initialRightAngle;
                objectToMove.transform.rotation = Quaternion.Euler(0, initialObjectAngle + angleDifference, 0);
            }
        }
        else
        {
            initialRightAngle = 0;
        }
    }
}
