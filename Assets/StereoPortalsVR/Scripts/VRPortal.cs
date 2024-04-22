using UnityEngine;

namespace StereoPortalsVR
{
    public class VRPortal : MonoBehaviour
    {
        [SerializeField] private HMD playerHMD;
        [SerializeField] private Transform targetPortal;
        [SerializeField] private int maxRecursion;
        [SerializeField] private Vector2Int renderTextureSize;

        private readonly Camera[] cameras = new Camera[2];
        private Transform portalRenderer;
        private Renderer targetPortalRendComp;

        void Start()
        {
            cameras[0] = transform.Find("LeftCamera").GetComponent<Camera>();
            cameras[1] = transform.Find("RightCamera").GetComponent<Camera>();

            portalRenderer = transform.Find("PortalRenderer");
            targetPortalRendComp = targetPortal.Find("PortalRenderer").GetComponent<MeshRenderer>();
            Invoke(nameof(InitializeCameras), 0.01f);
        }

        void InitializeCameras()
        {
            RenderTexture leftEyeTexture = new(renderTextureSize.x, renderTextureSize.y, 0);
            RenderTexture rightEyeTexture = new(renderTextureSize.x, renderTextureSize.y, 0);
            cameras[0].enabled = false;
            cameras[1].enabled = false;
            cameras[0].targetTexture = leftEyeTexture;
            cameras[1].targetTexture = rightEyeTexture;
            cameras[0].fieldOfView = playerHMD.cam.fieldOfView;
            cameras[1].fieldOfView = playerHMD.cam.fieldOfView;
            targetPortalRendComp.material.SetTexture("_LeftEyeTex", leftEyeTexture);
            targetPortalRendComp.material.SetTexture("_RightEyeTex", rightEyeTexture);
        }

        void Update()
        {
            for (int i = 0; i < 2; i++)
            {
                Matrix4x4 playerMatrix = playerHMD.eyes[i].localToWorldMatrix;
                Matrix4x4[] matrices = new Matrix4x4[maxRecursion];
                for (int j = 0; j < maxRecursion; j++)
                {
                    playerMatrix = transform.localToWorldMatrix * targetPortal.worldToLocalMatrix * playerMatrix;
                    matrices[j] = playerMatrix;
                }
                RenderCamera(matrices, i);
            }
        }

        void RenderCamera(Matrix4x4[] matrices, int camIndex)
        {
            if (IsInView())
            {
                targetPortalRendComp.material.SetInt("_RightOverwrite", camIndex);
                portalRenderer.gameObject.SetActive(false);

                for (int i = maxRecursion - 1; i >= 0; i--)
                {
                    cameras[camIndex].transform.SetPositionAndRotation(matrices[i].GetColumn(3), matrices[i].rotation);
                    cameras[camIndex].projectionMatrix = playerHMD.cam.GetStereoProjectionMatrix
                    (camIndex == 0 ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right);
                    SetProjectionMatrix(cameras[camIndex]);
                    cameras[camIndex].Render();
                }

                targetPortalRendComp.material.SetInt("_RightOverwrite", 0);
                portalRenderer.gameObject.SetActive(true);
            }
        }

        void SetProjectionMatrix(Camera cam)
        {
            Transform clipPlane = transform;
            int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - cam.transform.position));
            Vector3 camSpacePos = cam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
            Vector3 camSpaceNormal = cam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
            float camSpaceDistance = -Vector3.Dot(camSpacePos, camSpaceNormal);
            Vector4 clipPlaneMatrix = new(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDistance);
            cam.projectionMatrix = cam.CalculateObliqueMatrix(clipPlaneMatrix);
        }

        public bool IsInView()
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerHMD.cam);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, targetPortalRendComp.bounds);
        }
    }
}
