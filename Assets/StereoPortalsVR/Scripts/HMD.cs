using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StereoPortalsVR
{
    public class HMD : MonoBehaviour
    {
        [SerializeField] private Transform leftEye;
        [SerializeField] private Transform rightEye;

        public Camera cam;

        [HideInInspector] public Transform[] eyes = new Transform[2];

        void Start()
        {
            eyes[0] = leftEye;
            eyes[1] = rightEye;
        }
    }
}