using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class RerunCameraIdentifier : MonoBehaviour
{
    
    public enum CameraNumber
    {ONE=1, TWO=2, THREE=3,FOUR=4
        
    }

    public CameraNumber myNumber;
    
    // Start is called before the first frame update
    void Start() {
        m_Camera = GetComponent<Camera>();
        m_AudioListener = GetComponent<AudioListener>();
    }

    
    private CameraFollowMode mode = CameraFollowMode.Other;
    public Transform FollowTransform;
    public bool AdjustableCamera;
    public float m_FlySpeed = 25;
    private Vector3 Offset;
    private Quaternion OffsetRotation=Quaternion.identity;

    private Camera m_Camera;
    private AudioListener m_AudioListener;

    public void SetFollowMode(Transform t, Vector3 off,Vector3 OffsetRotationEuler)
    {
        FollowTransform = t;
        Offset = off;
        OffsetRotation = Quaternion.Euler(OffsetRotationEuler);
        mode = CameraFollowMode.Followone;

    }
    public void UnSetFollowMode()
    {
        FollowTransform = null;
        Offset = Vector3.zero;
        OffsetRotation=Quaternion.identity;
        mode = CameraFollowMode.Other;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (mode)
        {
            case CameraFollowMode.Followone:
                if (FollowTransform == null) return;
                var transform1 = transform;
                var rotation = FollowTransform.rotation;
                transform1.rotation = rotation * OffsetRotation;
                transform1.position = FollowTransform.position + rotation * Offset;

                break;
        }

        if (AdjustableCamera)
            {
                
                var transform2 = transform;
                
                if (Input.GetMouseButton(0) == true)
                {
                    // Get mouse input
                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = -Input.GetAxis("Mouse Y");

                    // Apply mouse with speed
                    transform2.Translate(Vector3.left * m_FlySpeed * mouseX * Time.deltaTime);
                    transform2.Translate(Vector3.up * m_FlySpeed * mouseY * Time.deltaTime);
                }

                transform2.Translate(Vector3.forward * m_FlySpeed * Input.mouseScrollDelta.y * Time.deltaTime);
            }
        
       
    }
    public void DelinkFollowTransforms()
    {
        FollowTransform = null;
    }
    
    public enum CameraFollowMode
    {
        Followone,
        Followmultiple,
        Fixed,
        Other
    }

    public void EnableCamera() {
        m_Camera.enabled = true;
        if(m_AudioListener!=null)
        {
            m_AudioListener.enabled = true;
        }

    }

    public void DisableCamera() {
        m_Camera.enabled = false;
        if(m_AudioListener!=null)
        {
            m_AudioListener.enabled = false;
        }
    }
    
  

    
}


