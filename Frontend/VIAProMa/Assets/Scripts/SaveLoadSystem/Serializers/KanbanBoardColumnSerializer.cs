using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KanbanBoardColumnVisualController))]
public class KanbanBoardColumnSerializer : MonoBehaviour, ISerializable
{
    private const string colorKey = "kanban_color";
    private const string widthKey = "kanban_width";
    private const string heightKey = "kanban_height";

    private KanbanBoardColumnVisualController columnVisualController;

    private void Awake()
    {
        columnVisualController = GetComponent<KanbanBoardColumnVisualController>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        Vector3 colorVector = SerializedObject.TryGet(colorKey, serializedObject.Vector3s, gameObject, out bool found);
        if (found)
        {
            columnVisualController.Color = new Color(colorVector.x, colorVector.y, colorVector.z);
        }
        float width = SerializedObject.TryGet(widthKey, serializedObject.Floats, gameObject, out found);
        if (found)
        {
            columnVisualController.Width = width;
        }
        float height = SerializedObject.TryGet(heightKey, serializedObject.Floats, gameObject, out found);
        if (found)
        {
            columnVisualController.Height = height;
        }
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Vector3s.Add(colorKey, 
            new Vector3(
            columnVisualController.Color.r,
            columnVisualController.Color.g,
            columnVisualController.Color.b));
        serializedObject.Floats.Add(heightKey, columnVisualController.Height);
        serializedObject.Floats.Add(widthKey, columnVisualController.Width);
        return serializedObject;
    }
}
