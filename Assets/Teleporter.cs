using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PortalTeleporter : MonoBehaviour
{
    public Transform destinationPortal;
    private bool isTeleporting = false;
    public float teleportCooldown = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (isTeleporting) return;

        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        Navigation navigation = xrOrigin.GetComponent<Navigation>();

        if (xrOrigin != null && !isTeleporting)
        {
            xrOrigin.transform.position = destinationPortal.position;

            float scaleRatio = destinationPortal.localScale.y / xrOrigin.transform.localScale.y;

            adjustPlayer(xrOrigin, navigation, scaleRatio);
            StartCoroutine(TeleportCooldown());
        }
    }

    void adjustPlayer(XROrigin xrOrigin, Navigation navigation, float scaleRatio)
    {
        xrOrigin.transform.localScale *= scaleRatio;
        
        navigation.speed *= scaleRatio;
        navigation.jumpHeight *= scaleRatio;
        CharacterController characterController = xrOrigin.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.height *= scaleRatio;
            characterController.radius *= scaleRatio;
            characterController.skinWidth *= scaleRatio;
            characterController.stepOffset *= scaleRatio;
            characterController.center = new Vector3(characterController.center.x, characterController.center.y * scaleRatio, characterController.center.z);
        }

        float angleDifference = destinationPortal.transform.eulerAngles.y - xrOrigin.transform.eulerAngles.y;
        if (destinationPortal.gameObject.tag == "PortalA")
        {
            angleDifference += 180;
        }
        xrOrigin.transform.Rotate(0, angleDifference, 0);
    }

    private IEnumerator TeleportCooldown()
    {
        isTeleporting = true;
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}
