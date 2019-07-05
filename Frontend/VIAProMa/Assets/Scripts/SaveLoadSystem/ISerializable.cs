using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializable
{
    SerializedObject Serialize();
    void Deserialize(SerializedObject serializedObject);

}
