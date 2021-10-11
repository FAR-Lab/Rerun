using System;
using UltimateReplay.Storage;
using UnityEngine;
using UltimateReplay;
using UltimateReplay.Statistics;

namespace Rerun
{
    [RequireComponent(typeof(RerunManager))]
    // <summary>
    // UI for replay controls. Partially built from ReplayControl, extracting UI only.
    // </summary>
    public class RerunGUI : MonoBehaviour
    {
        
        private float m_PlayPauseWidth = 24;
        private float m_PlayPauseHeight = 20;
        private float m_StateButtonWidth = 100;
        private float m_StateButtonHeight = 18;
        private readonly Color m_Normal = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        private readonly Color m_Highlight = Color.green;
        private readonly Color m_HighlightRecording = Color.red;
        private readonly Color s_FontHighlight = Color.white;

        private ReplayHandle m_RecordHandle = ReplayHandle.invalid;
        private ReplayHandle m_PlaybackHandle = ReplayHandle.invalid;

        private bool m_ShowSettings = false;
        private bool m_ReversePlay = false;
        private Texture2D m_PlayTexture = null;
        private Texture2D m_PauseTexture = null;
        private Texture2D m_SettingsTexture = null;

        public int m_UIScale = 2;
        private RerunManager m_RerunManager;

        private string m_InfoString = "";

        public void Awake()
        {
            // Find or create a replay manager
            ReplayManager.ForceAwake();
            m_RerunManager = GetComponent<RerunManager>();
            
        }

        public void Start()
        {
            m_PlayPauseWidth = 24 * m_UIScale;
            m_PlayPauseHeight = 20 * m_UIScale;
            m_StateButtonWidth = 48 * m_UIScale;
            m_StateButtonHeight = 18 * m_UIScale;

            m_PlayTexture = Resources.Load<Texture2D>("PlayIcon");
            m_PauseTexture = Resources.Load<Texture2D>("PauseIcon");
            m_SettingsTexture = Resources.Load<Texture2D>("SettingsIcon");
        }

