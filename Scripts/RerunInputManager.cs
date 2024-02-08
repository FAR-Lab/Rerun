using UnityEngine;

namespace Rerun
{
    [RequireComponent(typeof(RerunManager))]
    public class RerunInputManager : MonoBehaviour
    {
        private KeyCode m_LiveModeShortcut = KeyCode.L;
        private KeyCode m_RecordModeShortcut = KeyCode.R;
        private KeyCode m_PlayModeShortcut = KeyCode.P;
        private KeyCode m_OpenShortcut = KeyCode.O;

        private RerunManager m_RerunManager;

        void Start()
        {
            m_RerunManager = GetComponent<RerunManager>();
        }
        
        void Update()
        {
            // TODO - Improve input handling. This is mostly for testing
            
            if (IsReplayKeyPressed(m_RecordModeShortcut))
            {
                m_RerunManager.ToggleRecording();
            }
            if (IsReplayKeyPressed(m_LiveModeShortcut))
            {
                m_RerunManager.Live();
            }
            if (IsReplayKeyPressed(m_PlayModeShortcut))
            {
                m_RerunManager.Play();
            }
            if (IsReplayKeyPressed(m_OpenShortcut))
            {
                m_RerunManager.Open();
            }
            
            if (IsReplayKeyPressed(KeyCode.K))
            {
                m_RerunManager.BeginRecording("Standa0lone");
            }
            if (IsReplayKeyPressed(KeyCode.J))
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