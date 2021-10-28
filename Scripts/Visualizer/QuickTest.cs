using System.Collections;
using System.Collections.Generic;
using Rerun;
using UnityEngine;

public class QuickTest : MonoBehaviour
{
    [SerializeField]
    private RerunVisualizer m_RerunVisualizer;

    private LineRenderer m_LineRenderer;

    public int x = 0;
    public int z = 3;

    [SerializeField]
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
            
            m_LineRenderer.SetPosition(counter,new Vector3(counter/10.0f,dot+2,z));
            counter++;
        }

        /*m_Cursor.transform.position =
            new Vector3(m_RerunVisualizer.normalizedTime * (m_DotProducts.Count / 10.0f), 0, z);*/
        m_Cursor.transform.position = Vector3.MoveTowards(m_Cursor.transform.position,
            m_LineRenderer.GetPosition((int)(m_RerunVisualizer.normalizedTime * (float)m_LineRenderer.positionCount)),0.1f);
        
            //m_LineRenderer.GetPosition((int)(m_RerunVisualizer.normalizedTime * (float) m_LineRenderer.positionCount));
        
    }
}
