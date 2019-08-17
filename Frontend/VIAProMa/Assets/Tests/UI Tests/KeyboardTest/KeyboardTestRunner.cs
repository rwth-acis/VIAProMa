using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTestRunner : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Keyboard.Instance.Open(new Vector3(0, 0, 2), Vector3.zero, new List<string>() { "hello", "infixHello", "Hat", "Hugo", "asdf", "world"});
            string text = "Hello World";
        }
    }
}
