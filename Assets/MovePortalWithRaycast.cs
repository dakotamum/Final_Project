using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using UnityEngine.XR;

public class MovePortalWithRaycast : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Assign in the inspector
    public GameObject objectToMove; // Assign in the inspector or pass dynamically
    public XRNode rightInputSource;
    public XRNode leftInputSource;
    private float scaleSpeed = 0.5f;
    private float rotationSpeed = 45f;

    void Update()
    {
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightInputSource);
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftInputSource);
        rightDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        bool isTriggerPressed = triggerValue > 0.1f;

        if (isTriggerPressed)
        {
            RaycastHit hit;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            {
                Vector3 modifiedHitPoint = hit.point;

                rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAPressed);
                rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBPressed);
                leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isXPressed);
                leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isYPressed);
                if (isAPressed)
                {
                    objectToMove.transform.localScale = objectToMove.transform.localScale - Vector3.one * scaleSpeed * Time.deltaTime;
                }
                if (isBPressed)
                {
                    objectToMove.transform.localScale = objectToMove.transform.localScale + Vector3.one * scaleSpeed * Time.deltaTime;
                }

                if (isXPressed)
                {
                    objectToMove.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                if (isYPressed)
                {
                    objectToMove.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                }

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Table"))
                {
                    modifiedHitPoint.y = 1 - (1 - objectToMove.transform.localScale.y);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    modifiedHitPoint.y = -(1 - objectToMove.transform.localScale.y);
                }
                objectToMove.transform.position = modifiedHitPoint;
            }
        }
    }
}
