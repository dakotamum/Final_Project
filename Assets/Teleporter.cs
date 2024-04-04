using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PortalTeleporter : MonoBehaviour
{
    public Transform destinationPortal; // Assign this in the Inspector
    private bool isTeleporting = false; // Flag to check if we're in the cooldown period
    public float teleportCooldown = 1.0f; // Cooldown period in seconds

    private void OnTriggerEnter(Collider other)
    {
        if (isTeleporting) return; // Exit if we're in the cooldown period

        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        Navigation navigation = xrOrigin.GetComponent<Navigation>();

        if (xrOrigin != null && !isTeleporting)
        {
            // Teleport the XR Origin to the destination portal
            xrOrigin.transform.position = destinationPortal.position;

            float scaleRatio = destinationPortal.localScale.y / xrOrigin.transform.localScale.y;
            Debug.Log(destinationPortal.transform.localScale.y);

            ScaleXROriginAndAdjustSpeed(xrOrigin, navigation, scaleRatio);
            StartCoroutine(TeleportCooldown());
        }
    }

    void ScaleXROriginAndAdjustSpeed(XROrigin xrOrigin, Navigation navigation, float scaleRatio)
    {
        // Adjusting the local scale of the XR Origin
        xrOrigin.transform.localScale *= scaleRatio;

        // Assuming 'speed' and 'jumpHeight' are public or internal variables of your Navigation script
        navigation.speed *= scaleRatio;
        navigation.jumpHeight *= scaleRatio;

        // Scale the CharacterController dimensions
        CharacterController characterController = xrOrigin.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.height *= scaleRatio;
            characterController.radius *= scaleRatio;
            characterController.skinWidth *= scaleRatio;
            characterController.stepOffset *= scaleRatio;
            characterController.center = new Vector3(characterController.center.x, characterController.center.y * scaleRatio, characterController.center.z);
        }
    }

    private IEnumerator TeleportCooldown()
    {
        isTeleporting = true;
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}
