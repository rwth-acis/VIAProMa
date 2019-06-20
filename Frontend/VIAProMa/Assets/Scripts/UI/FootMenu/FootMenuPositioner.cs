using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMenuPositioner : MonoBehaviour
{
    [SerializeField] private HeightChanger heightChanger;

    private void Awake()
    {
        if (heightChanger == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(heightChanger));
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Camera.main.transform.position.x,
            heightChanger.Height,
            Camera.main.transform.position.z
            );
        transform.eulerAngles = new Vector3(
            0f,
            Camera.main.transform.eulerAngles.y,
            0f
            );
    }
}
