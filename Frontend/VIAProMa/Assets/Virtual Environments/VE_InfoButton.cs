using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VE_InfoButton : MonoBehaviour
{
    [SerializeField] private GameObject InfoTable;

    public void toggleTable()
    {
        InfoTable.SetActive(!InfoTable.activeSelf);
    }
}
