using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalKeyboard : MonoBehaviour
{
    public bool IsCapturingKeyboard { get; set; }

    private Keyboard keyboard;

    private void Awake()
    {
        keyboard = GetComponent<Keyboard>();
        IsCapturingKeyboard = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCapturingKeyboard)
        {
            foreach(char c in Input.inputString)
            {
                if (c == '\b') // backspace
                {
                    keyboard.Backspace();
                }
                else
                {
                    keyboard.AddLetter(c);
                }
            }
        }
    }
}
