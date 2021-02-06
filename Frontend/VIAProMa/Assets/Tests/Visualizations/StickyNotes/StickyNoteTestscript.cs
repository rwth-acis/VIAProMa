using i5.VIAProMa.Visualizations.Common;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.UI.InputFields;

namespace i5.VIAProMa.Visualizations.Haftnotizen{

    public class StickyNoteTestscript : MonoBehaviour
    {   
        // References to the Note Instance and its components
        [Header("Sticky Note Instance")]
        [SerializeField] private GameObject noteInstance;
        [Header("Sticky Note Components")]
        [SerializeField] private GameObject pinButton;
        [SerializeField] private GameObject unpinButton;
        [SerializeField] private GameObject colorButton;
        [SerializeField] private GameObject editButton;
        [SerializeField] private GameObject clearButton;
        [SerializeField] private InputField inputField;
        [SerializeField] private HaftnotizenVisualController controller;

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
                
            }
            // Cycle Color
            if (Input.GetKeyDown(KeyCode.F6))
            {
                controller.colorCycler.cycle();
            }
            // Edit Note
            if (Input.GetKeyDown(KeyCode.F7))
            {
                inputField.OnClick();
            }
            // Clear note
            if (Input.GetKeyDown(KeyCode.F8))
            {
                controller.inputField.Text = "";
            }
        }
    }
}

    
