using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTestRunner : MonoBehaviour
{
    public string initialText;
    public List<string> autocompleteOptions;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Keyboard.Instance.Open(new Vector3(0, 0, 2), Vector3.zero, initialText, autocompleteOptions);
        }
    }
}
