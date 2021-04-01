using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueEditBar : MonoBehaviour
{

    [SerializeField] int BarSize;
    [SerializeField] GameObject[] Buttons;

    // Start is called before the first frame update
    void Start()
    {
        Buttons = new GameObject[BarSize];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
