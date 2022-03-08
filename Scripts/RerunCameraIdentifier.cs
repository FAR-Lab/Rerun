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

    private CameraSetup.CameraState mode = CameraSetup.CameraState.Other;
    private Transform FollowTransform;
    private Vector3 Offset;
    private Quaternion OffsetRotation=Quaternion.identity;

    public void SetFollowMode(Transform t, Vector3 off,Vector3 OffsetRotationEuler)
    {
        FollowTransform = t;
        Offset = off;
        OffsetRotation = Quaternion.Euler(OffsetRotationEuler);
        mode = CameraSetup.CameraState.Followone;

    }
    public void UnSetFollowMode()
    {
        FollowTransform = null;
        Offset = Vector3.zero;
        OffsetRotation=Quaternion.identity;
        mode = CameraSetup.CameraState.Other;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (mode)
        {
            case CameraSetup.CameraState.Followone:
                var transform1 = transform;
                var rotation = FollowTransform.rotation;
                transform1.rotation = rotation * OffsetRotation;
                transform1.position = FollowTransform.position + rotation * Offset;
                
                break;
            case CameraSetup.CameraState.Followmultiple:
                break;
            case CameraSetup.CameraState.Fixed:
                break;
            case CameraSetup.CameraState.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        


       
    }
    
    
    [Serializable]
    public struct CameraSetup
    {
        public enum CameraState
        {
            Followone,
            Followmultiple,
            Fixed,
            Other
        }
    
        public CameraState CameraMode;
        public ParticipantOrder ParticipantToFollow;
        public Vector3 PositionOrOffset;
        public Vector3 RotationOrRot_Offset;
    }
}


