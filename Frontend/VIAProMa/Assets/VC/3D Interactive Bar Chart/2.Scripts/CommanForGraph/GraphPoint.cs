using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Graph.VittorCloud
{
    public class GraphPoint : MonoBehaviour
    {

        #region publicVariable
        public TextMeshPro label;
        public Transform labelContainer;

        public string labelText
        {
            get { return labelValue; }
            set
            {
                labelValue = value;
                label.text = labelValue;
            }
        }

        #endregion

        #region privateVariable

        string labelValue;

        #endregion


    }
}
