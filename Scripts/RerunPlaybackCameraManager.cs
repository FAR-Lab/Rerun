//#define DEBUGRERUNCAMERAS

using System.Collections.Generic;
using System.Linq;
using UltimateReplay;
using UnityEngine;

namespace Rerun {
    public class RerunPlaybackCameraManager : MonoBehaviour {

        public RerunLayoutManager m_LayoutManager;
        private Dictionary<RerunCameraIdentifier.CameraNumber, RerunCameraIdentifier> m_Cameras;

        public void Awake() {


            m_Cameras = new Dictionary<RerunCameraIdentifier.CameraNumber, RerunCameraIdentifier>();
            foreach (RerunCameraIdentifier v in GetComponentsInChildren<RerunCameraIdentifier>()) {
                if (m_Cameras.ContainsKey(v.myNumber)) {
                    
                     Debug.LogError($"multiple Cameras with the same identifier.{v.myNumber} at {v.transform.name} conflict with {m_Cameras[v.myNumber]}");
                }
                else {
#if DEBUGRERUNCAMERAS
                    Debug.Log("Found Camera Number: " + v.myNumber);
#endif 
                    m_Cameras.Add(v.myNumber, v);
                }
                
            }
        }


        public void EnableCameras() {
            m_LayoutManager.enabled = true;
            foreach (var rerunCameraIdentifier in m_Cameras.Values) {
                rerunCameraIdentifier.EnableCamera();
            }
        }

        public void DisableCameras() {

            m_LayoutManager.enabled = false;
            foreach (var rerunCameraIdentifier in m_Cameras.Values) {
                rerunCameraIdentifier.DisableCamera();
            }
        }




        private void EnterPlaybackFreeCam() { }

        // TODO - Replace / improve
        private void ExitPlaybackFreeCam() { }

        public Transform GetFollowCamera() {
            return m_Cameras.Values.Where(x => x.AdjustableCamera).FirstOrDefault().transform;
        }

        public void DeLinkCameras() {
            foreach (var rerunCameraIdentifier in m_Cameras.Values) {
                rerunCameraIdentifier.DelinkFollowTransforms();
            }
        }

        public void LinkCameras() {
            string scene = ConnectionAndSpawning.Singleton.GetLoadedScene();
            if (scene != ConnectionAndSpawning.WaitingRoomSceneName) {
                ScenarioManager tmp = ConnectionAndSpawning.Singleton.GetScenarioManager();
                if (tmp == null) return;
                foreach (CameraSetupXC cameraSetupXc in tmp.CameraSetups) {
                    Debug.Log("Going through cameras " + cameraSetupXc.CameraMode.ToString());
                    if (m_Cameras.ContainsKey(cameraSetupXc.targetNumber)) {
                        Transform val =
                            ConnectionAndSpawning.Singleton.GetClientMainCameraObject(cameraSetupXc
                                .ParticipantToFollow);
                        ApplyValues(cameraSetupXc, m_Cameras[cameraSetupXc.targetNumber], val);
                    }
                }

            }
        }

        public void ApplyValues(CameraSetupXC setup, RerunCameraIdentifier target, Transform followObject = null) {

            switch (setup.CameraMode) {
                case RerunCameraIdentifier.CameraFollowMode.Followone:
                    target.SetFollowMode(followObject, setup.PositionOrOffset, setup.RotationOrRot_Offset);
                    break;
                case RerunCameraIdentifier.CameraFollowMode.Followmultiple:
                    break;
                case RerunCameraIdentifier.CameraFollowMode.Fixed:
                    break;
                case RerunCameraIdentifier.CameraFollowMode.Other:
                    break;

            }
        }
    }
}