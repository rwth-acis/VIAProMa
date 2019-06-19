using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMenuPositioner : MonoBehaviour
{
    public float height = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Camera.main.transform.position.x,
            height,
            Camera.main.transform.position.z
            );
    }
}
