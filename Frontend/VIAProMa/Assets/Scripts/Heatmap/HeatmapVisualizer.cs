using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


/// <summary>
/// Manages visualization of data
/// </summary>
public class HeatmapVisualizer : MonoBehaviour
{
    public static HeatmapVisualizer instance;

    [Header("Visualization")]
    public float width = 10;
    public GameObject spherePrefab;

    [Header("Visualization Types")]
    public PointPosition position = new PointPosition();
    public PointColor color = new PointColor();
    public PointSize size = new PointSize();
    
    //private variables
    int[,] data;
    int min, max;
    PointRepresentation[] pointRepresentations;
    HeatmapDataManagement heatmapDataManagement;
    HeatmapPoint[,] points;
    Transform content;
    GreyBox greybox;


    /// <summary>
    /// Instanciates variables
    /// </summary>
    private void Awake()
    {
        heatmapDataManagement = GetComponent<HeatmapDataManagement>();
        heatmapDataManagement.OnDataChanged += OnUpdateData;
        greybox = greybox?greybox:FindObjectOfType<GreyBox>();
        content = transform.GetChild(0);
        instance = this;

        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        content.position = Vector3.zero;
        content.localScale = Vector3.one;

    }


    private void Start()
    {
        SetupPoints();
        data = heatmapDataManagement.data;
        pointRepresentations = new PointRepresentation[] { position, color, size };
        UpdateData(data);
        SetVisible(false);
    }


    /// <summary>
    /// Instanciates points in content as child objects
    /// </summary>
    public void SetupPoints()
    {
        int arraySize = heatmapDataManagement.arraySize;
        points = new HeatmapPoint[arraySize, arraySize];
        float stepSize = width / arraySize;
        Vector3 bottomLeft = transform.position - new Vector3((width - stepSize) / 2, 0, (width - stepSize) / 2);
        for (int x = 0; x < arraySize; x++)
        {
            for (int z = 0; z < arraySize; z++)
            {
                Vector3 position = bottomLeft + new Vector3(stepSize * x, 0, stepSize * z);
                GameObject point = Instantiate(spherePrefab, position, Quaternion.Euler(Vector3.zero), content);
                points[x, z] = point.GetComponent<HeatmapPoint>();
            }
        }
    }


    /// <summary>
    /// Set visibility of heatmap and greybox to [value]
    /// </summary>
    /// <param name="value">visibility</param>
    public static void SetVisible(bool value)
    {
        if (!instance) return;
        instance.content.gameObject.SetActive(value);
        instance.greybox.SetVisible(value);
    }


    /// <summary>
    /// Toggles visibility On/Off
    /// </summary>
    public static void Toggle()
    {
        if (!instance) return;
        SetVisible(!instance.content.gameObject.activeSelf);
    }


    /// <summary>
    /// Activates on changes to data in heatmapDatamanger
    /// </summary>
    public void OnUpdateData()
    {
        UpdateData(heatmapDataManagement.data);
    }


    /// <summary>
    /// Updates visualization to current data
    /// </summary>
    /// <param name="data">New Heatmapdata</param>
    public void UpdateData(int[,] data)
    {
        this.data = data;
        min = FindMin(data);
        max = FindMax(data);
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int z = 0; z < data.GetLength(1); z++)
            {
                foreach (var representation in pointRepresentations)
                {
                    representation.UpdateData(points[x,z], data[x,z]);
                }
            }
        }
    }
    

    /// <summary>
    /// Returns the linear mapping of the value from (min, max) to (0,1)
    /// </summary>
    /// <param name="value">The mapped value</param>
    /// <returns></returns>
    public float Value2Range(int value)
    {
        if (max - min == 0)
            return 0;
        else
            return (value - min) / (float)(max-min);
    }


    /// <summary>
    /// Returns the minimum value in a given 2D array
    /// </summary>
    /// <param name="array">The given array</param>
    /// <returns>Minimum entry in array</returns>
    public int FindMin(int[,] array)
    {
        int min = int.MaxValue;
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                min = Mathf.Min(min, array[x, z]);
            }
        }
        return min;
    }


    /// <summary>
    /// Returns the maximum value in a given 2D array
    /// </summary>
    /// <param name="array">The given array</param>
    /// <returns>Maximum entry in array</returns>
    public int FindMax(int[,] array)
    {
        int max = int.MinValue;
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int z = 0; z < array.GetLength(1); z++)
            {
                max = Mathf.Max(max, array[x, z]);
            }
        }
        return max;
    }


    /// <summary>
    /// Framework for all point representations
    /// </summary>
    [System.Serializable]
    public class PointRepresentation
    {
        public bool active = true;

        /// <summary>
        /// Applies representation if active and reverts it to standart if not
        /// </summary>
        /// <param name="point">Point to update</param>
        /// <param name="value">Coressponding data</param>
        public void UpdateData(HeatmapPoint point, int value)
        {
            if (active) ShowData(point, value);
            else StandardData(point);
        }
        
        public virtual void ShowData(HeatmapPoint point, int value) { }

        public virtual void StandardData(HeatmapPoint point) { }

        public static float Value2Range(int value)
        {
            return HeatmapVisualizer.instance.Value2Range(value);
        }
    }


    /// <summary>
    /// Moves point along Y-axis according to value
    /// </summary>
    [System.Serializable]
    public class PointPosition : PointRepresentation
    {
        public float height = 3;
        public override void ShowData(HeatmapPoint point, int value)
        {
            Vector3 position = point.transform.position;
            position.y = GetHeight(value);
            point.transform.position = position;
        }
        
        public override void StandardData(HeatmapPoint point)
        {
            Vector3 position = point.transform.position;
            position.y = 0;
            point.transform.position = position;
        }
        
        /// <summary>
        /// Get height for the given value in comparison to the max and min values
        /// </summary>
        /// <param name="value">The value of the sought height</param>
        /// <returns></returns>
        public float GetHeight(int value)
        {
            return Value2Range(value) * height;
        }
    }


    /// <summary>
    /// Changes color of point according to value and colorgradient
    /// </summary>
    [System.Serializable]
    public class PointColor : PointRepresentation
    {
        public Gradient colorGradient;
        public override void ShowData(HeatmapPoint point, int value)
        {
            Color color = GetColor(value);
            point.renderer.material.color = color;
        }
        
        public override void StandardData(HeatmapPoint point)
        {
            Color color = colorGradient.Evaluate(0);
            point.renderer.material.color = color;
        }
        
        /// <summary>
        /// Get color form the gradient for the given value in comparison to the max and min values
        /// </summary>
        /// <param name="value">The value of the sought color</param>
        /// <returns></returns>
        public Color GetColor(int value)
        {
            return colorGradient.Evaluate(Value2Range(value));
        }
    }


    /// <summary>
    /// Change size of point according to value
    /// </summary>
    [System.Serializable]
    public class PointSize : PointRepresentation
    {
        public float pointSize = 0.5f;
        public override void ShowData(HeatmapPoint point, int value)
        {
            Vector3 size = point.transform.localScale;
            size = Vector3.one * GetSize(value);
            point.transform.localScale = size;
        }

        public override void StandardData(HeatmapPoint point)
        {
            Vector3 size = point.transform.localScale;
            size = Vector3.one * pointSize;
            point.transform.localScale = size;
        }

        /// <summary>
        /// Get size for the given value in comparison to the max and min values
        /// </summary>
        /// <param name="value">The value of the sought size</param>
        /// <returns></returns>
        public float GetSize(int value)
        {
            return Value2Range(value) * pointSize;
        }
    }
}
