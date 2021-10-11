using UltimateReplay.Storage;
using UnityEngine;
using UltimateReplay;
using UnityEditor;
using UnityEngine.UI;

namespace Rerun
{
    [RequireComponent(typeof(RerunPlaybackCameraManager))]
    public class RerunManager : MonoBehaviour
    {
        private ReplayStorageTarget m_MemoryTarget = new ReplayMemoryTarget();
        private ReplayHandle m_RecordHandle = ReplayHandle.invalid;
        private ReplayHandle m_PlaybackHandle = ReplayHandle.invalid;
        private ReplayFileTarget m_FileTarget;
        private RerunPlaybackCameraManager m_RerunPlaybackCameraManager;

        // Properties
        public ReplayHandle recordHandle => m_RecordHandle;

        public ReplayHandle playbackHandle => m_PlaybackHandle;

        // Turning serialization off for safety
        private bool m_RecordToFile = true;

        // TODO - Create editor / inspector script for this
        [SerializeField]
        private string m_SessionIdentifier = "";

        // This example assumes that a replay object has been assigned for recording
        [SerializeField]
        private ReplayObject m_RigSource;

        // Assign a secondary object that will be replayed using data captured from the recordObject
        [SerializeField]
        private ReplayObject m_RigClone;

        private string m_InfoString;

        public string infoString
        {
            get => m_InfoString;
        }

        public void Awake()
        {
            // Find or create a replay manager
            ReplayManager.ForceAwake();
            m_RerunPlaybackCameraManager = GetComponent<RerunPlaybackCameraManager>();

            m_InfoString = "";
        }

        public void Live()
        {
            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }

            // Stop all recording
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                StopRecording();
            }

            // Stop all playback
            if (ReplayManager.IsReplaying(m_PlaybackHandle))
            {
                StopPlayback();
            }

            m_RigSource.gameObject.SetActive(true);
            m_RigClone.gameObject.SetActive(false);

            m_InfoString = "Live view";
        }

        public void ToggleRecording()
        {

            // Start a fresh recording
            if (!ReplayManager.IsRecording(m_RecordHandle))
            {
                BeginRecording();
            }
            else
            {
                // Stop recording and begin playback
                StopRecording();
                Play();
            }
        }

        public void Play()
        {
            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }
            StopPlayback();

            m_RerunPlaybackCameraManager.EnableCameras();

            // Begin playback, based on target
            if (m_RecordToFile)
            {
                m_PlaybackHandle = ReplayManager.BeginPlayback(m_FileTarget, null, true);
                string[] filePath = m_FileTarget.FilePath.Split('/');
                m_InfoString = "Playing file: " + filePath[filePath.Length - 1];
            }
            else
            {
                m_PlaybackHandle = ReplayManager.BeginPlayback(m_MemoryTarget, null, true);
                m_InfoString = "Playing from memory";
            }
        }

        public void Open()
        {
            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }

            var filePath = EditorUtility.OpenFilePanel("Choose Input Event Trace to Load", string.Empty, "replay");
            m_FileTarget = ReplayFileTarget.ReadReplayFile(filePath);
            Play();
        }

        private void StopPlayback()
        {
            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }
            
            // If not playing then do nothing
            if (!ReplayManager.IsReplaying(m_PlaybackHandle))
            {
                return;
            }
            
            ReplayManager.StopPlayback(ref m_PlaybackHandle);
            m_RerunPlaybackCameraManager.DisableCameras();
        }
        
        public void StopRecording()
        {
            // If not recording then do nothing
            if (!ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }

            ReplayManager.StopRecording(ref m_RecordHandle);
            m_RigClone.gameObject.SetActive(true);
            ReplayObject.CloneReplayObjectIdentity(m_RigSource, m_RigClone);
            m_InfoString = "Live view";
            
        }


        public void BeginRecording()
        {
            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle))
            {
                return;
            }

            StopPlayback();

            if (m_RecordToFile)
            {
                string fileName = m_SessionIdentifier + "_Rerun_" +
                                  System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".replay";
                m_FileTarget = ReplayFileTarget.CreateReplayFile(Application.persistentDataPath + "/" + fileName);

                if (m_FileTarget.MemorySize > 0)
                {
                    m_FileTarget.PrepareTarget(ReplayTargetTask.Discard);
                }

                m_RecordHandle = ReplayManager.BeginRecording(m_FileTarget, null, false, true);
                m_InfoString = "Recording file: " + fileName;
            }
            else
            {
                // Clear old data
                if (m_MemoryTarget.MemorySize > 0)
                {
                    m_MemoryTarget.PrepareTarget(ReplayTargetTask.Discard);
                }

                m_RecordHandle = ReplayManager.BeginRecording(m_MemoryTarget, null, false, true);
                m_InfoString = "Recording into memory";
            }
        }
    }
}