using UltimateReplay;
using UnityEngine;

namespace Rerun
{
    public class RerunPlaybackCameraManager : MonoBehaviour

    {
        // TODO - Clean up and improve. Separate freecam and camera management
        
        public Camera m_FreeCam = null;
        private Vector3 m_StartPosition;
        private Quaternion m_StartRotation;
        private Vector2 m_CamRotation = Vector2.zero;
        private const float k_LookMultiplier = 300;

        /// <summary>
        /// Should the free cam mode be enabled during playback.
        /// </summary>
        private bool m_AllowPlaybackFreeCam = true;

        /// <summary>
        /// How fast the free cam can move around the scene.
        /// </summary>
        private float m_FlySpeed = 2;

        /// <summary>
        /// How fast the free cam can look around the scene.
        /// </summary>
        private float m_LookSpeed = 0.8f;

        [SerializeField]
        private RenderTexture m_FreeCamRenderTexture;

        [SerializeField]
        private GameObject m_LayoutManager;

        public void Awake()
        {
            m_StartPosition = transform.position;
            m_StartRotation = transform.rotation;

            ReplayManager.ForceAwake();

            if (m_AllowPlaybackFreeCam == true)
            {
                // Find free cam
                m_FreeCam = GetComponent<Camera>();

                // Add the camera if one was not found
                if (m_FreeCam == null)
                    m_FreeCam = gameObject.AddComponent<Camera>();

                m_FreeCam.targetTexture = m_FreeCamRenderTexture;
                m_FreeCam.nearClipPlane = 0.1f;

                // Disable by default
                m_FreeCam.enabled = false;
                m_LayoutManager.SetActive(false);

            }
        }

        public void Update()
        {
            // Check for camera update
            if (m_AllowPlaybackFreeCam == false || m_FreeCam == null)
                return;

            // Get xy input
            float v = Input.GetKey(KeyCode.W) == true ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            float h = Input.GetKey(KeyCode.A) == true ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;

            // Move camera
            transform.Translate(Vector3.forward * m_FlySpeed * v * Time.deltaTime);
            transform.Translate(Vector3.right * m_FlySpeed * h * Time.deltaTime);

            // Check for mouse down
            if (Input.GetMouseButtonDown(1) == true)
            {
                // Prevent camera snapping when starting dragging - offset based upon initial rotation
                m_CamRotation.y = transform.localRotation.eulerAngles.y;
                m_CamRotation.x = transform.localRotation.eulerAngles.x;

                // Wrap angles
                if (m_CamRotation.y > 360)
                    m_CamRotation.y = 0;
            }

            // Check for mouse down
            if (Input.GetMouseButton(1) == true)
            {
                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                // Apply mouse with speed
                m_CamRotation.y += mouseX * m_LookSpeed * k_LookMultiplier * Time.deltaTime;
                m_CamRotation.x += mouseY * m_LookSpeed * k_LookMultiplier * Time.deltaTime;

                // Apply the rotation
                transform.rotation = Quaternion.Euler(m_CamRotation.x, m_CamRotation.y, 0);
            }
        }

        public void DisableCameras()
        {
            // Exit free cam
            ExitPlaybackFreeCam();
            m_LayoutManager.SetActive(false);
        }

        public void EnableCameras()
        {
            // Enable the free cam
            if (m_AllowPlaybackFreeCam)
            {
                EnterPlaybackFreeCam();
            }

            m_LayoutManager.SetActive(true);
        }
        

        // TODO - Replace / improve
        private void EnterPlaybackFreeCam()
        {
            // Require free cam
            if (m_FreeCam == null)
            {
                return;
            }

            Camera main = null;

            // Only one camera should be enabled
            Camera[] all = Component.FindObjectsOfType<Camera>();

            // Use the first camera
            for (int i = 0; i < all.Length; i++)
            {
                // Disregard any disabled cameras
                if (all[i].enabled == true)
                {
                    // Find first enabled
                    main = all[i];
                    break;
                }
            }

            // Check for a main camera found
            if (main != null)
            {
                // Get the positions
                transform.position = main.transform.position;
                transform.rotation = main.transform.rotation;
            }

            // Enable the free cam
            m_FreeCam.enabled = true;
        }

        // TODO - Replace / improve
        private void ExitPlaybackFreeCam()
        {
            // Require free cam
            if (m_FreeCam == null)
            {
                return;
            }

            m_FreeCam.enabled = false;

            // Reset position
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
    }
}