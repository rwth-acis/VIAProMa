using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializer
{
    SerializedData Serialize();
    void Deserialize(SerializedData serializedValues);

}
