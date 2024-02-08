using System;
using System.IO;
using UltimateReplay.Storage;
using UnityEngine;
using UltimateReplay;

using UnityEngine.SceneManagement;


#if (UNITY_STANDALONE || UNITY_EDITOR)
using SimpleFileBrowser; //https://assetstore.unity.com/packages/tools/gui/runtime-file-browser-113006#description
#endif


namespace Rerun {
    /// <summary>
    /// The main Rerun class.
    /// </summary>
    public class RerunManager : MonoBehaviour {
        public enum StartUpMode {
            RECORDING,
            LIVE,
            REPLAY
        }

        private StartUpMode m_StartupMode;
        private ReplayStorageTarget m_MemoryTarget = new ReplayMemoryTarget();
        private ReplayHandle m_RecordHandle = ReplayHandle.invalid;
        private ReplayHandle m_PlaybackHandle = ReplayHandle.invalid;
        private ReplayFileTarget m_FileTarget;
        private RerunPlaybackCameraManager m_RerunPlaybackCameraManager;

        /// <summary>
        /// Property for accessing the record ReplayHandle from Ultimate Replay
        /// </summary>
        public ReplayHandle recordHandle => m_RecordHandle;

        /// <summary>
        /// Property for accessing the playback ReplayHandle from Ultimate Replay
        /// </summary>
        public ReplayHandle playbackHandle => m_PlaybackHandle;

        // Set to true for now, but his could be exposed in editor for flexibility
        private bool m_RecordToFile = true;

        // String prefix for file name. Use inspector, or property to set programmatically
        // For example, use to store session ID, user name, scene name etc., in the file name
        // TODO - Store information like this in the recording itself, or JSON
        [SerializeField] private string m_RecordingPrefix = "";

        private string folderName = "temp";

        /// <summary>
        /// String prefix for filenames of recordings
        /// </summary>
        public string recordingPrefix {
            get => m_RecordingPrefix;
            set => m_RecordingPrefix = value;
        }

        // This is the main VR rig. This should reference a Rerun prefab, containing a ReplayObject.
        [Tooltip("This is the main VR rig. This should reference a Rerun prefab, containing a ReplayObject.")]
        [SerializeField]
        private ReplayObject m_RigSource;

        // This is the clone VR rig, that will be replayed using data captured from the source rig.
        // See Ultimate Replay documentation on clones.
        [Tooltip("This is the clone VR rig, that will be replayed using data captured from the source rig.")]
        [SerializeField]
        private ReplayObject m_RigClone;

        // Information about the active replay mode, name of file being recorded/played etc.
        private string m_InfoString;
        private bool m_InitComplete;

        /// <summary>
        /// String containing information about the active replay mode, name of file being recorded/played etc.
        /// </summary>
        public string infoString {
            get => m_InfoString;
        }


        public void RerunInitialization(
            bool _DontDestroyOnLoad,
            RerunPlaybackCameraManager rpcm,
            StartUpMode sum) {
            if (_DontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            ReplayManager.ForceAwake();

            m_StartupMode = sum;

            m_RerunPlaybackCameraManager = rpcm;

            switch (m_StartupMode) {
                case StartUpMode.RECORDING:
                    GetComponent<RerunGUI>().enabled = false;
                    GetComponent<RerunInputManager>().enabled = false;
                    break;
                case StartUpMode.LIVE:
                    GetComponent<RerunGUI>().enabled = true;
                    GetComponent<RerunInputManager>().enabled = true;
                    break;
                case StartUpMode.REPLAY:
                    GetComponent<RerunGUI>().enabled = true;
                    GetComponent<RerunInputManager>().enabled = true;
                    break;
            }

            m_InfoString = "";
            m_InitComplete = true;
        }


        private void NotInitError() {
            Debug.LogError(
                "Did not Init Rerun please do so befor calling any other method. The method is called ``RerunInitialization``!");
        }

        /// <summary>
        /// Enter Live mode.
        /// </summary>
        public void Live() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }

            // Stop all recording
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                StopRecording();
            }

            // Stop all playback
            if (ReplayManager.IsReplaying(m_PlaybackHandle)) {
                StopPlayback();
            }

            m_RigSource.gameObject.SetActive(true);
            m_RigClone.gameObject.SetActive(false);

            m_InfoString = "Live view";
        }

        /// <summary>
        /// Toggles the recording state. Can be called from a single button used to start and stop recording.
        /// </summary>
        public void ToggleRecording() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // Start a fresh recording
            if (!ReplayManager.IsRecording(m_RecordHandle)) {
                BeginRecording();
            }
            else {
                // Stop recording and begin playback
                StopRecording();
                Play();
            }
        }

        /// <summary>
        /// Enter Play mode. This will play back any recorded data, from file or memory.
        /// </summary>
        public void Play() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }

            StopPlayback();
            if (m_RerunPlaybackCameraManager != null) {
                m_RerunPlaybackCameraManager.EnableCameras();
            }

            // Begin playback, based on target
            if (m_RecordToFile) {
                m_PlaybackHandle = ReplayManager.BeginPlayback(m_FileTarget, null, true);
                string[] filePath = m_FileTarget.FilePath.Split('/');
                m_InfoString = "Playing file: " + filePath[filePath.Length - 1];
            }
            else {
                m_PlaybackHandle = ReplayManager.BeginPlayback(m_MemoryTarget, null, true);
                m_InfoString = "Playing from memory";
            }
        }


        /// <summary>
        /// Should some other part of your software need to know what replayfile is being loaded you can register (and de_register)
        /// callback delegates here that get called before the file is loaded. Mostly used to load the scene before the file is loaded.
        /// </summary>
        public delegate void preLoadDelegate(string fileToBeLoaded);

        private preLoadDelegate handlers;

        public void RegisterPreLoadHandler(preLoadDelegate del) {
            handlers += del;
        }

        public void DeRegisterPreLoadHandler(preLoadDelegate del) {
            handlers -= del;
        }


        /// <summary>
        /// Open a file dialog to load .replay recordings. Starts playback immediately after opening.
        /// </summary>
        public void Open() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }


