using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
	public Camera cam;
	public GameObject A;
	public GameObject B;

	private void Update()
	{

		//if (Input.GetButton("Fire1"))
		//Get the Right Hand A button press
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
			ShootA();
		}

        //if (Input.GetButton("Fire2"))
        //Get the Right Hand B button press
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
			ShootB();
		}
	}

	public void ShootA()
	{
		RaycastHit rayHit;

		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit))
		{
			Vector3 hitPos = rayHit.point;
			A.transform.position = hitPos;
		}
	}
	
	public void ShootB()
	{
		RaycastHit rayHit;

		if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit))
		{
			Vector3 hitPos = rayHit.point;
			B.transform.position = hitPos;
		}
	}
}
