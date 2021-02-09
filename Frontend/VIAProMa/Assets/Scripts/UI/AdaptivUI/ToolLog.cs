using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLog
{
    public ToolAction toolActionType { get; set; }
    public GameObject changedObject { get; set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
}
public enum ToolAction
{
    Delete,
    Move
}