#if UNITY_STANDALONE || UNITY_EDITOR
            FileBrowser.SetDefaultFilter(".replay");
            FileBrowser.ShowLoadDialog((paths) => { InternalOpenFile(paths[0]); },
                () => { Debug.Log("Canceled file loading"); },
                FileBrowser.PickMode.Files,
                false,
                DataStoragePathSupervisor.GetReRunDirectory(),
                null,
                "Select one ReRun file",
                "Select");

#else
// on Android (Oculus Headset) we do not require ReRun so we exclude the execution here.
//  var filePath = "";
//  InternalOpenFile(filePath);
#endif
        }

        private void InternalOpenFile(string filePath) {
            if (handlers != null) {
                handlers.Invoke(filePath);
            }


            m_FileTarget = ReplayFileTarget.ReadReplayFile(filePath);

            Play();
        }

        public bool IsRecording() {
            if (!m_InitComplete) {
                NotInitError();
            }

            return ReplayManager.IsRecording(m_RecordHandle);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void StopPlayback() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }

            // If not playing then do nothing
            if (!ReplayManager.IsReplaying(m_PlaybackHandle)) {
                return;
            }

            ReplayManager.StopPlayback(ref m_PlaybackHandle);
          
            if (m_RerunPlaybackCameraManager != null) {
                m_RerunPlaybackCameraManager.DisableCameras();
            }
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        public void StopRecording() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If not recording then do nothing
            if (!ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }

            ReplayManager.StopRecording(ref m_RecordHandle);
            Debug.Log("Stopped Recording with length: " + m_FileTarget.Duration);
            m_RigClone.gameObject.SetActive(true);
            if (m_StartupMode == StartUpMode.LIVE) {
                ReplayObject.CloneReplayObjectIdentity(m_RigSource, m_RigClone);
                m_InfoString = "Live view";
            }
            else {
                m_InfoString = "Stopped Recording!";
            }
        }

        public void SetRecordingFolder(string val) {
            folderName = val;
        }

        public string GetRecordingFolder() {
            return folderName;
        }

        public void BeginRecording(string Prefix) {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            m_RecordingPrefix = Prefix;
            BeginRecording();
        }
        

        /// <summary>
        /// Begin recording.
        /// </summary>
        private void BeginRecording() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            // If recording then do nothing (recording must be stopped first)
            if (ReplayManager.IsRecording(m_RecordHandle)) {
                return;
            }

            StopPlayback();

            if (m_RecordToFile) {
                string fileName = m_RecordingPrefix + "_Rerun_" +
                                  DateTime.Now.ToString(DataStoragePathSupervisor.DateTimeFormatFolder) + ".replay";


                string path = DataStoragePathSupervisor.GetReRunDirectory();

                m_FileTarget = ReplayFileTarget.CreateReplayFile(Path.Join(path , fileName));
                Debug.Log("RecordingToFile" + path + fileName);

                if (m_FileTarget.MemorySize > 0) {
                    m_FileTarget.PrepareTarget(ReplayTargetTask.Discard);
                }


                foreach (var r_obj in FindObjectsOfType<ReplayObject>()) {
                    if (r_obj.transform.root.gameObject.scene!=SceneManager.GetActiveScene()) {
                        SceneManager.MoveGameObjectToScene(r_obj.transform.root.gameObject,
                            SceneManager.GetActiveScene());
                    }
                }

                m_RecordHandle = ReplayManager.BeginRecording(m_FileTarget, null, false, true);

                m_InfoString = "Recording file: " + fileName;
            }
            else {
                // Clear old data
                if (m_MemoryTarget.MemorySize > 0) {
                    m_MemoryTarget.PrepareTarget(ReplayTargetTask.Discard);
                }

                m_RecordHandle = ReplayManager.BeginRecording(m_MemoryTarget, null, false, true);
                m_InfoString = "Recording into memory";
            }
        }

        public void DisableAllReRunCameras() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            if (m_RerunPlaybackCameraManager != null) {
                m_RerunPlaybackCameraManager.DisableCameras();
            }
        }

        public void EnableAllReRunCameras() {
            if (!m_InitComplete) {
                NotInitError();
                return;
            }

            if (m_RerunPlaybackCameraManager != null) {
                m_RerunPlaybackCameraManager.EnableCameras();
            }
        }
    }
}