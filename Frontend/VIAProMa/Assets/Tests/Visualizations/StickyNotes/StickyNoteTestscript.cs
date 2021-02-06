using i5.VIAProMa.Visualizations.Common;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNoteTestscript : MonoBehaviour
{   
    // Reference to Note Prefab
    [SerializeField] private GameObject notePrefab;
    // Variable to hold the created note instance
    private GameObject noteInstance;

    // Start is called before the first frame update
    void Start()
    {
        noteInstance = Instantiate(notePrefab, new Vector3(0,0,2), new Quaternion(0, -12, 0, 100));
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
            
        }
        // Edit Note
        if (Input.GetKeyDown(KeyCode.F7))
        {
            
        }
        // Clear note
        if (Input.GetKeyDown(KeyCode.F8))
        {
            
        }
    }
}

    
