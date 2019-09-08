using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataConverter<T>
{
    T FloatToValue(float f);

    float ValueToFloat(T value);

    string ValueToString(T value);
}