        public void OnGUI()
        {
            // Default label style
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = 20;

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 20;

            // Create the gui screen area
            GUILayout.BeginArea(new Rect(10 * m_UIScale * 2, 10 * m_UIScale, Screen.width - 20 * m_UIScale * 2,
                Screen.height - 20 * m_UIScale));

            {
                GUILayout.BeginHorizontal();
                {
                    // Create a button style
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

                    buttonStyle.fontSize = 8 * m_UIScale;
                    buttonStyle.padding = new RectOffset(3 * m_UIScale, 3 * m_UIScale, 3 * m_UIScale, 3 * m_UIScale);
                    buttonStyle.margin = new RectOffset(-1 * m_UIScale, -1 * m_UIScale, 0, 0);

                    ///////////
                    // Record
                    ///////////
                    
                    GUI.color = (ReplayManager.IsRecording(m_RecordHandle) == true) ? m_HighlightRecording : m_Normal;

                    m_RecordHandle = m_RerunManager.recordHandle;
                    if (!ReplayManager.IsRecording(m_RecordHandle))
                    {
                        GUILayout.Box(new GUIContent("Rec (R)", "Begin recording"), boxStyle,
                            GUILayout.Width(m_StateButtonWidth), GUILayout.Height(m_StateButtonHeight));
                    }
                    else
                    {
                        GUILayout.Box(new GUIContent("Stop (R)", "Stop recording"), boxStyle,
                            GUILayout.Width(m_StateButtonWidth), GUILayout.Height(m_StateButtonHeight));
                    }

                    ///////////
                    // Playback
                    ///////////
                    
                    // Show only if not recording 
                    if (!ReplayManager.IsRecording(m_RecordHandle))
                    {
                        GUI.color = (ReplayManager.IsReplaying(m_PlaybackHandle) == true) ? m_Highlight : m_Normal;

                        GUILayout.Box(new GUIContent("Play (P)", "Begin playback"), boxStyle,
                            GUILayout.Width(m_StateButtonWidth), GUILayout.Height(m_StateButtonHeight));
                    }

                    ///////////
                    // Live
                    ///////////
                    
                    // Show only if not recording
                    if (!ReplayManager.IsRecording(m_RecordHandle))
                    {
                        GUI.color = (ReplayManager.IsRecording(m_RecordHandle) == false &&
                                     ReplayManager.IsReplaying(m_PlaybackHandle) == false)
                            ? m_Highlight
                            : m_Normal;

                        GUILayout.Box(new GUIContent("Live (L)", "Live mode"), boxStyle,
                            GUILayout.Width(m_StateButtonWidth),
                            GUILayout.Height(m_StateButtonHeight));
                    }

                    ///////////
                    // Open
                    ///////////
                    
                    // Show only if not recording
                    if (!ReplayManager.IsRecording(m_RecordHandle))
                    {
                        GUI.color = m_Normal;

                        GUILayout.Box(new GUIContent("Open (O)", "Open playback file"), boxStyle,
                            GUILayout.Width(m_StateButtonWidth), GUILayout.Height(m_StateButtonHeight));
                    }

                    ///////////
                    // Timer
                    ///////////
                    
                    GUI.color = m_Highlight;
                    
                    // Show only if recording
                    if (ReplayManager.IsRecording(m_RecordHandle) == true)
                    {
                        // Get the storage target
                        ReplayStorageTarget target = ReplayManager.GetReplayStorageTarget(m_RecordHandle);

                        string recordTime =
                            ReplayTime.GetCorrectedTimeValueString((target != null) ? target.Duration : 0);

                        GUI.color = Color.red;
                        GUILayout.Box(string.Format("Recording: {0}", recordTime), boxStyle,
                            GUILayout.Width(m_StateButtonWidth * 4), GUILayout.Height(m_StateButtonHeight));
                    }

                    ///////////
                    // Info string
                    ///////////
                    
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.white;
                    GUILayout.Label(m_RerunManager.infoString, labelStyle);
                }
                GUILayout.EndHorizontal();


                ///////////
                // Playback media controls
                ///////////

                // Push to bottom
                GUILayout.FlexibleSpace();

                // TODO - This is not so clean currently but this level of data access is required here for visualizing the timeline

                if (ReplayManager.IsReplaying(m_RerunManager.playbackHandle) == true)
                {
                    m_PlaybackHandle = m_RerunManager.playbackHandle;
                    // Get the playback source
                    ReplayStorageTarget target = ReplayManager.GetReplayStorageTarget(m_PlaybackHandle);

                    // Get the playback time
                    ReplayTime playbackTime = ReplayManager.GetPlaybackTime(m_PlaybackHandle);

                    GUILayout.BeginHorizontal();
                    {
                        // Check for paused
                        if (ReplayManager.IsPlaybackPaused(m_PlaybackHandle) == true)
                        {
                            // Draw a play button
                            if (GUILayout.Button(m_PlayTexture, GUILayout.Width(m_PlayPauseWidth),
                                    GUILayout.Height(m_PlayPauseHeight)) == true)
                            {
                                // Resume the replay
                                ReplayManager.ResumePlayback(m_PlaybackHandle);
                            }
                        }
                        else
                        {
                            // Draw a pause button
                            if (GUILayout.Button(m_PauseTexture, GUILayout.Width(m_PlayPauseWidth),
                                    GUILayout.Height(m_PlayPauseHeight)) == true)
                            {
                                // Pause playback
                                ReplayManager.PausePlayback(m_PlaybackHandle);
                            }
                        }

                        // Slider space
                        GUILayout.BeginVertical();
                        {
                            // Push down slightly
                            GUILayout.Space(10 * m_UIScale);

                            float input = playbackTime.NormalizedTime;

                            // Draw the seek slider
                            float output = GUILayout.HorizontalSlider(input, 0, 1, GUILayout.Height(m_PlayPauseHeight));

                            // Check for change
                            if (input != output)
                            {
                                // Seek to recording location
                                ReplayManager.SetPlaybackTimeNormalized(m_PlaybackHandle, output, PlaybackOrigin.Start);
                            }
                        }
                        GUILayout.EndVertical();

                        // Settings button
                        if (GUILayout.Button(new GUIContent(m_SettingsTexture, "Open playback settings"),
                            GUILayout.Width(m_PlayPauseWidth), GUILayout.Height(m_PlayPauseHeight)) == true)
                        {
                            // Toggle settings
                            m_ShowSettings = !m_ShowSettings;
                        }

                        // Check for settings window
                        if (m_ShowSettings == true)
                        {
                            Rect area = new Rect(Screen.width - 360, Screen.height - 150, 240, 50);

                            DrawGUISettings(area);
                        }

                        string currentTime = ReplayTime.GetCorrectedTimeValueString(playbackTime.Time);
                        string totalTime =
                            ReplayTime.GetCorrectedTimeValueString((target != null) ? target.Duration : 0);

                        GUILayout.Label(string.Format("{0} / {1}", currentTime, totalTime), GUI.skin.button,
                            GUILayout.Width(75));
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
        }

        private void DrawGUISettings(Rect area)
        {
            GUILayout.BeginArea(area, GUI.skin.box);
            {
                ReplayTime playbackTime = ReplayManager.GetPlaybackTime(m_PlaybackHandle);

                // Playback speed
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Speed:", GUILayout.Width(55));
                    float timeScale = GUILayout.HorizontalSlider(playbackTime.TimeScale, 0, 2);

                    // Update the time scale
                    if (timeScale != playbackTime.TimeScale)
                        ReplayManager.SetPlaybackTimeScale(m_PlaybackHandle, timeScale);
                }
                GUILayout.EndHorizontal();

                // Playback direction
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Reverse:", GUILayout.Width(55));
                    bool result = GUILayout.Toggle(m_ReversePlay, string.Empty);

                    // Chcck for change
                    if (result != m_ReversePlay)
                    {
                        m_ReversePlay = result;

                        // Check if we are currently replaying
                        if (ReplayManager.IsReplaying(m_PlaybackHandle) == true)
                        {
                            // Set direction for playback
                            ReplayManager.SetPlaybackDirection(m_PlaybackHandle, (m_ReversePlay == true)
                                ? ReplayManager.PlaybackDirection.Backward
                                : ReplayManager.PlaybackDirection.Forward);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
    }
}