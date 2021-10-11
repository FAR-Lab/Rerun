using UltimateReplay.Statistics;
using UnityEngine;

namespace Rerun
{
    /// <summary>
    /// Simple util component that can be added to a game object and will display the total usage size of all storage targets combined.
    /// This component uses the legacy IMGUI APi to display the information.
    /// </summary>
    public class RerunStatistics : MonoBehaviour
    {
        // Methods
        public void OnGUI()
        {
            // Get memory usage
            int memoryUsage = ReplayStorageTargetStatistics.CalculateReplayMemoryUsage();
            GUIStyle style = new GUIStyle();
            style.fontSize = 30;
            
            GUILayout.Space(32);
            GUILayout.Label("Replay Memory Usage: " + ReplayStatisticsUtil.GetMemorySizeSmallestUnit(memoryUsage) + ReplayStatisticsUtil.GetMemoryUnitString(memoryUsage),style);
        }
    }
}