using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapPoint : MonoBehaviour
{
    public Renderer renderer;

    public int value;

    public void UpdateData(int value)
    {
        this.value = value;
        // Change Color
        Color color = HeatmapVisualizer.instance.GetColor(value);
        renderer.material.color = color;
        // Change position
        Vector3 position = transform.position;
        position.y = HeatmapVisualizer.instance.GetHeight(value);
        transform.position = position;
        // Change size
        Vector3 size = transform.localScale;
        size = Vector3.one * HeatmapVisualizer.instance.GetSize(value);
        transform.localScale = size;
    }
}
