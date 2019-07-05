using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadTester : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(SaveLoadManager.Instance.SerializeSaveGame());
        }
    }
}
