using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Transform APos;
    public Transform BPos;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("PortalA"))
        {
            CharacterController cc = GetComponent<CharacterController>();
            cc.enabled = false;
            transform.SetPositionAndRotation(BPos.transform.position, new Quaternion(transform.rotation.x, BPos.rotation.y, transform.rotation.z, transform.rotation.w));
            cc.enabled = true;
        }

        if (col.CompareTag("PortalB"))
        {
            CharacterController cc = GetComponent<CharacterController>();
            cc.enabled = false;
            transform.SetPositionAndRotation(APos.transform.position, new Quaternion(transform.rotation.x, APos.rotation.y, transform.rotation.z, transform.rotation.w));
            cc.enabled = true;
        }
    }
}
