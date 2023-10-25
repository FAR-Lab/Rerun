using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rerun {
    [RequireComponent(typeof(Camera))]
    public class RerunFreeCameraControl : MonoBehaviour {
        // Start is called before the first frame update
        

        private Vector2 m_CamRotation = Vector2.zero;
        private const float k_LookMultiplier = 300;

        /// <summary>
        /// How fast the free cam can move around the scene.
        /// </summary>
        private float m_FlySpeed = 2;

        /// <summary>
        /// How fast the free cam can look around the scene.
        /// </summary>
        private float m_LookSpeed = 0.8f;


        private Camera m_FreeCam;

        void Start() {
            m_FreeCam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update() {
            // Check for camera update
            if ( m_FreeCam == null || m_FreeCam.enabled==false)
                return;

            // Get xy input
            float v = Input.GetKey(KeyCode.W) == true ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            float h = Input.GetKey(KeyCode.A) == true ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;

            // Move camera
            transform.Translate(Vector3.forward * m_FlySpeed * v * Time.deltaTime);
            transform.Translate(Vector3.right * m_FlySpeed * h * Time.deltaTime);

            // Check for mouse down
            if (Input.GetMouseButtonDown(1) == true) {
                // Prevent camera snapping when starting dragging - offset based upon initial rotation
                m_CamRotation.y = transform.localRotation.eulerAngles.y;
                m_CamRotation.x = transform.localRotation.eulerAngles.x;

                // Wrap angles
                if (m_CamRotation.y > 360)
                    m_CamRotation.y = 0;
            }

            // Check for mouse down
            if (Input.GetMouseButton(1) == true) {
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

        public void DisableCameras() {
            m_FreeCam.enabled = false;
        }

        public void EnableCameras() {
            m_FreeCam.enabled = true;
        }
    }
}