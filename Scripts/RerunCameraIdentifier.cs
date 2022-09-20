using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerunCameraIdentifier : MonoBehaviour
{
    
    public enum CameraNumber
    {ONE=1, TWO=2, THREE=3,FOUR=4
        
    }

    public CameraNumber myNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    private CameraFollowMode mode = CameraFollowMode.Other;
    public Transform FollowTransform;
    private Vector3 Offset;
    private Quaternion OffsetRotation=Quaternion.identity;

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
    
  

    
}


