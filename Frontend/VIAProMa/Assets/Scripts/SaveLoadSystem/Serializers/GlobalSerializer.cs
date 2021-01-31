using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.SaveLoadSystem.Core;
using UnityEngine;

public class GlobalSerializer : MonoBehaviour, ISerializable
{
    private const string globalKey = "GLOBAL"; 
    public void Deserialize(SerializedObject serializedObject)
    {

    }

    public SerializedObject Serialize()
    {
        SerializedObject obj = new SerializedObject();
        obj.Bools[globalKey] = true;
        return obj;
    }
}
