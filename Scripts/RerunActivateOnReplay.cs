using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateReplay;

namespace Rerun
{


    public class RerunActivateOnReplay : MonoBehaviour
    {
        private RerunManager m_rerunManager;
        // Start is called before the first frame update
        void Start()
        {
            m_rerunManager = FindObjectOfType<RerunManager>();
            
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }
}