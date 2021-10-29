using System.Collections;
using System.Collections.Generic;
using Rerun;
using UnityEngine;

namespace Rerun
{
    public class RerunLineRendererVisualizer : MonoBehaviour
    {
        [SerializeField]
        private RerunVisualizer m_RerunVisualizer;

        private LineRenderer m_LineRenderer;

        public int x = 0;
        public int z = 3;

        //[SerializeField]
        private GameObject m_Cursor;

        private List<float> m_DotProducts;

        // Start is called before the first frame update
        void Start()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.positionCount = 0;
            m_LineRenderer.startWidth = 0.1f;
            m_LineRenderer.endWidth = 0.1f;

            m_DotProducts = new List<float>();

            m_Cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_Cursor.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            m_Cursor.transform.position = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            m_DotProducts = m_RerunVisualizer.GetDotProduct();
            m_LineRenderer.positionCount = 0;

            int counter = 0;
            foreach (var dot in m_DotProducts)
            {
                m_LineRenderer.positionCount = counter + 1;

                m_LineRenderer.SetPosition(counter, new Vector3(counter / 10.0f, dot + transform.position.y, transform.position.z));
                counter++;
            }

            if (m_LineRenderer.positionCount > 0)
            {
                m_Cursor.transform.position = Vector3.MoveTowards(m_Cursor.transform.position,
                    m_LineRenderer.GetPosition(
                        (int)Mathf.RoundToInt((m_RerunVisualizer.normalizedTime *
                                               (float)(m_LineRenderer.positionCount - 1.0f)))), 0.1f);
            }
        }
    }
}