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

        [SerializeField]
        private float m_Samples = 20f;

        [SerializeField]
        private List<GameObject> m_ReplayTransformObjects;

        private float m_NormalizedTime = 0;

        private List<Transform> m_Transforms;
        
        List<Vector3> positions_0;
        List<Vector3> positions_1;
        List<Quaternion> rotations_0;
        List<Quaternion> rotations_1;

        private List<float> dotProducts_0;

        
        public float normalizedTime
        {
            get => m_NormalizedTime;
        }

        public List<float> GetDotProduct()
        {
            return dotProducts_0;
        }

        // Start is called before the first frame update
        void Start()
        {
            //InitObjects();
            m_Transforms = new List<Transform>();
            
            positions_0 = new List<Vector3>();
            positions_1 = new List<Vector3>();
            rotations_0 = new List<Quaternion>();
            rotations_1 = new List<Quaternion>();

            dotProducts_0 = new List<float>();
            
        }

        private void Update()
        {
            ReplayHandle handle = m_RerunManager.playbackHandle;
            ReplayStorageTarget target = ReplayManager.GetReplayStorageTarget(handle);
            float duration = target.Duration;
            float step = duration / m_Samples;
                
            // Get the playback time
            ReplayTime playbackTime = ReplayManager.GetPlaybackTime(handle);
            float time = playbackTime.NormalizedTime;
                
            m_NormalizedTime = playbackTime.NormalizedTime;
        }

        private void InitObjects()
        {
            // Add LineRenderer to each object to be visualized
            foreach (var go in m_ReplayTransformObjects)
            {
                var lr = go.AddComponent<LineRenderer>();
                //var lr = go.AddComponent<TrailRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.positionCount = 0;
                lr.startWidth = 0.01f;
                lr.endWidth = 0.01f;
                Color c1 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1);
                Color c2 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1);
                
                lr.SetColors(c1, c1);
                // lr.startColor = c1;
                // lr.endColor = c2;
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
                ReplayTime playbackTime = ReplayManager.GetPlaybackTime(handle);
                float time = playbackTime.NormalizedTime;
                
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

                                Debug.Log("Position: " + transformSerializer.Position);
                                Debug.Log("Rotation: " + transformSerializer.Rotation);
                                Debug.Log("Scale: " + transformSerializer.Scale);

                                positions.Add(transformSerializer.Position);

                                if (counter == 0)
                                {
                                    positions_0.Add(transformSerializer.Position);
                                    rotations_0.Add(transformSerializer.Rotation);
                                }
                                if (counter == 1)
                                {
                                    positions_1.Add(transformSerializer.Position);
                                    rotations_1.Add(transformSerializer.Rotation);
                                }

                                

                            }
                        }
                        go.GetComponent<LineRenderer>().positionCount = positions.Count;
                        go.GetComponent<LineRenderer>().SetPositions(positions.ToArray());
                    }
                    counter++;
                }

                dotProducts_0.Clear();
                for (int i = 0; i < positions_0.Count; i++)
                {
                    Vector3 fwd_0 = (rotations_0[i] * Vector3.forward).normalized;
                    Vector3 fwd_1 = (rotations_1[i] * Vector3.forward).normalized;
                    dotProducts_0.Add(Vector3.Dot((positions_1[i] - positions_0[i]).normalized, fwd_0));
                }
            }
        }
    }
}