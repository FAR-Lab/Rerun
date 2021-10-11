using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rerun
{
    public class RerunLayoutManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_Layouts;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Keypad0) || Input.GetKey(KeyCode.Alpha0))
            {
                ToggleLayouts(0);
            }
            if (Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Alpha1))
            {
                ToggleLayouts(1);
            }
            if (Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Alpha2))
            {
                ToggleLayouts(2);
            }
            if (Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.Alpha3))
            {
                ToggleLayouts(3);
            }
            if (Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Alpha4))
            {
                ToggleLayouts(4);
            }
        }

        private void ToggleLayouts(int activeLayout)
        {
            for (int i = 0; i < m_Layouts.Count; i++)
            {
                if (activeLayout == i)
                {
                    m_Layouts[i].SetActive(true);
                }
                else
                {
                    m_Layouts[i].SetActive(false);
                }
            }
        }
    }
}