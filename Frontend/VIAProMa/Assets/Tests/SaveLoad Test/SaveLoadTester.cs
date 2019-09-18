using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadTester : MonoBehaviour
{
    public string loadJson;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log(SaveLoadManager.Instance.SerializeSaveGame());
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!string.IsNullOrEmpty(loadJson))
            {
                SaveLoadManager.Instance.DeserializeSaveGame(loadJson);
            }
        }
    }
}
