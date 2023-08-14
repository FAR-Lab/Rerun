using UnityEngine;

namespace Rerun
{
    public class RerunInputManager : MonoBehaviour
    {
        private KeyCode m_LiveModeShortcut = KeyCode.L;
        private KeyCode m_RecordModeShortcut = KeyCode.R;
        private KeyCode m_PlayModeShortcut = KeyCode.P;
        private KeyCode m_OpenShortcut = KeyCode.O;

        private RerunManager m_RerunManager;

        [HideInInspector]
        public bool InputOpen = false;
        void Start()
        {
            m_RerunManager = GetComponent<RerunManager>();
        }
        
        void Update()
        {
            // TODO - Improve input handling. This is mostly for testing
            // There are more inputs in timeline.cs under the EVERYTHING prefab
            // There are more inputs in RerunLayoutManager.cs
            if (IsReplayKeyPressed(m_RecordModeShortcut) && !InputOpen)
            {
                m_RerunManager.ToggleRecording();
            }
            if (IsReplayKeyPressed(m_LiveModeShortcut) && !InputOpen)
            {
                m_RerunManager.Live();
            }
            if (IsReplayKeyPressed(m_PlayModeShortcut) && !InputOpen)
            {
                m_RerunManager.Play();
            }
            if (IsReplayKeyPressed(m_OpenShortcut) && !InputOpen)
            {
                m_RerunManager.Open();
            }
            
            if (IsReplayKeyPressed(KeyCode.K) && !InputOpen)
            {
                m_RerunManager.BeginRecording();
            }
            if (IsReplayKeyPressed(KeyCode.J) && !InputOpen)
            {
                m_RerunManager.StopRecording();
            }
            
        }

        private bool IsReplayKeyPressed(KeyCode key)
        {
            // Check for key press
            return Input.GetKeyDown(key);
        }
    }
}