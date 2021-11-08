using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace BarGraph.VittorCloud
{
    public class CreateBarGraph : MonoBehaviour
    {
        [MenuItem("ViitorCloud/BarGraph/Create BarGraph")]
        public static void CreatePieChart()
        {
            GameObject go = Instantiate(Resources.Load("BarGraph") as GameObject);
            go.name = "BarGraph";
        }
    }
}
