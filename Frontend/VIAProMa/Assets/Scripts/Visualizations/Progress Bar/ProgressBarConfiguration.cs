using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarConfiguration : MonoBehaviour, IWindow
{
    public event EventHandler WindowClosed;

    [SerializeField] private AppBarSpawner appBarSpawner;

    public bool WindowEnabled
    {
        get; set;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
    }
}
