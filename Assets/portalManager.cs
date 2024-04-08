using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalManager : MonoBehaviour
{
    public Transform APos;
    public Transform BPos;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("PortalA"))
        {
            CharacterController cc = GetComponent<CharacterController>();

            cc.enabled = false;
            transform.position = BPos.transform.position;
            transform.rotation = new Quaternion(transform.rotation.x, BPos.rotation.y, transform.rotation.z, transform.rotation.w);

            cc.enabled = true;
        }

        if (col.CompareTag("PortalB"))
        {
            CharacterController cc = GetComponent<CharacterController>();

            cc.enabled = false;
            transform.position = APos.transform.position;
            transform.rotation = new Quaternion(transform.rotation.x, APos.rotation.y, transform.rotation.z, transform.rotation.w);

            cc.enabled = true;
        }
    }
}
