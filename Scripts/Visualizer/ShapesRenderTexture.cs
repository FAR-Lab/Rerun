using System;
using System.Collections.Generic;
using Rerun;
using Shapes;
using UnityEngine;
using UnityEngine.UI;

namespace Rerun
{


    //public class ShapesRT : MonoBehaviour {
    [ExecuteAlways]
    public class ShapesRenderTexture : ImmediateModeShapeDrawer
    {

        // this camera component should be disabled,
        // to prevent it from automatically rendering every frame
        public Camera rtCamera;
        private RenderTexture rt;

        [SerializeField]
        private float nt = 1;

        private List<float> dots;

        [SerializeField]
        private RerunVisualizer rv;
        
        private List<float> m_DotProducts;

        private PolylinePath m_Path;
        void Awake()
        {
            // Create an RT
            rt = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);

            // I attached this script to a Unity quad for preview purposes
            //GetComponent<MeshRenderer>().material.mainTexture = rt;
            GetComponent<RawImage>().material.mainTexture = rt;

            rtCamera.targetTexture = rt; // make sure the camera is configured to target the RT
            /*rt.DiscardContents( true, true ); // clear before drawing, in case you have old data in the RT
            using( Draw.Command( rtCamera ) ) { // set up a Shapes draw command in the camera
                Draw.RectangleBorder( Vector2.zero, new Vector2( 1, 1 ), 0.025f, 0.1f, Color.white );
                //Draw.Scale( 1f / 3f );
                Draw.Line( Vector2.zero, Vector2.right, 0.15f, Color.red );
                Draw.Line( Vector2.zero, Vector2.up, 0.15f, Color.green );
                Draw.Disc( Vector2.zero, 0.15f, Color.white );
            }*/

            //rtCamera.Render(); // Render the camera immediately

            dots = new List<float>();
            m_DotProducts = new List<float>();

        }

        private void Update()
        {
            nt = rv.normalizedTime;
            dots = rv.GetDotProduct();
            m_DotProducts = rv.GetDotProduct();

        }

        public void UpdatePolylinePath()
        {
            
        }

        public override void DrawShapes(Camera cam)
        {

            /* using( Draw.Command( cam ) ){
     
                 // set up static parameters. these are used for all following Draw.Line calls
                 Draw.LineGeometry = LineGeometry.Volumetric3D;
                 Draw.ThicknessSpace = ThicknessSpace.Pixels;
                 Draw.Thickness = 4; // 4px wide
     
                 // set static parameter to draw in the local space of this object
                 Draw.Matrix = transform.localToWorldMatrix;
     
                 // draw lines
                 Draw.Line( Vector3.zero, Vector3.right,   Color.red   );
                 Draw.Line( Vector3.zero, Vector3.up,      Color.green );
                 Draw.Line( Vector3.zero, Vector3.forward, Color.blue  );
             }*/

            //rt.DiscardContents( true, true ); // clear before drawing, in case you have old data in the RT
            using (Draw.Command(rtCamera))
            {
                // set up a Shapes draw command in the camera
                /*Draw.RectangleBorder( Vector2.zero, new Vector2( 1, 1 ), 0.025f, 0.1f, Color.white );
                //Draw.Scale( 1f / 3f );
                Draw.Line( Vector2.zero, Vector2.right, 0.15f, Color.red );
                Draw.Line( Vector2.zero, Vector2.up, 0.15f, Color.green );
                //Draw.Disc( Vector2.zero, 0.15f, Color.white );
                Draw.Arc(new Vector3(0,0,-1), Mathf.PI/2, 2f*Mathf.PI);*/

                //Draw.Scale( rv.normalizedTime)
                float start = -2;
                float end = 2;
                //Draw.Disc(new Vector3(nt * end, 0, 0), 0.2f);

                PolylinePath p = new PolylinePath();
                for (int i = 0; i < m_DotProducts.Count; i++)
                {
                    p.AddPoint((end*(float)i/(float)(m_DotProducts.Count-1)),m_DotProducts[i],0);
                    Color c_ = new Color(1-m_DotProducts[i], 0, m_DotProducts[i]);
                    p.SetColor(i,c_);
                }
                
                Draw.Polyline( p, closed:false, thickness:0.05f);
                
                int index = Mathf.RoundToInt(nt * (float)(m_DotProducts.Count-1.0f));
                Color c = new Color(1, 0, 0);
                //Draw.Rectangle(Vector3.zero, new Rect(0,0,1,1),c);
                c = new Color(1-m_DotProducts[index], 0, m_DotProducts[index]);
                Draw.Disc(new Vector3(nt * end, m_DotProducts[index], 0), 0.1f, c);
                Draw.Line(new Vector3(0, 0, 0),new Vector3(end, 0, 0));
                Draw.Line(new Vector3(0, 0, 0),new Vector3(nt*end, 0, 0),Color.yellow);

                
                //Draw.Disc(new Vector3(nt * end, nt, 0), 0.2f);

                //Draw.Disc(new Vector3(0, nt*end,0),0.2f);

                /*for (int i = 1; i < dots.Count; i++)
                {
                    //Draw.Disc(new Vector3(nt*i, 0,0),0.2f*i);
                    //Draw.Disc(new Vector3(0, nt*end,0),0.2f);
                    Draw.Disc(new Vector3(0, nt*end,0),0.2f);
                    //Draw.Line(new Vector3((i - 1), dots[i-1], 0),new Vector3((i - 1), dots[i], 0));
                }*/


            }

            rtCamera.Render(); // Render the camera immediately

        }

    }
}