using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace BarGraph.VittorCloud
{
    public class BarProperty : MonoBehaviour
    {
        #region publicVariable
        public TextMeshPro BarLabel;
        public GameObject LabelContainer;
        private string barValue;

        public MeshRenderer barMesh;

        public BarMouseClick barClickEvents;


        float ScaleFactor;
        #endregion

        #region privateVariables
        float originalYscale = 0;

        public string BarValue { get => barValue; set => barValue = value; }
        #endregion

        #region UnityCallBacks


        private void Awake()
        {
            // Debug.Log("SetBarLabelVisible : " + LabelContainer.transform.localScale.y, this.gameObject);
            originalYscale = LabelContainer.transform.localScale.y;
            Debug.Log("originalYscale : " + LabelContainer.transform.lossyScale.y, this.gameObject);
            LabelContainer.SetActive(false);

        }
      
        public void OnEnable()
        {
            LabelContainer.SetActive(false);
        }




        #endregion

        #region Customfunctions
        public void SetBarLabelVisible(string value, float scaleFactor)
        {

            BarLabel.text = value;
            LabelContainer.SetActive(true);
            Debug.Log("SetBarLabelVisible : " + LabelContainer.transform.localScale.y + " : " + transform.localScale.y, this.gameObject);
            if (transform.localScale.y == 0)
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x, originalYscale * scaleFactor/ transform.localScale.x, LabelContainer.transform.localScale.z);
            else
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x, originalYscale * scaleFactor / transform.localScale.y, LabelContainer.transform.localScale.z);


        }
        public void SetBarLabel(string value, float factor)
        {
            BarLabel.text = value;
            LabelContainer.SetActive(false);
            ScaleFactor = factor;


        }

        public void SetLabelEnabel()
        {

            Debug.Log("SetBarLabelVisible : " + LabelContainer.transform.localScale.y + " : " + transform.localScale. y, this.gameObject);
            if (transform.localScale.y == 0)
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x, originalYscale / (transform.localScale.x ), LabelContainer.transform.localScale.z);
            else
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x , originalYscale * ScaleFactor / transform.localScale.y, LabelContainer.transform.localScale.z);

            LabelContainer.SetActive(true);

        }


        public void SetBarColor(Color barColor)
        {

            barMesh.material.color = barColor;
        }

        public Color GetBarColor()
        {

            return barMesh.material.color;
        }


        public void SetBarMat(Material barMat)
        {

            barMesh.material = barMat;
        }



        #endregion

    }
}
