using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rerun
{
    public class RerunLayoutManager : MonoBehaviour
    {
        [HideInInspector] public int currentView = 0;
        [SerializeField]
        private List<GameObject> m_Layouts;
        public GameObject myTimeline;
        public GameObject myHotkeyscreen;
        private bool AmadeLarger;
        private bool BmadeLarger;
        private GameObject ARectangle;
        private RectTransform ARectTransform;
        private GameObject BRectangle;
        private RectTransform BRectTransform;
        
        // Update is called once per frame
        private void Awake()
        {
            myTimeline.SetActive(true);
            myTimeline.transform.SetParent(m_Layouts[0].transform);
        }
        void Update()
        {
            if(GameObject.Find("ARectangle") != null)
            {
                ARectangle = GameObject.Find("ARectangle");
            }

            if (GameObject.Find("BRectangle"))
            {
                BRectangle = GameObject.Find("BRectangle");
            }
            if ((Input.GetKey(KeyCode.Keypad0) || Input.GetKey(KeyCode.Alpha0)) && !transform.GetComponentInParent<RerunInputManager>().InputOpen)
            {
                ToggleLayouts(0);
                if(ARectangle != null && AmadeLarger == true)
                {
                    ARectangle.SetActive(true);
                    // Reverse position operation
                    ARectTransform = ARectangle.GetComponent<RectTransform>();
                    ARectTransform.anchoredPosition = new Vector2(((ARectTransform.anchoredPosition.x/2) - Screen.width / 4), ((ARectTransform.anchoredPosition.y / 2) + (Screen.height / 4)));
                    // Reverse size operation
                    ARectTransform.sizeDelta = new Vector2(ARectTransform.rect.width / 2, ARectTransform.rect.height / 2);
                    AmadeLarger = false;
                }

                if (BRectangle != null && BmadeLarger == true)
                {
                    BRectangle.SetActive(true);
                    // Reverse position operation
                    BRectTransform = BRectangle.GetComponent<RectTransform>();
                    BRectTransform.anchoredPosition = new Vector2(((BRectTransform.anchoredPosition.x/2) + Screen.width / 4), ((BRectTransform.anchoredPosition.y / 2) + (Screen.height / 4)));
                    // Reverse size operation
                    BRectTransform.sizeDelta = new Vector2(BRectTransform.rect.width / 2, BRectTransform.rect.height / 2);
                    BmadeLarger = false;
                }
            }

            if ((Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Alpha1)) && !transform.GetComponentInParent<RerunInputManager>().InputOpen)
            {
                ToggleLayouts(1);
                if (ARectangle != null)
                {
                    ARectangle.SetActive(true);
                    if(AmadeLarger == false)
                    {
                        ARectTransform = ARectangle.GetComponent<RectTransform>();
                        ARectTransform.anchoredPosition = new Vector2(((Screen.width / 4 + ARectTransform.anchoredPosition.x))*2, (ARectTransform.anchoredPosition.y - (Screen.height / 4)) * 2);
                        ARectTransform.sizeDelta = new Vector2(ARectTransform.rect.width * 2, ARectTransform.rect.height * 2);
                        AmadeLarger = true;
                    }
                }

                if (BRectangle != null)
                {
                    BRectangle.SetActive(false);
                }
            }
            if ((Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Alpha2)) && !transform.GetComponentInParent<RerunInputManager>().InputOpen)
            {
                ToggleLayouts(2);
                if (ARectangle != null)
                {
                    ARectangle.SetActive(false);
                }

                if (BRectangle != null)
                {
                    BRectangle.SetActive(true);
                    if (BmadeLarger == false)
                    {
                        BRectTransform = BRectangle.GetComponent<RectTransform>();
                        BRectTransform.anchoredPosition = new Vector2((BRectTransform.anchoredPosition.x - Screen.width / 4) * 2, (BRectTransform.anchoredPosition.y - (Screen.height / 4))*(2));
                        BRectTransform.sizeDelta = new Vector2(BRectTransform.rect.width * 2, BRectTransform.rect.height * 2);
                        BmadeLarger = true;
                    }
                }
            }
            if ((Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.Alpha3)) && !transform.GetComponentInParent<RerunInputManager>().InputOpen)
            {
                ToggleLayouts(3);
                if (ARectangle != null)
                {
                    ARectangle.SetActive(false);
                }
                if (BRectangle != null)
                {
                    BRectangle.SetActive(false);
                }
            }
        }

        public void ToggleLayouts(int activeLayout)
        {
            for (int i = 0; i < m_Layouts.Count; i++)
            {
                if (activeLayout == i)
                {
                    m_Layouts[i].SetActive(true);
                    myTimeline.transform.SetParent(m_Layouts[i].transform);
                    myHotkeyscreen.transform.SetParent(m_Layouts[i].transform);
                    currentView = activeLayout;

                }
                else
                {
                    m_Layouts[i].SetActive(false);
                }
            }
        }
    }
}