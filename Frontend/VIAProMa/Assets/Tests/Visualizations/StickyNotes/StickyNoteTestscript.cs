using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class StickyNoteTestscript : MonoBehaviour
{   
    // References to the Note Instance and its components
    [Header("Sticky Note Instance")]
    [SerializeField] private GameObject noteInstance;
    [Header("Sticky Note Components")]
    [SerializeField] private Interactable pinButton;
    [SerializeField] private Interactable unpinButton;
    [SerializeField] private Interactable colorButton;
    [SerializeField] private Interactable editButton;
    [SerializeField] private Interactable keyboardAcceptButton;
    [SerializeField] private Interactable clearButton;

    // Helper variables
    private bool pinned = false;
    private bool write = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {   
        // Pin/Unpin Note
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (!pinned)
            {
                pinButton.TriggerOnClick();
                pinned = true;
            }
            else
            {
                unpinButton.TriggerOnClick();
                pinned = false;
            }
            
        }
        // Cycle Color
        if (Input.GetKeyDown(KeyCode.F6))
        {
            colorButton.TriggerOnClick();
        }
        // Edit Note
        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (!write)
            {
                editButton.TriggerOnClick();
                write = true;
            }
            else
            {
                keyboardAcceptButton.TriggerOnClick();
                write = false;
            }
        }
        // Clear note
        if (Input.GetKeyDown(KeyCode.F8))
        {
           clearButton.TriggerOnClick();
        }
    }
}


    
