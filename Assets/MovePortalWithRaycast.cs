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

    private static bool handlePortalA = false;

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
                    fixedObject.SetActive(false);
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
            BoxCollider fixedObjectBoxCollider = fixedObject.GetComponent<BoxCollider>();
            BoxCollider objectToMoveBoxCollider = objectToMove.GetComponent<BoxCollider>();
            fixedObjectBoxCollider.enabled = false;
            objectToMoveBoxCollider.enabled = false;
            fixedObject.SetActive(false);
            objectToMove.SetActive(true);
            indicator.gameObject.SetActive(true);
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                handlePortalA = true;

                Vector3 newObjectToMovePosition = hit.point;

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Table"))
                {
                    isOnTable = true;
                    newObjectToMovePosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    isOnTable = false;
                    newObjectToMovePosition.y = -(1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }

                if (newObjectToMovePosition.y < -1.0f)
                {
                    newObjectToMovePosition.y = -1;
                }

                objectToMove.transform.position = newObjectToMovePosition;
            }
            else
            {
                fixedObjectBoxCollider.enabled = true;
                objectToMoveBoxCollider.enabled = true;
                return;
            }
            fixedObjectBoxCollider.enabled = true;
            objectToMoveBoxCollider.enabled = true;
        }
        else
        {
            indicator.gameObject.SetActive(false);

            if (handlePortalA)
            {
                fixedObject.SetActive(true);
                fixedObject.transform.localScale = xrOrigin.transform.localScale;

                Vector3 newFixedObjectPosition;
                if (Minimap.isMinimapActive)
                {
                    newFixedObjectPosition = Minimap.originalPosition + Minimap.originalForward * xrOrigin.transform.localScale.magnitude;
                }
                else
                {
                    newFixedObjectPosition = xrOrigin.transform.position + xrOrigin.transform.forward * xrOrigin.transform.localScale.magnitude;
                }
                fixedObject.transform.SetPositionAndRotation(newFixedObjectPosition, xrOrigin.transform.rotation);
                
                handlePortalA = false;
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

                Vector3 newPosition = objectToMove.transform.position;

                if (isOnTable)
                {
                    newPosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }
                else
                {
                    newPosition.y = -(1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }
                objectToMove.transform.position = newPosition;
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
