using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UltimateReplay;
using UltimateReplay.Core;
using UltimateReplay.Serializers;
using UltimateReplay.Storage;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

namespace Rerun
{
    public class RerunVisualizer : MonoBehaviour
    {
        [SerializeField]
        private RerunManager m_RerunManager;

        // Number of samples used for visualization
        [SerializeField]
        private float m_Samples = 20f;

        // Objects to be visualized
        [SerializeField]
        private List<GameObject> m_ReplayTransformObjects;

        private float m_NormalizedTime = 0;
        
        
        List<Vector3> m_Positions0;
        List<Vector3> m_Positions1;
        List<Quaternion> m_Rotations0;
        List<Quaternion> m_Rotations1;

        private List<float> m_DotProducts0;

        
        public float normalizedTime
        {
            get => m_NormalizedTime;
        }

        public List<float> GetDotProduct()
        {
            return m_DotProducts0;
        }

        // Start is called before the first frame update
        void Start()
        {
            //InitObjects();

            m_Positions0 = new List<Vector3>();
            m_Positions1 = new List<Vector3>();
            m_Rotations0 = new List<Quaternion>();
            m_Rotations1 = new List<Quaternion>();

            m_DotProducts0 = new List<float>();
            
        }

        private void Update()
        {
            if (ReplayManager.IsReplayingAny)
            {
                ReplayHandle handle = m_RerunManager.playbackHandle;
                ReplayStorageTarget target = ReplayManager.GetReplayStorageTarget(handle);
                float duration = target.Duration;
                float step = duration / m_Samples;

                // Get the playback time
                ReplayTime playbackTime = ReplayManager.GetPlaybackTime(handle);
                m_NormalizedTime = playbackTime.NormalizedTime;
            }
        }

        private void InitObjects()
        {
            m_Positions0.Clear();
            m_Positions1.Clear();
            m_Rotations0.Clear();
            m_Rotations1.Clear();
            
            // Add LineRenderer to each object to be visualized
            foreach (var go in m_ReplayTransformObjects)
            {
                LineRenderer lr = go.GetComponent<LineRenderer>();
                if (lr== null)
                {
                    lr = go.AddComponent<LineRenderer>();    
                };
                
                //var lr = go.AddComponent<TrailRenderer>();
                lr.material = new Material(Shader.Find("Unlit/Color"));
                lr.material.color = Color.blue;
                lr.positionCount = 0;
                lr.startWidth = 0.01f;
                lr.endWidth = 0.01f;
                Color c1 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1);
                Color c2 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1);
                
                lr.SetColors(c1, c2);
            }
        }

        public void SetupVisualizers()
        {
            //HH test
            // m_ReplayTransformObjects.Clear();
            // GameObject clone = GameObject.FindGameObjectWithTag("Clone");
            // m_ReplayTransformObjects.Add(clone);
            InitObjects();
            //
            if (ReplayManager.IsReplayingAny)
            {
                List<Vector3> positions = new List<Vector3>();
                ReplayHandle handle = m_RerunManager.playbackHandle;
                ReplayStorageTarget target = ReplayManager.GetReplayStorageTarget(handle);
                float duration = target.Duration;
                float step = duration / m_Samples;
                
                // Get the playback time
                // ReplayTime playbackTime = ReplayManager.GetPlaybackTime(handle);
                // float time = playbackTime.NormalizedTime;
                
//                m_NormalizedTime = playbackTime.NormalizedTime;

                int counter = 0;

                foreach (var go in m_ReplayTransformObjects)
                {
                    positions.Clear();
                    float offset = 0;
                    ReplayIdentity id = go.GetComponent<ReplayTransform>().ReplayObject.ReplayIdentity;

                    while (offset < duration)
                    {
                        ReplaySnapshot snapshot = target.FetchSnapshot(offset);
                        //snapshot.
                        offset += step;

                        // Get data for object with the target id
                        ReplayState state = snapshot.RestoreSnapshot(id);

                        // Create an object serializer instance
                        ReplayObjectSerializer objectSerializer = new ReplayObjectSerializer();

                        // Run deserialize
                        objectSerializer.OnReplayDeserialize(state);

                        // Access component data - Similar properties exist for replay variables, events methods etc.
                        IList<ReplayComponentData> components = objectSerializer.ComponentStates;

                        foreach (ReplayComponentData data in components)
                        {
                            
                            if (data.BehaviourIdentity != go.GetComponent<ReplayTransform>().ReplayIdentity)
                            {
                                continue;
                            }
                            Debug.Log("Target replay component id: " + data.BehaviourIdentity);

                            // Get the serialize type for this component
                            Type serializerType =
                                ReplaySerializers.GetSerializerTypeFromID(data.ComponentSerializerID);

                            // Check for transform
                            if (serializerType == typeof(ReplayTransformSerializer))
                            {
                                ReplayTransformSerializer transformSerializer = new ReplayTransformSerializer();

                                // Read the transform data
                                data.ComponentStateData.PrepareForRead();
                                transformSerializer.OnReplayDeserialize(data.ComponentStateData);

                                // Debug.Log("Position: " + transformSerializer.Position);
                                // Debug.Log("Rotation: " + transformSerializer.Rotation);
                                // Debug.Log("Scale: " + transformSerializer.Scale);

                                positions.Add(transformSerializer.Position);

                                if (counter == 0)
                                {
                                    m_Positions0.Add(transformSerializer.Position);
                                    m_Rotations0.Add(transformSerializer.Rotation);
                                }
                                if (counter == 1)
                                {
                                    m_Positions1.Add(transformSerializer.Position);
                                    m_Rotations1.Add(transformSerializer.Rotation);
                                }
                            }
                        }
                        go.GetComponent<LineRenderer>().positionCount = positions.Count;
                        go.GetComponent<LineRenderer>().SetPositions(positions.ToArray());
                    }
                    counter++;
                }

                m_DotProducts0.Clear();
                for (int i = 0; i < m_Positions0.Count; i++)
                {
                    Vector3 fwd_0 = (m_Rotations0[i] * Vector3.forward).normalized;
                    Vector3 fwd_1 = (m_Rotations1[i] * Vector3.forward).normalized;
                    m_DotProducts0.Add(Vector3.Dot((m_Positions1[i] - m_Positions0[i]).normalized, fwd_0));
                }
            }
        }
    }
}