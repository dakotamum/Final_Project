using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PortalTeleporter : MonoBehaviour
{
    public Transform destinationPortal;
    public static float scaleRatio;

    private void OnTriggerEnter(Collider other)
    {
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        Navigation navigation = xrOrigin.GetComponent<Navigation>();

        if (xrOrigin != null)
        {
            if (destinationPortal.gameObject.CompareTag("PortalA"))
            {
                xrOrigin.transform.position = destinationPortal.position - destinationPortal.forward * destinationPortal.localScale.y;
            }
            else
            {
                xrOrigin.transform.position = destinationPortal.position + destinationPortal.forward * destinationPortal.localScale.y;
            }

            scaleRatio = destinationPortal.localScale.y / xrOrigin.transform.localScale.y;

            AdjustPlayer(xrOrigin, navigation);
        }
    }

    void AdjustPlayer(XROrigin xrOrigin, Navigation navigation)
    {
        xrOrigin.transform.localScale *= scaleRatio;

        navigation.speed *= scaleRatio;
        navigation.jumpHeight *= scaleRatio;
        if (xrOrigin.TryGetComponent<CharacterController>(out var characterController))
        {
            characterController.height *= scaleRatio;
            characterController.radius *= scaleRatio;
            characterController.skinWidth *= scaleRatio;
            characterController.stepOffset *= scaleRatio;
            characterController.center = new Vector3(characterController.center.x, characterController.center.y * scaleRatio, characterController.center.z);
        }

        float angleDifference = destinationPortal.transform.eulerAngles.y - xrOrigin.transform.eulerAngles.y;
        if (destinationPortal.gameObject.CompareTag("PortalA"))
        {
            angleDifference += 180;
        }
        xrOrigin.transform.Rotate(0, angleDifference, 0);
    }
}
