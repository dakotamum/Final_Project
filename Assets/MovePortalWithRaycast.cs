using Unity.XR.CoreUtils;
using UnityEngine;
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
    private static bool isOnForest = false;
    private bool portalToggle = false;

    private float lastToggleTime = 0f;
    private readonly float toggleCooldown = 0.5f;

    private static bool handlePortalA = false;

    private void Start()
    {
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        fixedObject.transform.localScale = xrOrigin.transform.localScale;
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
                objectToMove.SetActive(portalToggle);
                if (!portalToggle && fixedObject.activeSelf)
                {
                    fixedObject.SetActive(false);
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
            indicator.gameObject.SetActive(true);
            isOnTable = false;
            isOnForest = false;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                handlePortalA = true;

                Vector3 newObjectToMovePosition = hit.point;

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Table"))
                {
                    isOnTable = true;
                    newObjectToMovePosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ForestFloor"))
                {
                    isOnForest = true;
                    isOnTable = false;
                    newObjectToMovePosition.y = -(1.0f - objectToMove.transform.localScale.y) + 1.75f * objectToMove.transform.localScale.y;
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    isOnTable = false;
                    newObjectToMovePosition.y = - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
                }

                objectToMove.transform.position = newObjectToMovePosition;
            }
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

        if (isAPressed)
        {
            Vector3 newScale = objectToMove.transform.localScale - scaleSpeed * Time.deltaTime * Vector3.one;
            newScale.x = Mathf.Max(0.08f, newScale.x);
            newScale.y = Mathf.Max(0.08f, newScale.y);
            newScale.z = Mathf.Max(0.08f, newScale.z);

            objectToMove.transform.localScale = newScale;
            Vector3 newPosition = objectToMove.transform.position;
            if (isOnTable)
            {
                newPosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
            }
            else if (isOnForest)
            {
                newPosition.y = - (1.0f - objectToMove.transform.localScale.y) + 1.75f * objectToMove.transform.localScale.y;
            }
            else 
            {
                newPosition.y = - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
            }
            objectToMove.transform.position = newPosition;
        }

        if (isBPressed)
        {
            Vector3 newScale = objectToMove.transform.localScale + scaleSpeed * Time.deltaTime * Vector3.one;
            newScale.x = Mathf.Min(1f, newScale.x);
            newScale.y = Mathf.Min(1f, newScale.y);
            newScale.z = Mathf.Min(1f, newScale.z);

            objectToMove.transform.localScale = newScale;
            Vector3 newPosition = objectToMove.transform.position;

            if (isOnTable)
            {
                newPosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
            }
            else if (isOnForest)
            {
                newPosition.y = - (1.0f - objectToMove.transform.localScale.y) + 1.75f * objectToMove.transform.localScale.y;
            }
            else 
            {
                newPosition.y = - (1.0f - objectToMove.transform.localScale.y) + 0.75f * objectToMove.transform.localScale.y;
            }
            objectToMove.transform.position = newPosition;
        }

        if (isXPressed)
        {
            objectToMove.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (isYPressed)
        {
            objectToMove.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
