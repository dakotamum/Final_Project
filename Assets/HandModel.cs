// Note: a lot of this code was reused from the tutorials we've done in class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandModel : MonoBehaviour
{
    // start is called before the first frame update
    public InputDeviceCharacteristics controllerChrateristics;
    private bool isValid = false;
    private InputDevice targetDevice;
    public GameObject controllerPrefab;
    public GameObject handModelPrefab;
    private GameObject spawnedHandModel;
    private Animator handAnimator;
    void Start()
    {
        getDevices();

    }
    void Update()
    {
        if (!isValid)
        {
            getDevices();
        }
        else
        {
            spawnedHandModel.SetActive(true);
            UpdateAnimator();
        }
    }
    void getDevices()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        InputDevices.GetDevicesWithCharacteristics(controllerChrateristics, devices);

        if (devices.Count > 0)
        {
            isValid = true;
            // instantiate devices, models, and hand animator
            targetDevice = devices[0];
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }
    void UpdateAnimator()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
    // Update is called once per frame
}
