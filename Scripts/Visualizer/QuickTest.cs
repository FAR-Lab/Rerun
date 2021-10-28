using System.Collections;
using System.Collections.Generic;
using Rerun;
using UnityEngine;

public class QuickTest : MonoBehaviour
{
    [SerializeField]
    private RerunVisualizer rv;

    private LineRenderer lr;

    public int x = 0;
    public int z = 3;

    private List<float> dotProducts;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        dotProducts = new List<float>();
        
    }

    // Update is called once per frame
    void Update()
    {
        dotProducts = rv.GetDotProduct();
        lr.positionCount = 0;

        int counter = 0;
        foreach (var dot in dotProducts)
        {
            lr.positionCount = counter + 1;
            
            lr.SetPosition(counter,new Vector3(counter/10.0f,dot,z));
            counter++;
        }
    }
}
