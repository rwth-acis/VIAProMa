using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    private float length;

    public float Length
    {
        get { return length; }
        set
        {
            length = value;
            if (value <= 0)
            {
                gameObject.SetActive(false);
                Debug.Log("deactivated gameobject", this);
            }
            else
            {
                Debug.Log("set length", this);
                gameObject.SetActive(true);
                transform.localScale = new Vector3(
                    transform.localScale.x,
                    value,
                    transform.localScale.z);
            }
        }
    }

    public ValueRange ValueRange { get; set; }
}

public enum ValueRange
{
    INT, FLOAT, STRING
}
