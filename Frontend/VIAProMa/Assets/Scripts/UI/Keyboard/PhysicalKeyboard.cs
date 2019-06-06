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
        // handle special keys, e.g. DEL, left arrow, right arrow, Pos1, End
        if (IsCapturingKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                keyboard.Delete();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                keyboard.CursorPos--;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                keyboard.CursorPos++;
            }
            else if (Input.GetKeyDown(KeyCode.End))
            {
                keyboard.CursorPos = keyboard.Text.Length;
            }
            else if (Input.GetKeyDown(KeyCode.Home))
            {
                keyboard.CursorPos = 0;
            }
            else // handle general input
            {
                foreach (char c in Input.inputString)
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
}
