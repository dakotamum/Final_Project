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
    private readonly float rotationSpeed = 45f;
    private bool isOnTable = true;
    private bool portalToggle = false;

    private float lastToggleTime = 0f;
    private readonly float toggleCooldown = 0.5f;

    void Update()
    {
        if (Time.time - lastToggleTime > toggleCooldown)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
            leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerPressed);

            //Toggle Portal Mode
            if (isLeftTriggerPressed)
            {
                portalToggle = !portalToggle;
                objectToMove.SetActive(portalToggle);
                fixedObject.SetActive(portalToggle);
                lastToggleTime = Time.time;
            }
        }

        if (!portalToggle)
        {
            return;
        }

        rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerPressed);

        if (isRightTriggerPressed)
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                XROrigin xrOrigin = FindObjectOfType<XROrigin>();

                Vector3 newObjectToMovePosition = hit.point;

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Table"))
                {
                    isOnTable = true;
                    newObjectToMovePosition.y = 1.0f - (1.0f - objectToMove.transform.localScale.y);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    isOnTable = false;
                    newObjectToMovePosition.y = objectToMove.transform.localScale.y;
                }

                // newObjectToMovePosition.y = objectToMove.transform.localScale.y / 2;

                Vector3 newFixedObjectPosition;
                if (Minimap.isMinimapActive)
                {
                    newFixedObjectPosition = Minimap.originalPosition + Minimap.originalForward * xrOrigin.transform.localScale.magnitude;
                }
                else
                {
                    newFixedObjectPosition = xrOrigin.transform.position + xrOrigin.transform.forward * xrOrigin.transform.localScale.magnitude;
                }

                //Move both the exit portal and the entry portal to new locations, and both having the Player orientation


                objectToMove.transform.SetPositionAndRotation(newObjectToMovePosition, xrOrigin.transform.rotation);
                fixedObject.transform.SetPositionAndRotation(newFixedObjectPosition, xrOrigin.transform.rotation);
                fixedObject.transform.localScale = xrOrigin.transform.localScale * 0.5f;

                //Set the entry portal slightly away from the player
                //Vector3 modifiedFixedPoint = (xrOrigin.transform.forward * 0.75f) + xrOrigin.transform.position;
                //fixedObject.transform.position = modifiedFixedPoint;
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
                newPosition.y = 1 - (1 - objectToMove.transform.localScale.y);
            }
            else
            {
                newPosition.y = -(0.5f - objectToMove.transform.localScale.y);
            }
            objectToMove.transform.position = newPosition;
        }

        if (isBPressed)
        {
            Vector3 newScale = objectToMove.transform.localScale + scaleSpeed * Time.deltaTime * Vector3.one;
            newScale.x = Mathf.Min(3f, newScale.x);
            newScale.y = Mathf.Min(3f, newScale.y);
            newScale.z = Mathf.Min(3f, newScale.z);

            objectToMove.transform.localScale = newScale;
            Vector3 newPosition = objectToMove.transform.position;
            if (isOnTable)
            {
                newPosition.y = 1 - (1 - objectToMove.transform.localScale.y);
            }
            else
            {
                newPosition.y = -(0.5f - objectToMove.transform.localScale.y);
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
