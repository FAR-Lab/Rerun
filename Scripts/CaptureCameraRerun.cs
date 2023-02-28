using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rerun;
using UnityEngine;

public class CaptureCameraRerun : MonoBehaviour
{
    private Transform CameraTransform;

    private bool TrackingActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) &&
            Input.GetKeyUp(KeyCode.V) &&
            Input.GetKeyUp(KeyCode.P))
        {
            if (!TrackingActive)
            {
                CameraTransform =FindObjectsOfType<RerunCameraIdentifier>()
                    .Where(x => x.myNumber == RerunCameraIdentifier.CameraNumber.FOUR)
                    .First().transform;
                if (CameraTransform != null)
                {
                    CameraTransform.forward = transform.forward;
                   
                    TrackingActive = true;
                }

            }
            else
            {
                TrackingActive = false;
                CameraTransform = null;
            }
        }
    }

    private void LateUpdate()
    {
        if (TrackingActive)
        {
            CameraTransform.position = transform.position;
        }
        
    }
}
