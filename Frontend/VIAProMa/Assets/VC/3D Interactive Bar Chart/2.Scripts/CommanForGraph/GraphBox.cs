using BarGraph.VittorCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graph.VittorCloud
{
    public class GraphBox : MonoBehaviour
    {
        #region publicVariables




        public GameObject XAxis, YAxis, ZAxis;
        public GraphPoint XPoint, YPoint, ZPoint;
        public GameObject XYPlane, XZPlane, YZPlane;

        public GameObject barParent;
        public float planeSizeOffset = 1;
        public GameObject horizontalGroup;



        #endregion

        #region privateVariables

        
        protected List<GraphPoint> ListOfXPoint;
        protected List<GraphPoint> ListOfYPoints;
        protected List<GraphPoint> ListOfZPoints;
        public List<BarGorup> ListOfGroups;
        protected float XpointCount;
        protected float YpointCount;
        protected float ZpointCount;
        protected float graphScaleFactor;

        float XLength;
        float YLength;
        float ZLength;
        float xOffset;
        float yOffset;
        float zOffset;


        #endregion

        #region UnityCallBacks
        // Start is called before the first frame update
        public void Awake()
        {
            ListOfXPoint = new List<GraphPoint>();
            ListOfYPoints = new List<GraphPoint>();
            ListOfZPoints = new List<GraphPoint>();
            ListOfGroups = new List<BarGorup>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region CustomeGraphBehaviours

        public void setBarScale(float scaleFactorvalue)
        {

            graphScaleFactor = scaleFactorvalue;

        }


        public void InitGraphBox(float xLength, float yLength, float zLength, float xSegment, float ySegment, float zSegment)
        {


            XLength = xLength;
            YLength = yLength;
            ZLength = zLength;




           xOffset = xSegment == 0 ? XLength : xSegment;
           yOffset = ySegment == 0 ? YLength : ySegment;
           zOffset = zSegment == 0 ? ZLength : zSegment;


            XYPlane.transform.localScale = new Vector3(XLength + xOffset, YLength + yOffset, XYPlane.transform.localScale.z);
            XZPlane.transform.localScale = new Vector3(XLength + xOffset, XZPlane.transform.localScale.y, ZLength + zOffset);
            YZPlane.transform.localScale = new Vector3(YZPlane.transform.localScale.x, YLength + yOffset, ZLength + zOffset);

            barParent.transform.rotation = transform.rotation;




            ////gameObject.AddComponent<BarGraphManager>();

        }

        public void InitXAxis(int xMaxSize, float xStart, float xSegment, float xRowOffset)
        {


            XAxis.transform.localScale = new Vector3(XLength + xSegment, XAxis.transform.localScale.y, XAxis.transform.localScale.z);

            if (xSegment == 0)
            {

                XpointCount = xMaxSize;

            }
            else
            {

                XpointCount = ((XLength - xStart) / xSegment) + 1;

            }

            for (int i = 0; i < XpointCount; i++)
            {


                float distance = 1;
                Vector3 pos;

                if (i == 0)
                {
                    distance = xStart;
                    pos = transform.localPosition + new Vector3(distance, 0, 0);
                }
                else
                {
                    distance = xSegment;
                    pos = ListOfXPoint[i - 1].transform.localPosition + new Vector3(distance, 0, 0);
                }




                GraphPoint temp = Instantiate(XPoint, transform.position, transform.rotation);


                temp.transform.parent = transform;
                temp.transform.localPosition = pos;

                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition= new Vector3(0, 0,  -(ZLength + (1.8f * zOffset)))/ graphScaleFactor;
               
                ListOfXPoint.Add(temp);

            }


        }
        public void InitYAxis(int yMaxSize, float yStart, float ySegment)
        {


            YAxis.transform.localScale = new Vector3(YAxis.transform.localScale.x, YLength + ySegment, YAxis.transform.localScale.z);
            YpointCount = ((YLength - yStart) / ySegment) + 1;
            for (int i = 0; i < YpointCount; i++)
            {

                float distance = 1;
                Vector3 pos;
                if (i == 0)
                {
                    distance = yStart;
                    pos = transform.localPosition + new Vector3(0, distance, 0);
                }
                else
                {
                    distance = ySegment;

                    pos = ListOfYPoints[i - 1].transform.localPosition + new Vector3(0, distance, 0);
                }
                GraphPoint temp = Instantiate(YPoint, transform.localPosition, transform.rotation);
                temp.transform.parent = transform;
                temp.transform.localPosition = pos;

                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition = new Vector3(XLength+ (1.8f*xOffset), 0, 0) / graphScaleFactor;
                ListOfYPoints.Add(temp);

            }


        }
        public void InitZAxis(int zMaxSize, float zStart, float zSegment, float zRowOffset, float xStart, float xSegment)
        {


            ZAxis.transform.localScale = new Vector3(ZAxis.transform.localScale.x, ZAxis.transform.localScale.y, ZLength + zSegment);


            if (zSegment == 0)
            {

                ZpointCount = zMaxSize;

            }
            else
            {

                ZpointCount = ((ZLength - zStart) / zSegment) + 1;

            }


            for (int i = 0; i < ZpointCount; i++)
            {

                float distance = 1;
                Vector3 pos;
                if (i == 0)
                {
                    distance = zStart;
                    pos = transform.localPosition + new Vector3(0, 0, -distance);
                }
                else
                {
                    distance = zSegment;

                    pos = ListOfZPoints[i - 1].transform.localPosition + new Vector3(0, 0, -distance);
                }
                GraphPoint temp = Instantiate(ZPoint, transform.position, transform.rotation);
                temp.transform.parent = transform;
                temp.transform.localPosition = pos;


                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition = temp.labelContainer.localPosition = new Vector3(XLength + (1.8f * xOffset), 0, 0)/ graphScaleFactor;
                ListOfZPoints.Add(temp);

                GameObject grouptemp = GameObject.Instantiate(horizontalGroup, transform.position, transform.rotation);

                grouptemp.transform.parent = barParent.transform;
                grouptemp.transform.localPosition = pos + new Vector3((zRowOffset * i), 0, 0);

                grouptemp.GetComponent<RectTransform>().sizeDelta = new Vector2(XLength, YLength);
                grouptemp.GetComponent<HorizontalLayoutGroup>().padding.left = (int)xStart;
                grouptemp.GetComponent<HorizontalLayoutGroup>().spacing = (int)xSegment;
                grouptemp.transform.localScale = Vector3.one;
                ListOfGroups.Add(grouptemp.GetComponent<BarGorup>());
                // Debug.Log("count incremented =======" + ListOfGroups.Count);
            }



        }

        public void AssignAxisName(int xIndex, int zIndex, string xValue, string zValue)
        {

            ListOfZPoints[zIndex].labelText = zValue;
            ListOfXPoint[xIndex].labelText = xValue.ToString();

        }
        public void FetchYPointValues(int ymin, int ymax)
        {

            //Debug.Log("Final values min max " + ymin + " , " + ymax);
            float range = ymax - ymin;
            //Debug.Log("range  & YpointCount" + range + " , " + YpointCount);
            float offset = range / YpointCount;
            //Debug.Log("offset " + offset);
            float value = ymin;
            for (int i = 0; i < ListOfYPoints.Count; i++)
            {
                value += offset;
                ListOfYPoints[i].labelText = value.ToString();
                //Debug.Log("value " + value);
            }
        }

        public void FetchXPointValues(int xmin, int xmax)
        {

            //Debug.Log("Final values min max " + ymin + " , " + ymax);
            float range = xmax - xmin;
            //Debug.Log("range  & YpointCount" + range + " , " + YpointCount);
            float offset = range / XpointCount;
            //Debug.Log("offset " + offset);
            float value = xmin;
            for (int i = 0; i < ListOfXPoint.Count; i++)
            {
                value += offset;
                ListOfXPoint[i].labelText = value.ToString();
                //Debug.Log("value " + value);
            }
        }

        public void FetchZPointValues(int zmin, int zmax)
        {

            //Debug.Log("Final values min max " + ymin + " , " + ymax);
            float range = zmax - zmin;
            //Debug.Log("range  & YpointCount" + range + " , " + YpointCount);
            float offset = range / ZpointCount;
            //Debug.Log("offset " + offset);
            float value = zmin;
            for (int i = 0; i < ListOfZPoints.Count; i++)
            {
                value += offset;
                ListOfZPoints[i].labelText = value.ToString();
                //Debug.Log("value " + value);
            }
        }



        #endregion
    }
}

